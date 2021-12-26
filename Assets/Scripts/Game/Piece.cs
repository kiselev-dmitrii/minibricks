using System;
using UnityEngine;

namespace MiniBricks.Game {
    [Serializable]
    public class PieceDef {
        public float MoveStep;
        public float FallSpeed;
    }
    
    public class Piece : MonoBehaviour {
        [SerializeField]
        private Rigidbody2D rb;

        private PieceDef def;

        public event Action<Piece, Collision2D> Touched;

        public void Initialize(PieceDef def) {
            this.def = def;
            rb.isKinematic = true;
            rb.velocity = Vector2.down * def.FallSpeed;
        }

        public void Move(int direction) {
            float sign = Mathf.Sign(direction);
            rb.position += sign * Vector2.right * def.MoveStep;
        }
        
        public void Rotate() {
            rb.rotation += 90;
        }

        public void Release() {
            rb.isKinematic = false;
        }

        private void OnCollisionEnter2D(Collision2D col) {
            Touched?.Invoke(this, col);
        }
    }
}