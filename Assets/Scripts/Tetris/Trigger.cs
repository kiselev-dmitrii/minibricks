using System;
using UnityEngine;

namespace MiniBricks.Tetris {
    public class Trigger : MonoBehaviour {
        public Action<Trigger, Collider2D> Fired;

        private void OnTriggerEnter2D(Collider2D col) {
            Fired?.Invoke(this, col);
        }
    }
}