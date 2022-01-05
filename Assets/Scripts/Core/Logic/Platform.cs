using UnityEngine;

namespace MiniBricks.Core.Logic {
    public class Platform : MonoBehaviour {
        [SerializeField]
        private Transform top;

        public Vector3 GetTopPoint() {
            return top.position;
        }
    }
}