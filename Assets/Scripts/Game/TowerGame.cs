using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MiniBricks.Game;
using UnityEngine;

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
        public bool IsPaused { get; set; }

        public GameSimulation() {
            Data = new GameData();
            features = new List<IFeature>();
            State = GameState.NotStarted;
            Result = null;
            IsPaused = false;
        }

        public void Dispose() {
        }
        
        public IFeature AddFeature(IFeature feature) {
            features.Add(feature);
            return feature;
        }

        public void AddFeatures(IEnumerable<IFeature> collection) {
            features.AddRange(collection);
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
            if (State != GameState.Running || IsPaused) {
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