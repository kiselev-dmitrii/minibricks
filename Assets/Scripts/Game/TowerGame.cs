using System;
using MiniBricks.Game;
using MiniBricks.Game.Commands;
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

    public class TowerGame : IDisposable {
        private readonly TowerGameDef def;
        private readonly Map map;
        private readonly IPieceFactory pieceFactory;
        private Piece currentPiece;
        private float maxTouchPosition;

        public TowerGame(TowerGameDef def, Map map, IPieceFactory pieceFactory) {
            this.def = def;
            this.map = map;
            this.pieceFactory = pieceFactory;
            
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
            SpawnPiece();
        }

        public void ProcessCommand(CommandType commandType) {
            switch (commandType) {
                case CommandType.Left:
                    currentPiece.Move(-1);
                    break;
                case CommandType.Right:
                    currentPiece.Move(1);
                    break;
                case CommandType.Rotate:
                    currentPiece.Rotate();
                    break;
                case CommandType.StartAccelerate:
                    break;
                case CommandType.StopAccelerate:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
            }
        }

        public void Finish() {
        }
        
        public float GetHeight() {
            return maxTouchPosition;
        }
        
        private void SpawnPiece() {
            var spawnPoint = map.PlatformTop.position + Vector3.up * (maxTouchPosition + def.SpawnHeight);
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
            currentPiece.Release();
            currentPiece = null;
            
            UpdateMaxTouchPosition(col);
            SpawnPiece();
        }

        private void UpdateMaxTouchPosition(Collision2D col) {
            var maxTouchPositionInWorld = map.PlatformTop.position.y + maxTouchPosition;
            foreach (var contact in col.contacts) {
                if (contact.point.y > maxTouchPositionInWorld) {
                    maxTouchPositionInWorld = contact.point.y;
                }
            }
            maxTouchPosition = maxTouchPositionInWorld - map.PlatformTop.position.y;
        }

        private void OnFallTriggerFired(Trigger arg1, Collider2D arg2) {
            Debug.LogError("Piece has been fallen");
        }   
    }
}