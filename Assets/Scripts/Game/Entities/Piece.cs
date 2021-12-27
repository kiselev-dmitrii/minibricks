using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniBricks.Game {
    [Serializable]
    public class PieceDef {
        public float MoveStep;
        public float FallSpeed;
        public float AccelerationSpeed;
    }

    public enum PieceState {
        Descending,
        Placed
    }
    
    public class Piece : MonoBehaviour {
        [SerializeField]
        private Rigidbody2D rb;
        [SerializeField]
        private PolygonCollider2D polygonCollider;

        private PieceDef def;
        private Transform cachedTransform;
        
        public event Action<Piece, Collision2D> Touched;
        public PieceState State { get; private set; }
        public event Action<Piece> StateChanged;
        public bool IsAccelerated { get; private set; }
        
        public void Initialize(PieceDef def) {
            this.def = def;
            
            cachedTransform = transform;
            SetState(PieceState.Descending);
        }

        public void SetState(PieceState state) { 
            if (state == PieceState.Descending) {
                rb.isKinematic = true;
                SetAccelerated(false);
            } else {
                rb.isKinematic = false;
                IsAccelerated = false;
            }

            State = state;
            StateChanged?.Invoke(this);
        }
        
        public void Move(int direction) {
            EnsureIsDescending();
            float sign = Mathf.Sign(direction);
            rb.position += sign * def.MoveStep * Vector2.right;
        }
        
        public void Rotate() {
            EnsureIsDescending();
            rb.rotation += 90;
        }

        public void SetAccelerated(bool isEnabled) {
            EnsureIsDescending();
            var speed = isEnabled ? def.AccelerationSpeed : def.FallSpeed;
            rb.velocity = Vector2.down * speed;
            IsAccelerated = isEnabled;
        }

        public void Destroy() {
            GameObject.Destroy(gameObject);
        }

        public IEnumerable<Vector3> GetPoints() {
            foreach (var point in polygonCollider.points) {
                var localPoint = point + polygonCollider.offset;
                var worldPoint = cachedTransform.TransformPoint(localPoint);
                yield return worldPoint;
            }
        }

        private void OnCollisionEnter2D(Collision2D col) {
            Touched?.Invoke(this, col);
        }

        private void EnsureIsDescending() {
            if (State != PieceState.Descending) {
                throw new InvalidOperationException("The piece is not in descending state");
            }
        }
    }
}