using NData;
using UnityEngine.EventSystems;

namespace KiselevDmitry.NData.Bindings {
    public class OnClickBinding : CommandBinding, IPointerClickHandler {
        public void OnPointerClick(PointerEventData eventData) {
            if (_command == null) {
                return;
            }

            _command.DynamicInvoke();
        }
    }
}
