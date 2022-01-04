using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiniBricks.Utils {
    public static class GameObjectUtils {
        public static T Instantiate<T>(String path, Transform parent = null) where T : Object {
            var prefab = Resources.Load<T>(path);
            return Object.Instantiate(prefab, parent);
        }
    }
}