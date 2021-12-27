using System.Collections;
using UnityEngine;

namespace MiniBricks.Game.Entities {
    public class PieceTouchEffect : MonoBehaviour {
        [SerializeField]
        private Piece piece;
        [SerializeField]
        private SpriteRenderer sr;

        private Color originalColor;
        
        public void Awake() {
            piece.StateChanged += OnStateChanged;
            originalColor = sr.color;
        }

        public void OnDestroy() {
            piece.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(Piece _) {
            if (piece.State == PieceState.Placed) {
                StartCoroutine(PlayEffect());
            }
        }

        private IEnumerator PlayEffect() {
            sr.color = originalColor + Color.white * 0.1f;
            yield return new WaitForSeconds(0.3f);
            sr.color = originalColor;
        }
    }
}