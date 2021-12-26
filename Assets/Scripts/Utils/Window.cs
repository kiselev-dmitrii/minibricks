using System;
using NData;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiniBricks.Utils {
    public class Window : Context {
        public String Path { get; }
        private readonly GameObject gameObject;

        public Window(String prefabPath) {
            Path = prefabPath;
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            gameObject = Object.Instantiate(prefab, null);
            gameObject.SetActive(false);

            var dataContext = gameObject.AddComponent<ItemDataContext>();
            dataContext.SetContext(this);
        }

        public bool IsActive() {
            return gameObject.activeSelf;
        }

        public void SetActive(bool isActive) {
            gameObject.SetActive(isActive);
        }

        public void Activate() {
            gameObject.SetActive(true);
        }

        public void Deactivate() {
            gameObject.SetActive(false);
        }

        public void Destroy() {
            Object.Destroy(gameObject);
        }
    }
}