using UnityEngine;

namespace MiniBricks.Tetris {
    public class FollowCamera : MonoBehaviour {
        [SerializeField]
        private Vector3 offset;
        [SerializeField]
        private float factor = 30;

        private TetrisFeature tetrisFeature;
        private Transform cachedTransform;
        
        public void Initialize(TetrisFeature tetrisFeature) {
            this.tetrisFeature = tetrisFeature;
            cachedTransform = transform;
        }

        public void Update() {
            var targetPosition = CalculateTowerTop() + offset;
            cachedTransform.position = Vector3.Lerp(cachedTransform.position, targetPosition, factor * Time.deltaTime);
        }

        private Vector3 CalculateTowerTop() {
            var result = tetrisFeature.GetPlatformTop();
            var pieces = tetrisFeature.PlacedPieces;
            
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
    }
}