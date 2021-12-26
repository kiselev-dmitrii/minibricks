using NData;
using UnityEngine;
using UnityEngine.UI;

namespace KiselevDmitry.NData.Bindings {
    [RequireComponent(typeof(Button))]
    public class InteractableBinding : BooleanBinding {
        private Button button;

        public override void Awake() {
            button = GetComponent<Button>();
        }

        protected override void ApplyNewValue(bool newValue) {
            button.interactable = newValue;
        }
    }
}
