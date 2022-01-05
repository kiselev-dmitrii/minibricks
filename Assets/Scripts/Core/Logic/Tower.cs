using System;
using System.Collections.Generic;
using MiniBricks.Core.Logic.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniBricks.Core.Logic {
    public class Tower : MonoBehaviour {
        [SerializeField]
        private PieceTrigger trigger;

        private int towerId;
        private IPieceFactory pieceFactory;

        private Platform currentPlatform;
        private bool isActive;
        private Piece currentPiece;
        private List<Piece> placedPieces;
        private float maxHeight;
        private int numFalls;
        private int numLives;
        private Piece[] spawningPieces;
        
        public void Initialize(int towerId, GameSettings settings, IPieceFactory pieceFactory) {
            this.towerId = towerId;
            this.pieceFactory = pieceFactory;
            currentPlatform = null;
            isActive = false;
            currentPiece = null;
            placedPieces = new List<Piece>();
            maxHeight = 0;
            SpawnHeight = settings.SpawnHeight;
            spawningPieces = settings.PiecePrefabs;
            NumLives = settings.NumLives;
            
            trigger.Fired += OnFallTriggerFired;
        }

        private void OnDestroy() {
            trigger.Fired -= OnFallTriggerFired;
        }

        public event Action<Piece> PieceSpawned; 
        public event Action<Piece> PieceTouched;
        public event Action<Piece> PieceFalling;
        
        public int Id => towerId;
                
        public float SpawnHeight { get; set; }

        public event Action<Tower> NumLivesChanged;
        public int NumLives {
            get => numLives;
            set {
                numLives = value;
                NumLivesChanged?.Invoke(this);
            }
        }
        
        public event Action<Tower> MaxHeightChanged;
        /// <summary>
        /// Return maximum height that tower had during gameplay
        /// </summary>
        public float MaxHeight => maxHeight;

        /// <summary>
        /// Changes platform of tower
        /// </summary>
        public void SetPlatform(Platform platformPrefab) {
            if (currentPlatform != null) {
                GameObject.Destroy(currentPlatform);
                currentPlatform = null;
            }
            
            currentPlatform = GameObject.Instantiate(platformPrefab, transform);
        }

        public Platform GetPlatform() {
            return currentPlatform;
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

        public Vector3 GetBottomPoint() {
            return currentPlatform.GetTopPoint();
        }

        /// <summary>
        /// Returns current piece
        /// </summary>
        public Piece GetCurrentPiece() {
            return currentPiece;
        }

        /// <summary>
        /// Returns number of pieces that fall
        /// </summary>
        public int NumFalls => numFalls;

        /// <summary>
        /// Returns spawn point for pieces
        /// </summary>
        public Vector3 CalculateSpawnPoint() {
            var topPoint = CalculateTopPoint();
            var spawnPoint = topPoint + SpawnHeight * Vector3.up;
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
            int i = Random.Range(0, spawningPieces.Length);
            var prefab = spawningPieces[i];
            
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

            piece.Destroy();

            numFalls += 1;
            if (NumLives > 0) {
                NumLives -= 1;
            }
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
                MaxHeightChanged?.Invoke(this);
            }
        }
    }
}