using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniBricks.Core.Logic {
    public enum PieceState {
        Descending,
        Placed
    }
    
    public class Piece : MonoBehaviour {
        [SerializeField]
        private Rigidbody2D rb;
        [SerializeField]
        private PolygonCollider2D polygonCollider;

        private float moveStep;
        private float speed;
        private Transform cachedTransform;
        
        public event Action<Piece, Collision2D> Touched;
        public event Action<Piece> StateChanged;
        public PieceState State { get; private set; }
        
        public void Initialize(float moveStep, float speed) {
            this.moveStep = moveStep;
            this.speed = speed;
            cachedTransform = transform;
            SetState(PieceState.Descending);
            SetSpeed(speed);
        }

        public void SetState(PieceState state) { 
            rb.isKinematic = state == PieceState.Descending;
            State = state;
            StateChanged?.Invoke(this);
        }
        
        public void Move(int direction) {
            EnsureIsDescending();
            float sign = Mathf.Sign(direction);
            rb.position += sign * moveStep * Vector2.right;
        }
        
        public void Rotate() {
            EnsureIsDescending();
            rb.rotation -= 90;
        }

        public void SetSpeed(float value) {
            EnsureIsDescending();
            speed = value;
            rb.velocity = Vector2.down * speed;
        }
        
        public Vector3 GetPosition() {
            return rb.position;
        }

        public Vector2 GetSize() {
            return polygonCollider.bounds.size;
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