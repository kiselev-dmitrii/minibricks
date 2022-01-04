using UnityEngine;

namespace MiniBricks.Tetris {
    public class CameraBackgroundAligner : MonoBehaviour {
        [SerializeField]
        private Transform background;
        [SerializeField]
        private new Camera camera;
        
        public void OnEnable() {
            var height = camera.orthographicSize;
            float width = camera.aspect * height;
            background.localScale = new Vector3(2*width, 2*height, 1);
        }
    }
}