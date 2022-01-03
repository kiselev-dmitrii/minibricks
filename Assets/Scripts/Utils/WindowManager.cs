using System;
using NData;
using UnityEngine;

namespace MiniBricks.Utils {
    public class WindowManager : MonoBehaviour {
        [SerializeField]
        private Transform parent;
        [SerializeField]
        private Camera uiCamera;

        public static WindowManager Current { get; private set; } 
        
        public static WindowManager Create(String path) {
            var prefab = Resources.Load<WindowManager>(path);
            var result = GameObject.Instantiate(prefab);
            return result;
        }

        public void SetCurrent() {
            Current = this;
        }
        
        public GameObject CreateView(Window window) {
            GameObject prefab = Resources.Load<GameObject>(window.Path);
            var view = GameObject.Instantiate(prefab, parent);
            view.SetActive(false);

            var dataContext = view.AddComponent<ItemDataContext>();
            dataContext.SetContext(window);

            var canvas = view.GetComponent<Canvas>();
            canvas.worldCamera = uiCamera;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;

            return view;
        }

        public void DestroyView(Window window) {
            GameObject.Destroy(window.View);
        }
    }

    public abstract class Window : Context {
        public String Path { get; }
        public GameObject View { get; private set; }
        
        protected WindowManager WindowManager { get; }

        protected Window(String path) {
            WindowManager = WindowManager.Current;
            Path = path;
            View = WindowManager.CreateView(this);
        }
        
        public void Destroy() {
            if (IsActive()) {
                OnDeactivated();
            }
            WindowManager.DestroyView(this);
            View = null;
        }
        
        public bool IsActive() {
            return View.activeSelf;
        }

        public void SetActive(bool value) {
            if (value == IsActive()) {
                return;
            }
            
            View.SetActive(value);
            
            if (value) {
                OnActivated();
            } else {
                OnDeactivated();
            }
        }

        public void Activate() {
            SetActive(true);
        }

        public void Deactivate() {
            SetActive(false);
        }
        
        protected virtual void OnActivated() {}
        protected virtual void OnDeactivated() {}
        protected virtual void OnDestroy() {}
    }
}