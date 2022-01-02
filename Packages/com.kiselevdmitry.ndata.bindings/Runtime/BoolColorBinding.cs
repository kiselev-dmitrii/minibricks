using NData;
using UnityEngine;
using UnityEngine.UI;

namespace KiselevDmitry.NData.Bindings {
    [RequireComponent(typeof(Graphic))]
    public class BoolColorBinding : BooleanBinding {
        public Color TrueColor;
        public Color FalseColor;
        private Graphic graphic;

        public override void Awake() {
            graphic = GetComponent<Graphic>();
        }

        protected override void ApplyNewValue(bool newValue) {
            graphic.color = newValue ? TrueColor : FalseColor;
        }
    }
}
