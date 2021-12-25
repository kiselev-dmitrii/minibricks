using System;
using UnityEngine;

namespace MiniBricks.Tetris {
    public interface IPieceDef {
        float MoveStep { get; }
    }
    
    public class Piece : MonoBehaviour {
        [SerializeField]
        private Rigidbody2D rb;

        private IPieceDef pieceDef;
        
        public event Action<Piece, Collision2D> Touched;

        public void Initialize(IPieceDef pieceDef) {
            this.pieceDef = pieceDef;
            rb.velocity = Vector2.down;
        }

        public void Move(int direction) {
            float sign = Mathf.Sign(direction);
            rb.position += sign * Vector2.right * pieceDef.MoveStep;
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