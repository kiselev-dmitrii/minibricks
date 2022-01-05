using MiniBricks.Core.Logic.Interfaces;
using UnityEngine;

namespace MiniBricks.Core.Logic.Factories {
    public class TowerFactory : ITowerFactory {
        private readonly GameSettings settings;
        private readonly IPieceFactory pieceFactory;

        public TowerFactory(GameSettings settings, IPieceFactory pieceFactory) {
            this.settings = settings;
            this.pieceFactory = pieceFactory;
        }
        
        public Tower Create(int towerId, Vector3 position, Transform parent) {
            var tower = GameObject.Instantiate(settings.TowerPrefab, position, Quaternion.identity, parent);
            tower.Initialize(towerId, settings, pieceFactory);
            return tower;
        }
    }
}