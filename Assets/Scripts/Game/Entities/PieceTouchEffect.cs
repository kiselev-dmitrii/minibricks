using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace MiniBricks.Game.Entities {
    [RequireComponent(typeof(Piece), typeof(SpriteRenderer))]
    public class PieceTouchEffect : MonoBehaviour {
        [SerializeField]
        private float duration = 0.1f;
        [SerializeField]
        private float intensity = 0.5f;

        private Piece piece;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;

        public void Awake() {
            piece = GetComponent<Piece>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            piece.StateChanged += OnStateChanged;
            originalColor = spriteRenderer.color;
        }

        public void OnDestroy() {
            piece.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(Piece _) {
            if (piece.State != PieceState.Placed) {
                return;
            }

            var targetColor = originalColor + intensity*Color.white;
            
            var sequence = DOTween.Sequence();
            sequence.Append(spriteRenderer.DOColor(targetColor, duration/2));
            sequence.Append(spriteRenderer.DOColor(originalColor, duration / 2));
            sequence.Play();
        }
    }
}