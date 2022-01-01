using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MiniBricks.Game;
using MiniBricks.Game.CommandProviders;
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
        public int RequiredHeight;

    }
    
    public interface ICommand {
        void Execute(Piece piece);
    }

    public class GameData {
        public Piece CurrentPiece { get; set; }
        public List<Piece> SpawnedPieces { get; }
        public float Height { get; set; }
        public int NumFalls { get; set; }

        public GameData() {
            CurrentPiece = null;
            SpawnedPieces = new List<Piece>();
            Height = 0;
            NumFalls = 0;
        }
    }

    public interface IFeature {
        void GameStarted(); 
        void Tick();
        void GameFinished(GameResult result);
    }

    public enum GameResult {
        Win,
        Defeat,
        Abort
    }

    public enum GameState {
        NotStarted,
        Running,
        Finished
    }
    
    public class GameSimulation : IDisposable {
        private readonly List<IFeature> features;

        public GameData Data { get; }
        public GameState State { get; private set; }
        public GameResult? Result { get; private set; }

        public GameSimulation() {
            Data = new GameData();
            features = new List<IFeature>();
            State = GameState.NotStarted;
            Result = null;
        }

        public IFeature AddFeature(IFeature feature) {
            features.Add(feature);
            return feature;
        }

        [CanBeNull]
        public T GetFeature<T>() where T : IFeature {
            return (T)features.FirstOrDefault(x => x is T);
        }
        
        public void Start() {
            State = GameState.Running;
            foreach (var system in features) {
                system.GameStarted();
            }
        }
        
        public void Tick() {
            if (State != GameState.Running) {
                return;
            }
            foreach (var system in features) {
                system.Tick();
            }
        }

        public void Finish(GameResult result) {
            State = GameState.Finished;
            Result = result;
            foreach (var system in features) {
                system.GameFinished(result);
            }
        }

        public void Dispose() {
        }
    }

    public class TetrisFeature : IFeature {
        private readonly GameSimulation gameSimulation;
        private readonly Map map;
        private readonly TowerGameDef def;
        private readonly PieceFactory pieceFactory;

        private GameData gameData;

        public event Action<Piece> PieceSpawned; 
        public event Action<Piece> PieceTouched;
        public event Action<Piece> PieceFalling;

        public TetrisFeature(GameSimulation gameSimulation, Map map, TowerGameDef def, PieceFactory pieceFactory) {
            this.gameSimulation = gameSimulation;
            this.map = map;
            this.def = def;
            this.pieceFactory = pieceFactory;
        }
        
        public void GameStarted() {
            gameData = gameSimulation.Data;
            SpawnPiece();
            
            foreach (var fallTrigger in map.FallTriggers) {
                fallTrigger.Fired += OnFallTriggerFired;
            }
        }
        
        public void Tick() {
        }
        
        public void GameFinished(GameResult result) {
            foreach (var fallTrigger in map.FallTriggers) {
                fallTrigger.Fired += OnFallTriggerFired;
            }
        }
        
        private void SpawnPiece() {
            var spawnPoint = map.PlatformTop.position + Vector3.up * (gameData.Height + def.SpawnHeight);
            int i = Random.Range(0, def.PiecePrefabs.Length);
            var prefab = def.PiecePrefabs[i];
            
            var piece = pieceFactory.Create(prefab, spawnPoint, map.transform);
            piece.Touched += OnCurrentPieceTouched;

            gameData.CurrentPiece = piece;
            gameData.SpawnedPieces.Add(piece);
            PieceSpawned?.Invoke(piece);
        }

        private void OnCurrentPieceTouched(Piece piece, Collision2D col) {
            if (piece != gameData.CurrentPiece) {
                return;
            }
            gameData.CurrentPiece.Touched -= OnCurrentPieceTouched;
            gameData.CurrentPiece.SetState(PieceState.Placed);
            gameData.CurrentPiece = null;
            
            UpdateHeight(piece);
            PieceTouched?.Invoke(piece);
            
            SpawnPiece();
        }

        private void UpdateHeight(Piece placedPiece) {
            var heightInWorld = map.PlatformTop.position.y + gameData.Height;
            foreach (var point in placedPiece.GetPoints()) {
                if (point.y > heightInWorld) {
                    heightInWorld = point.y;
                }
            }
            gameData.Height = heightInWorld - map.PlatformTop.position.y;
        }

        private void OnFallTriggerFired(PieceTrigger trigger, Piece piece) {
            PieceFalling?.Invoke(piece);
            
            if (piece == gameData.CurrentPiece) {
                gameData.CurrentPiece.Touched -= OnCurrentPieceTouched;
                gameData.CurrentPiece = null;
                SpawnPiece();
            }
            
            gameData.NumFalls += 1;

            gameData.SpawnedPieces.Remove(piece);
            piece.Destroy();
        }
    }

    public class PhysicsFeature : IFeature {
        public void GameStarted() {
            Physics.autoSimulation = false;
        }

        public void Tick() {
            Physics.Simulate(Time.deltaTime);
        }

        public void GameFinished(GameResult result) {
        }
    }

    public class InputFeature : IFeature {
        private readonly GameSimulation game;
        private readonly ICommandProvider commandProvider;
        private GameData data;

        public InputFeature(GameSimulation game, ICommandProvider commandProvider) {
            this.game = game;
            this.commandProvider = commandProvider;
        }
        
        public void GameStarted() {
            data = game.Data;
        }

        public void Tick() {
            var command = commandProvider.GetNextCommand();
            if (command == null) {
                return;
            }
            if (data.CurrentPiece == null) {
                return;
            }
            
            command.Execute(data.CurrentPiece);
        }

        public void GameFinished(GameResult result) {
        }
    }

    public class EndGameFeature : IFeature {
        private readonly GameSimulation game;
        private readonly TowerGameDef def;

        private GameData data;

        public EndGameFeature(GameSimulation game, TowerGameDef def) {
            this.game = game;
            this.def = def;
        }
        
        public void GameStarted() {
            data = game.Data;
        }

        public int GetLives() {
            return def.NumLives - data.NumFalls;
        }

        public int GetHeight() {
            return Mathf.CeilToInt(data.Height);
        }
        
        public void Tick() {
            if (GetLives() <= 0) {
                game.Finish(GameResult.Defeat);
                return;
            }

            if (GetHeight() >= def.RequiredHeight) {
                game.Finish(GameResult.Win);
                return;
            }
        }

        public void GameFinished(GameResult result) {
        }
    }
}