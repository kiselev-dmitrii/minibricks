using UnityEngine;

namespace MiniBricks.Controllers {
    public class Platform : MonoBehaviour {
        [SerializeField]
        private Transform top;

        public Vector3 GetTopPoint() {
            return top.position;
        }
    }
}