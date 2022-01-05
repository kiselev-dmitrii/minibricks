using MiniBricks.Core.Logic;
using UnityEngine;

namespace MiniBricks.Core.View {
    public enum CameraOutput {
        Framebuffer,
        RenderTexture
    }
    
    public class CameraView : MonoBehaviour {
        [SerializeField]
        private Vector3 offset;
        [SerializeField]
        private float factor = 30;
        [SerializeField]
        private new Camera camera;
        
        private Transform cachedTransform;
        private Tower target;
        private RenderTexture rt;

        private void Awake() {
            cachedTransform = transform;
        }
        
        public void SetTarget(Tower tower) {
            target = tower;
            cachedTransform.position = GetTargetPosition();
        }

        private Vector3 GetTargetPosition() {
            return target.CalculateTopPoint() + offset;
        }
        
        private void Update() {
            if (target == null) {
                return;
            }
            
            cachedTransform.position = Vector3.Lerp(cachedTransform.position, GetTargetPosition(), factor * Time.deltaTime);
        }

        public CameraOutput CurrentOutput {
            get {
                if (camera.targetTexture != null) {
                    return CameraOutput.RenderTexture;
                }
                return CameraOutput.Framebuffer;
            }
        }
        
        public void SetFamebufferOutput() {
            camera.targetTexture = null;
            
            if (rt != null) {
                rt.Release();
                rt = null;
            }
        }

        public void SetRenderTextureOutput(int width, int height) {
            if (rt != null) {
                rt.Release();
                rt = null;
            }
            
            rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            rt.Create();
            camera.targetTexture = rt;
        }

        public float GetPixelsPerUnit() {
            var heightInPixels = Screen.height;
            var heightInUnits = camera.orthographicSize * 2;
            return heightInPixels / heightInUnits;
        }
        
        public RenderTexture GetRenderTexture() {
            return rt;
        }

        public void Destroy() {
            GameObject.Destroy(gameObject);
        }
        
        private void OnDestroy() {
            if (rt != null) {
                rt.Release();
                rt = null;
            }
        }
    }
}