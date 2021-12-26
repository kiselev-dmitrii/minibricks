using UnityEngine;

namespace KiselevDmitry.Utils {
    public static class TransformUtils {
        public static void DestroyChildren(this Transform transform) {
            int count = transform.childCount;
            for (int i = count - 1; i >= 0; i--) {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
