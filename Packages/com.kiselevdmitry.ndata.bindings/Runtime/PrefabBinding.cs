using KiselevDmitry.Utils;
using NData;
using UnityEngine;

namespace KiselevDmitry.NData.Bindings {
    public class PrefabBinding : Binding {
        private Property<GameObject> property;

        protected override void Bind() {
            var context = GetContext(Path);
            if (context == null) return;

            property = context.FindProperty<GameObject>(Path, this);
            if (property == null) return;

            property.OnChange += OnChange;
        }

        protected override void Unbind() {
            if (property == null) return;
            property.OnChange -= OnChange;
            property = null;
        }

        protected override void OnChange() {
            transform.DestroyChildren();
            if (property == null) return;

            var prefab = property.GetValue();
            var go = Instantiate(prefab);
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
        }
    }
}
