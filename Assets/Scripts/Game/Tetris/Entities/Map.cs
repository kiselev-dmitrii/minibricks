using UnityEngine;

namespace MiniBricks.Tetris {
    public class Map : MonoBehaviour {
        public PieceTrigger[] FallTriggers;
        public Transform PlatformTop;
        public Camera Camera;
    }
}