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
    }
    
    public interface IPieceFactory {
        Piece Create(Piece prefab, Vector2 position, Transform parent);
    }

    public interface ICommand {
        void Execute(Piece piece);
    }

    public class TowerGame : IDisposable {
        private readonly TowerGameDef def;
        private readonly Map map;
        private readonly IPieceFactory pieceFactory;
        private Piece currentPiece;
        
        public float MaxHeight { get; private set; }
        public int NumFalls { get; private set; }

        public TowerGame(TowerGameDef def, Map map, IPieceFactory pieceFactory) {
            this.def = def;
            this.map = map;
            this.pieceFactory = pieceFactory;

            MaxHeight = 0;
            NumFalls = 0;
            
            foreach (var fallTrigger in map.FallTriggers) {
                fallTrigger.Fired += OnFallTriggerFired;
            }
        }

        public void Dispose() {
            foreach (var fallTrigger in map.FallTriggers) {
                fallTrigger.Fired -= OnFallTriggerFired;
            }
        }
        
        public void Start() {
            Physics.autoSimulation = false;
            SpawnPiece();
        }

        public void Tick() {
            Physics.Simulate(Time.deltaTime);
        }

        public void AddCommand(ICommand command) {
            if (currentPiece == null) {
                return;
            }
            command.Execute(currentPiece);
        }

        private void SpawnPiece() {
            var spawnPoint = map.PlatformTop.position + Vector3.up * (MaxHeight + def.SpawnHeight);
            int i = Random.Range(0, def.PiecePrefabs.Length);
            var prefab = def.PiecePrefabs[i];
            
            currentPiece = pieceFactory.Create(prefab, spawnPoint, map.transform);
            currentPiece.Touched += OnCurrentPieceTouched;
        }
        
        private void OnCurrentPieceTouched(Piece piece, Collision2D col) {
            if (piece != currentPiece) {
                throw new Exception("Something went wrong");
            }
            currentPiece.Touched -= OnCurrentPieceTouched;
            currentPiece.SetState(PieceState.Placed);
            currentPiece = null;
            
            UpdateHeight(piece);
            SpawnPiece();
        }

        private void UpdateHeight(Piece placedPiece) {
            var heightInWorld = map.PlatformTop.position.y + MaxHeight;
            foreach (var point in placedPiece.GetPoints()) {
                if (point.y > heightInWorld) {
                    heightInWorld = point.y;
                }
            }
            MaxHeight = heightInWorld - map.PlatformTop.position.y;
        }

        private void OnFallTriggerFired(PieceTrigger trigger, Piece piece) {
            NumFalls += 1;
            piece.Destroy();
        }   
    }
}