using NData;
using UnityEngine;
using UnityEngine.UI;

namespace KiselevDmitry.NData.Bindings {
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
            if (property == null) return;

            property.OnChange += OnChange;
        }

        protected override void Unbind() {
            if (property == null) return;
            property.OnChange -= OnChange;
            property = null;
        }

        protected override void OnChange() {
            if (property == null) return;
            Sprite sprite = property.GetValue();
            image.sprite = sprite;
        }
    }
}
