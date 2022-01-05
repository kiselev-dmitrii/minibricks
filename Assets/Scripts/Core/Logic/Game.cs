using System;
using System.Collections.Generic;
using MiniBricks.Core.Logic.Interfaces;
using UnityEngine;

namespace MiniBricks.Core.Logic {
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

    public class Game : IDisposable {
        private readonly ITowerFactory towerFactory;
        private readonly List<ICommand> commands;
        private readonly List<Tower> towers;
        private readonly List<Tower> activeTowers;
        private readonly List<TowerResult> towerResults;
        private readonly GameObject gameObject;
        
        private GameState gameState;
        private int targetHeight;
        
        public event Action<ICommand> CommandExecuted;
        public event Action<Tower, GameResult> TowerDeactivated;
        public event Action TargetHeightChanged;
        public GameSettings Settings { get; }
        
        public Game(GameSettings settings, ITowerFactory towerFactory) {
            this.towerFactory = towerFactory;
            commands = new List<ICommand>();
            
            towers = new List<Tower>();
            activeTowers = new List<Tower>();
            towerResults = new List<TowerResult>();
            gameObject = new GameObject("Game");
            
            gameState = GameState.NotStarted;
            targetHeight = settings.BaseRequiredHeight;
            
            Settings = settings;
        }

        public void Dispose() {
            GameObject.Destroy(gameObject);
        }

        public Transform Transform => gameObject.transform;
        
        /// <summary>
        /// Creates and adds new player to the game
        /// </summary>
        public Tower CreateTower() {
            var towerPosition = Vector3.right * Settings.DistanceBetweenTowers * towers.Count;
            int towerId = towers.Count + 1;
            var tower = towerFactory.Create(towerId, towerPosition, gameObject.transform);
            towers.Add(tower);
            return tower;
        }

        /// <summary>
        /// Returns tower by its id
        /// </summary>
        public Tower GetTower(int towerId) {
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
            return targetHeight;
        }

        public void SetTargetHeight(int value) {
            targetHeight = value;
            TargetHeightChanged?.Invoke();
        }
        
        private void UpdateTowerStates() {
            for (var i = 0; i < activeTowers.Count; i++) {
                var tower = activeTowers[i];
                if (tower.GetNumLives() <= 0) {
                    DeactivateTower(i, GameResult.Defeat);
                    --i;
                    continue;
                }

                if (tower.GetMaxHeight() >= targetHeight) {
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
                command.Execute(this);
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