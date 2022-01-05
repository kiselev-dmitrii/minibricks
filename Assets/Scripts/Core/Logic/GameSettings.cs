using System;

namespace MiniBricks.Core.Logic {
    [Serializable]
    public class GameSettings {
        public float SpawnHeight;
        public float MoveStep;
        public float BaseFallSpeed;
        public float AcceleratedFallSpeed;
        public float DistanceBetweenTowers;
        public int BaseRequiredHeight;
        public int NumLives;
        
        public Piece[] PiecePrefabs;
        public Tower TowerPrefab;
    }
}