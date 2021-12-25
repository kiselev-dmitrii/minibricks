using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace MiniBricks.Tetris {
    
    
    public interface ITowerDef {
        /// <summary>
        /// The distance between the top of the tower and the place where a piece spawns
        /// </summary>
        float SpawnHeight { get; }
    }

    public interface IPieceFactory {
        Piece Create(Vector2 position);
    }
    
    public class Tower : MonoBehaviour {
        [FormerlySerializedAs("fallTrigger")]
        [SerializeField]
        private Trigger[] fallTriggers;
        [SerializeField]
        private Transform platformTop;

        private ITowerDef def;
        private IPieceFactory pieceFactory;
        private Piece currentPiece;
        private float maxTouchPosition;

        public void Initialize(ITowerDef def, IPieceFactory pieceFactory) {
            this.def = def;
            this.pieceFactory = pieceFactory;
        }

        public void Run() {
            foreach (var fallTrigger in fallTriggers) {
                fallTrigger.Fired += OnFallTriggerFired;
            }

            SpawnPiece();
        }

        public void Finish() {
            foreach (var fallTrigger in fallTriggers) {
                fallTrigger.Fired -= OnFallTriggerFired;
            }
        }

        public void Rotate() {
            if (currentPiece == null) {
                return;
            }
            currentPiece.Rotate();
        }

        public void Move(int direction) {
            if (currentPiece == null) {
                return;
            }
            currentPiece.Move(direction);
        }
        
        public float GetHeight() {
            return maxTouchPosition;
        }

        private Vector3 CalculateSpawnPoint() {
            return platformTop.position + Vector3.up * (maxTouchPosition + def.SpawnHeight);
        }

        private void SpawnPiece() {
            var piecePosition = CalculateSpawnPoint();
            currentPiece = pieceFactory.Create(piecePosition);
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
            var maxTouchPositionInWorld = platformTop.position.y + maxTouchPosition;
            foreach (var contact in col.contacts) {
                if (contact.point.y > maxTouchPositionInWorld) {
                    maxTouchPositionInWorld = contact.point.y;
                }
            }
            maxTouchPosition = maxTouchPositionInWorld - platformTop.position.y;
        }

        private void OnFallTriggerFired(Trigger arg1, Collider2D arg2) {
            Debug.LogError("Piece has been fallen");
        }   
    }
    
    


}