using DG.Tweening;
using MiniBricks.Core.Logic;
using UnityEngine;

namespace MiniBricks.Core.View {
    [RequireComponent(typeof(Piece), typeof(SpriteRenderer))]
    public class PieceView : MonoBehaviour {
        [SerializeField]
        private Piece piece;
        [SerializeField]
        private SpriteRenderer spriteRenderer;
        
        [SerializeField]
        private float effectDuration = 0.1f;
        [SerializeField]
        private float effectIntensity = 0.5f;

        private Color originalColor;

        public void Awake() {
            originalColor = spriteRenderer.color;
            
            piece.StateChanged += OnStateChanged;
        }

        public void OnDestroy() {
            piece.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(Piece _) {
            if (piece.State != PieceState.Placed) {
                return;
            }
            PlayTouchEffect();
        }

        private void PlayTouchEffect() {
            var targetColor = originalColor + effectIntensity*Color.white;
            
            var sequence = DOTween.Sequence();
            sequence.Append(spriteRenderer.DOColor(targetColor, effectDuration/2));
            sequence.Append(spriteRenderer.DOColor(originalColor, effectDuration / 2));
            sequence.Play();
        }
    }
}