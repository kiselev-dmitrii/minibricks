using System;
using System.Collections.Generic;
using MiniBricks.Controllers;
using UnityEngine;

namespace MiniBricks.Game.Entities {
    [Serializable]
    public class MultiplayerGameDef {
        public TowerDef Tower;
        public float DistanceBetweenTowers;
        public int RequiredHeight;

    }
    
    public enum GameState {
        NotStarted,
        Running,
        Paused,
        Finished
    }
    
    public enum GameResult {
        Victory,
        Defeat
    }
    
    public class TowerResult {
        public Tower Tower;
        public GameResult Result;
    }
    
    public interface ICommand {
        int TowerId { get; }
        void Execute(Tower tower);
    }
    
    public class MultiplayerGame : IDisposable {        
        private readonly MultiplayerGameDef def;
        private readonly IPieceFactory pieceFactory;
        private readonly List<ICommand> commands;
        private readonly List<Tower> towers;
        private readonly List<Tower> activeTowers;
        private readonly List<TowerResult> towerResults;
        private readonly GameObject gameObject;
        
        private GameState gameState;
        
        public event Action<ICommand> CommandExecuted;
        public event Action<Tower, GameResult> TowerDeactivated;

        public MultiplayerGame(MultiplayerGameDef def, IPieceFactory pieceFactory) {
            this.def = def;
            this.pieceFactory = pieceFactory;
            gameState = GameState.NotStarted;
            commands = new List<ICommand>();
            
            towers = new List<Tower>();
            activeTowers = new List<Tower>();
            towerResults = new List<TowerResult>();
            gameObject = new GameObject("Game");
        }

        public void Dispose() {
            GameObject.Destroy(gameObject);
        }

        public Transform Transform => gameObject.transform;
        
        /// <summary>
        /// Creates and adds new player to the game
        /// </summary>
        public Tower CreateTower() {
            var towerPrefab = Resources.Load<Tower>("Entities/Tower");
            var towerPosition = Vector3.right * def.DistanceBetweenTowers * towers.Count;
            var tower = GameObject.Instantiate(towerPrefab, towerPosition, Quaternion.identity, gameObject.transform);
            int towerId = towers.Count + 1;
            tower.Initialize(towerId, def.Tower, pieceFactory);
            towers.Add(tower);
            return tower;
        }

        /// <summary>
        /// Returns tower by its id
        /// </summary>
        public Tower FindTower(int towerId) {
            return towers[towerId - 1];
        }

        /// <summary>
        /// Starts the game
        /// </summary>
        public void Start() {
            if (gameState != GameState.NotStarted) {
                throw new InvalidOperationException("Attempt to start game several times");
            }
            gameState = GameState.Running;

            foreach (var tower in towers) {
                tower.Activate();
                activeTowers.Add(tower);
            }
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
            UpdateTowerStates();
            UpdateGameState();
        }

        public IReadOnlyList<TowerResult> GetTowerResults() {
            return towerResults;
        }

        public int GetTargetHeight() {
            return def.RequiredHeight;
        }
        
        private void UpdateTowerStates() {
            for (var i = 0; i < activeTowers.Count; i++) {
                var tower = activeTowers[i];
                if (tower.GetNumLives() <= 0) {
                    DeactivateTower(i, GameResult.Defeat);
                    --i;
                    continue;
                }

                if (tower.GetMaxHeight() >= def.RequiredHeight) {
                    DeactivateTower(i, GameResult.Victory);
                    --i;
                    continue;
                }
            }
        }

        private void UpdateGameState() {
            if (activeTowers.Count == 0) {
                gameState = GameState.Finished;
            }
        }
        
        private void DeactivateTower(int towerIdx, GameResult result) {
            var tower = towers[towerIdx];
            activeTowers.RemoveAt(towerIdx);
            tower.Deactivate();
            
            towerResults.Add(new TowerResult() {
                Tower = tower,
                Result = result
            });
            TowerDeactivated?.Invoke(tower, result);
        }
        
        private void ExecuteCommands() {
            foreach (var command in commands) {
                var tower = towers[command.TowerId - 1];
                command.Execute(tower);
                CommandExecuted?.Invoke(command);
            }
            
            commands.Clear();
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
    }
}