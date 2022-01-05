using MiniBricks.Core.Logic.Interfaces;
using UnityEngine;

namespace MiniBricks.Core.Logic.Factories {
    public class PieceFactory : IPieceFactory {
        private readonly GameSettings settings;

        public PieceFactory(GameSettings settings) {
            this.settings = settings;
        }
            
        public Piece Create(Piece prefab, Vector2 position, Transform parent) {
            var result = GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
            result.Initialize(settings.MoveStep, settings.BaseFallSpeed);
            return result;
        }
    }
}