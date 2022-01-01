using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniBricks.Utils {
    public class DefaultButtonAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        private const float downScale = 0.95f;
        private const float downDuration = 0.05f;
        private const float upScale = 1.05f;
        private const float upDuration = 0.05f;
        private const float resetDuration = 0.01f;

        public void OnPointerDown(PointerEventData eventData) {
            transform.DOScale(downScale * Vector3.one, downDuration);
        }

        public void OnPointerUp(PointerEventData eventData) {
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(upScale * Vector3.one, upDuration));
            sequence.Append(transform.DOScale(Vector3.one, resetDuration));
            sequence.Play();
        }
    }
}