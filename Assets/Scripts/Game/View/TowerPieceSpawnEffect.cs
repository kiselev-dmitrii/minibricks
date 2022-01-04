using MiniBricks.Controllers;
using UnityEngine;

namespace MiniBricks.Game.View {
    public class TowerPieceSpawnEffect : MonoBehaviour {
        [SerializeField]
        private GameObject effectPrefab;
        [SerializeField]
        private Tower tower;
        
        public void Awake() {
            tower.PieceSpawned += OnPieceSpawned;
        }

        public void OnDestroy() {
            tower.PieceSpawned -= OnPieceSpawned;
        }
        
        private void OnPieceSpawned(Piece piece) {
            GameObject.Instantiate(effectPrefab, piece.GetPosition(), Quaternion.identity, transform);
        }
    }
}