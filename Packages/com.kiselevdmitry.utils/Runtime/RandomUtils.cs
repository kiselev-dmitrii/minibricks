using System.Collections.Generic;

namespace KiselevDmitry.Utils {
    public static class RandomUtils {
        public static T GetRandom<T>(this IReadOnlyList<T> collection) {
            int i = UnityEngine.Random.Range(0, collection.Count);
            return collection[i];
        }
    }
}