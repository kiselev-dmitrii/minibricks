using NData;
using UnityEngine;
using UnityEngine.UI;

namespace KiselevDmitry.NData.Bindings {
    public class TextureBinding : Binding {
        private RawImage image;
        private Property<Texture> property;

        public override void Awake() {
            image = GetComponent<RawImage>();
        }

        protected override void Bind() {
            var context = GetContext(Path);
            if (context == null) return;

            property = context.FindProperty<Texture>(Path, this);
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
            var texture = property.GetValue();
            image.texture = texture;
        }
    }
}