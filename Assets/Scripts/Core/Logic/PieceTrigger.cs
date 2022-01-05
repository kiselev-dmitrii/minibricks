using System;
using UnityEngine;

namespace MiniBricks.Core.Logic {
    public class PieceTrigger : MonoBehaviour {
        public Action<PieceTrigger, Piece> Fired;

        private void OnTriggerEnter2D(Collider2D col) {
            var piece = col.gameObject.GetComponent<Piece>();
            if (piece == null) {
                return;
            }
            Fired?.Invoke(this, piece);
        }
    }
}