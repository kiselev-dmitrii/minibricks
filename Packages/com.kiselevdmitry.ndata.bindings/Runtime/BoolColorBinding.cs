using NData;
using UnityEngine;
using UnityEngine.UI;

namespace KiselevDmitry.NData.Bindings {
    [RequireComponent(typeof(Image))]
    public class BoolColorBinding : BooleanBinding {
        public Color TrueColor;
        public Color FalseColor;
        private Image image;

        public override void Awake() {
            image = GetComponent<Image>();
        }

        protected override void ApplyNewValue(bool newValue) {
            image.color = newValue ? TrueColor : FalseColor;
        }
    }
}
