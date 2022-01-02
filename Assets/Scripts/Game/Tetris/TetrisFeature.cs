using System;
using System.Collections.Generic;
using MiniBricks.Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniBricks.Tetris {
    public class TetrisFeature : IFeature {
        private readonly GameSimulation gameSimulation;
        private readonly Map map;
        private readonly TowerGameDef def;
        private readonly PieceFactory pieceFactory;

        public Piece CurrentPiece { get; private set; }
        public List<Piece> PlacedPieces { get; private set; }
        public float Height { get; private set; }
        public int NumFalls { get; private set; }
        
        public event Action<Piece> PieceSpawned; 
        public event Action<Piece> PieceTouched;
        public event Action<Piece> PieceFalling;

        public TetrisFeature(GameSimulation gameSimulation, Map map, TowerGameDef def, PieceFactory pieceFactory) {
            this.gameSimulation = gameSimulation;
            this.map = map;
            this.def = def;
            this.pieceFactory = pieceFactory;
            
            CurrentPiece = null;
            PlacedPieces = new List<Piece>();
            Height = 0;
            NumFalls = 0;
        }

        public Vector3 GetPlatformTop() {
            return map.PlatformTop.position;
        }

        public void GameStarted() {            
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
            var spawnPoint = map.PlatformTop.position + Vector3.up * (Height + def.SpawnHeight);
            int i = Random.Range(0, def.PiecePrefabs.Length);
            var prefab = def.PiecePrefabs[i];
            
            var piece = pieceFactory.Create(prefab, spawnPoint, map.transform);
            piece.Touched += OnCurrentPieceTouched;

            CurrentPiece = piece;
            PieceSpawned?.Invoke(piece);
        }

        private void OnCurrentPieceTouched(Piece piece, Collision2D col) {
            if (piece != CurrentPiece) {
                return;
            }
            CurrentPiece.Touched -= OnCurrentPieceTouched;
            CurrentPiece.SetState(PieceState.Placed);
            CurrentPiece = null;
            PlacedPieces.Add(piece);
            
            UpdateHeight(piece);
            PieceTouched?.Invoke(piece);
            
            SpawnPiece();
        }

        private void UpdateHeight(Piece placedPiece) {
            var heightInWorld = map.PlatformTop.position.y + Height;
            foreach (var point in placedPiece.GetPoints()) {
                if (point.y > heightInWorld) {
                    heightInWorld = point.y;
                }
            }
            Height = heightInWorld - map.PlatformTop.position.y;
        }

        private void OnFallTriggerFired(PieceTrigger trigger, Piece piece) {
            PieceFalling?.Invoke(piece);
            
            if (piece == CurrentPiece) {
                CurrentPiece.Touched -= OnCurrentPieceTouched;
                CurrentPiece = null;
                SpawnPiece();
            } else {
                PlacedPieces.Remove(piece);
            }

            NumFalls += 1;
            piece.Destroy();
        }
    }
}