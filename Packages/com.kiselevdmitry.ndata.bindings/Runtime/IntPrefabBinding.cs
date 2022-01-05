using System;
using System.Linq;
using KiselevDmitry.Utils;
using NData;
using UnityEngine;

namespace KiselevDmitry.NData.Bindings {
    public class IntPrefabBinding : Binding {
        [Serializable]
        private class IntPrefabPair {
            public int Value;
            public GameObject Prefab;
        }

        [SerializeField]
        private IntPrefabPair[] pairs;
        [SerializeField]
        private GameObject defaultPrefab;
        
        private Property<int> property;

        protected override void Bind() {
            var context = GetContext(Path);
            if (context == null) return;

            property = context.FindProperty<int>(Path, this);
            if (property != null) {
                property.OnChange += OnChange;
            }
        }

        protected override void Unbind() {
            if (property != null) {
                property.OnChange -= OnChange;
                property = null;
            }
        }

        protected override void OnChange() {
            if (property == null) {
                return;
            }
            
            transform.DestroyChildren();
            
            var value = property.GetValue();
            var pair = pairs.FirstOrDefault(x => x.Value == value);
            var prefab = pair != null ? pair.Prefab : defaultPrefab;
            
            if (prefab != null) {
                GameObject.Instantiate(prefab, transform);
            }
        }
    }
}
