using System.Collections.Generic;

namespace KiselevDmitry.Utils {
    public static class DictionaryUtils {
        public static T Get<K, T>(this IDictionary<K, T> dictionary, K key) {
            if (dictionary.TryGetValue(key, out T value)) {
                return value;
            }

            return default(T);
        }
    }
}