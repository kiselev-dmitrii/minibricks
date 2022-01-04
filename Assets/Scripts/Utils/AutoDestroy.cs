using System.Collections;
using UnityEngine;

namespace MiniBricks.Utils {
    public class AutoDestroy : MonoBehaviour {
        [SerializeField]
        private float duration;

        private void OnEnable() {
            StartCoroutine(Coroutine());
        }

        private IEnumerator Coroutine() {
            yield return new WaitForSeconds(duration);
            GameObject.Destroy(gameObject);
        }
    }
}