using NData;
using UnityEngine;
using UnityEngine.UI;

namespace KiselevDmitry.NData.Bindings {
    [RequireComponent(typeof(Image))]
    public class SpriteBinding : Binding {
        private Image image;
        private Property<Sprite> property;

        public override void Awake() {
            image = GetComponent<Image>();
        }

        protected override void Bind() {
            var context = GetContext(Path);
            if (context == null) return;

            property = context.FindProperty<Sprite>(Path, this);
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
            var sprite = property.GetValue();
            image.sprite = sprite;
        }
    }
}
