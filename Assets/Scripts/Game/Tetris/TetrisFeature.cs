using System;
using MiniBricks.Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniBricks.Tetris {
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
            Physics2D.simulationMode = SimulationMode2D.Script;
            gameData = gameSimulation.Data;
            SpawnPiece();
            
            foreach (var fallTrigger in map.FallTriggers) {
                fallTrigger.Fired += OnFallTriggerFired;
            }
        }
        
        public void Tick() {
            Physics2D.Simulate(Time.deltaTime);
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
}