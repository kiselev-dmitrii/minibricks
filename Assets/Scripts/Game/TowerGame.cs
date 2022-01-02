using System;
using System.Collections.Generic;
using MiniBricks.Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniBricks.Tetris {
    [Serializable]
    public class TowerGameDef {
        /// <summary>
        /// The distance between the top of the tower and the place where a piece spawns
        /// </summary>
        public float SpawnHeight;
        /// <summary>
        /// Templates of pieces
        /// </summary>
        public Piece[] PiecePrefabs;
        
        /// <summary>
        /// Amount of lives
        /// </summary>
        public int NumLives;

        /// <summary>
        /// Required Height
        /// </summary>
        public float RequiredHeight;

    }
    
    public interface ICommand {
        void Execute(Piece piece);
    }

    public enum GameResult {
        Win,
        Defeat
    }

    public enum GameState {
        NotStarted,
        Running,
        Paused,
        Finished
    }

    public class TowerGame : IDisposable {
        private readonly TowerGameDef def;
        private readonly Map map;
        private readonly PieceFactory pieceFactory;
        
        private readonly List<ICommand> commands;
        private GameState gameState;
        private GameResult? gameResult;
        private Piece currentPiece;
        private readonly List<Piece> placedPieces;
        private float maxHeight;
        private int numFalls;

        public TowerGame(TowerGameDef def, Map map, PieceFactory pieceFactory) {
            this.def = def;
            this.map = map;
            this.pieceFactory = pieceFactory;
            commands = new List<ICommand>();
            gameState = GameState.NotStarted;
            gameResult = null;
            currentPiece = null;
            placedPieces = new List<Piece>();
            maxHeight = 0;
            numFalls = 0;
            
            foreach (var fallTrigger in map.FallTriggers) {
                fallTrigger.Fired += OnFallTriggerFired;
            }
        }

        public void Dispose() {
            foreach (var fallTrigger in map.FallTriggers) {
                fallTrigger.Fired -= OnFallTriggerFired;
            }
        }

        #region API
        public event Action<Piece> PieceSpawned; 
        public event Action<Piece> PieceTouched;
        public event Action<Piece> PieceFalling;
        public event Action<TowerGame> StateChanged;
        
        public int GetNumLives() {
            return def.NumLives - numFalls;
        }

        public float GetMaxHeight() {
            return maxHeight;
        }

        public int GetNumFalls() {
            return numFalls;
        }

        public IReadOnlyList<Piece> GetPlacedPieces() {
            return placedPieces;
        }

        public Piece GetCurrentPiece() {
            return currentPiece;
        }

        public GameResult? GetResult() {
            return gameResult;
        }

        public GameState GetState() {
            return gameState;
        }

        public void Pause() {
            if (gameState != GameState.Running) {
                throw new InvalidOperationException("Game is not running");
            }
            gameState = GameState.Paused;
        }

        public void Unpause() {
            if (gameState != GameState.Paused) {
                throw new InvalidOperationException("Game is not paused");
            }
            gameState = GameState.Running;
            
        }
        #endregion

        public void Start() {
            if (gameState != GameState.NotStarted) {
                throw new InvalidOperationException("Attempt to start game several times");
            }
            gameState = GameState.Running;
            
            SpawnPiece();
        
            StateChanged?.Invoke(this);
        }

        public void AddCommand(ICommand command) {
            if (command == null) {
                return;
            }
            if (gameState != GameState.Running) {
                return;
            }
            commands.Add(command);
        }

        public void Tick() {
            if (gameState != GameState.Running) {
                return;
            }

            ExecuteCommands();
            Physics2D.Simulate(Time.deltaTime);
            
            UpdateGameState();
        }
        
        private void ExecuteCommands() {
            if (currentPiece != null) {
                foreach (var command in commands) {
                    command.Execute(currentPiece);
                }
            }

            commands.Clear();
        }
        
        private void UpdateGameState() {
            if (maxHeight >= def.RequiredHeight) {
                FinishGame(GameResult.Win);
                return;
            }
            
            if (GetNumLives() <= 0) {
                FinishGame(GameResult.Defeat);
                return;
            }
        }

        private void FinishGame(GameResult result) {
            if (gameState != GameState.Running) {
                throw new InvalidOperationException("Attempt to finish not running game");
            }
            
            gameResult = result;
            gameState = GameState.Finished;
            StateChanged?.Invoke(this);
        }

        private void SpawnPiece() {
            float spawnHeight = maxHeight + def.SpawnHeight;
            var spawnPoint = map.GetPlatformTop() + spawnHeight * Vector3.up;
            int i = Random.Range(0, def.PiecePrefabs.Length);
            var prefab = def.PiecePrefabs[i];
            
            var piece = pieceFactory.Create(prefab, spawnPoint, map.transform);
            piece.Touched += OnCurrentPieceTouched;

            currentPiece = piece;
            PieceSpawned?.Invoke(piece);
        }

        private void OnCurrentPieceTouched(Piece piece, Collision2D col) {
            if (piece != currentPiece) {
                return;
            }
            currentPiece.Touched -= OnCurrentPieceTouched;
            currentPiece.SetState(PieceState.Placed);
            currentPiece = null;
            placedPieces.Add(piece);
            
            UpdateMaxHeight(piece);
            PieceTouched?.Invoke(piece);
            
            SpawnPiece();
        }
        
        private void UpdateMaxHeight(Piece placedPiece) {
            var heightInWorld = map.GetPlatformTop().y + maxHeight;
            foreach (var point in placedPiece.GetPoints()) {
                if (point.y > heightInWorld) {
                    heightInWorld = point.y;
                }
            }
            maxHeight = heightInWorld - map.GetPlatformTop().y;
        }
        
        private void OnFallTriggerFired(PieceTrigger trigger, Piece piece) {
            PieceFalling?.Invoke(piece);
            
            if (piece == currentPiece) {
                currentPiece.Touched -= OnCurrentPieceTouched;
                currentPiece = null;
                SpawnPiece();
            } else {
                placedPieces.Remove(piece);
            }

            numFalls += 1;
            piece.Destroy();
        }
    }
}