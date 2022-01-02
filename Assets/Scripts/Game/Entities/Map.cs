using UnityEngine;

namespace MiniBricks.Tetris {
    public class Map : MonoBehaviour {
        public PieceTrigger[] FallTriggers;
        public Transform PlatformTop;
        public Camera Camera;

        public Vector3 GetPlatformTop() {
            return PlatformTop.position;
        }
    }
}