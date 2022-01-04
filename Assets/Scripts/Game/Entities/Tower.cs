using System;
using System.Collections.Generic;
using MiniBricks.Game;
using MiniBricks.Tetris;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniBricks.Controllers {
    [Serializable]
    public class TowerDef {
        public float SpawnHeight;
        public Piece[] Pieces;
        public int NumLives;
    }

    public interface IPieceFactory {
        Piece Create(Piece prefab, Vector2 position, Transform parent);
    }

    public class Tower : MonoBehaviour {
        [SerializeField]
        private PieceTrigger trigger;

        private int towerId;
        private TowerDef def;
        private IPieceFactory pieceFactory;

        private Platform currentPlatform;
        private bool isActive;
        private Piece currentPiece;
        private List<Piece> placedPieces;
        private float maxHeight;
        private int numFalls;

        public event Action<Piece> PieceSpawned; 
        public event Action<Piece> PieceTouched;
        public event Action<Piece> PieceFalling;
        public event Action<Tower> HeightChanged;
        public event Action<Tower> NumLivesChanged;
        
        public void Initialize(int towerId, TowerDef def, IPieceFactory pieceFactory) {
            this.towerId = towerId;
            this.def = def;
            this.pieceFactory = pieceFactory;
            currentPlatform = null;
            isActive = false;
            currentPiece = null;
            placedPieces = new List<Piece>();
            maxHeight = 0;

            trigger.Fired += OnFallTriggerFired;
        }

        private void OnDestroy() {
            trigger.Fired -= OnFallTriggerFired;
        }

        public int GetId() {
            return towerId;
        }
        
        /// <summary>
        /// Changes platform of tower
        /// </summary>
        public void SetPlatform(String platformPath) {
            if (currentPlatform != null) {
                GameObject.Destroy(currentPlatform);
                currentPlatform = null;
            }
            
            var prefab = Resources.Load<Platform>(platformPath);
            currentPlatform = GameObject.Instantiate(prefab, transform);
        }
        
        /// <summary>
        /// Returns approximate top of tower in world space
        /// It is used for camera positioning
        /// </summary>
        public Vector3 CalculateTopPoint() {
            var result = currentPlatform.GetTopPoint();
            var pieces = placedPieces;
            
            if (pieces.Count == 0) {
                return result;
            }

            var maxY = pieces[0].GetPosition().y;
            foreach (var piece in pieces) {
                var piecePos = piece.GetPosition();
                if (piecePos.y > maxY) {
                    maxY = piecePos.y;
                }
            }

            if (maxY > result.y) {
                result.y = maxY;
            }
            
            return result;
        }

        /// <summary>
        /// Returns current piece
        /// </summary>
        public Piece GetCurrentPiece() {
            return currentPiece;
        }

        /// <summary>
        /// Returns number of lives
        /// </summary>
        public int GetNumLives() {
            return def.NumLives - numFalls;
        }

        /// <summary>
        /// Returns number of pieces that fall
        /// </summary>
        public int GetNumFalls() {
            return numFalls;
        }

        /// <summary>
        /// Return maximum height that tower had during gameplay
        /// </summary>
        public int GetMaxHeight() {
            return Mathf.FloorToInt(maxHeight);
        }

        /// <summary>
        /// Returns spawn point for pieces
        /// </summary>
        public Vector3 CalculateSpawnPoint() {
            var topPoint = CalculateTopPoint();
            var spawnPoint = topPoint + def.SpawnHeight * Vector3.up;
            return spawnPoint;
        }
        
        /// <summary>
        /// Runs auto spawner
        /// </summary>
        public void Activate() {
            if (currentPiece == null) {
                SpawnPiece();
            }
            isActive = true;
        }

        /// <summary>
        /// Deactivates auto spawner
        /// </summary>
        public void Deactivate() {
            isActive = false;
        }
        
        private void SpawnPiece() {
            int i = Random.Range(0, def.Pieces.Length);
            var prefab = def.Pieces[i];
            
            var spawnPoint = CalculateSpawnPoint();
            var piece = pieceFactory.Create(prefab, spawnPoint, transform);
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

            if (isActive) {
                SpawnPiece();
            }   
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
            
            NumLivesChanged?.Invoke(this);
        }
        
        private void UpdateMaxHeight(Piece placedPiece) {
            var heightInWorld = currentPlatform.GetTopPoint().y + maxHeight;
            foreach (var point in placedPiece.GetPoints()) {
                if (point.y > heightInWorld) {
                    heightInWorld = point.y;
                }
            }
            var newMaxHeight = heightInWorld - currentPlatform.GetTopPoint().y;
            if (newMaxHeight > maxHeight) {
                maxHeight = newMaxHeight;
                HeightChanged?.Invoke(this);
            }
        }
    }
}