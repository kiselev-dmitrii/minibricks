using NData;
using UnityEngine;

namespace KiselevDmitry.NData.Bindings {
    public class RepeatItemBinding : Binding {
        public GameObject ItemTemplate;
        
        private Transform cachedTransform;
        private Property<int> property;

        public override void Awake() {
            cachedTransform = transform;
        }

        protected override void Bind() {
            var context = GetContext(Path);
            if (context == null) {
                return;
            }

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

            var targetValue = Mathf.Abs(property.GetValue());
            var value = cachedTransform.childCount;
            int delta = targetValue - value;
            if (delta == 0) {
                return;
            }

            if (delta > 0) {
                CreateItems(delta);
            }
            else {
                RemoveItems(-delta);
            }
        }

        private void CreateItems(int n) {
            for (int i = 0; i < n; ++i) {
                Instantiate(ItemTemplate, cachedTransform);
            }
        }

        private void RemoveItems(int n) {
            for (int i = 0; i < n; ++i) {
                var child = cachedTransform.GetChild(0);
                Destroy(child.gameObject);
            }
        }
    }
}