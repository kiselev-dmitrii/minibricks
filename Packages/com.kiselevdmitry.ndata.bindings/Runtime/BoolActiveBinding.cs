using NData;

namespace KiselevDmitry.NData.Bindings {
    public class BoolActiveBinding : BooleanBinding {
        protected override void ApplyNewValue(bool newValue) {
            gameObject.SetActive(newValue);
        }
    }
}
