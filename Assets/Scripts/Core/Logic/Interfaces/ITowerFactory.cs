using UnityEngine;

namespace MiniBricks.Core.Logic.Interfaces {
    public interface ITowerFactory {
        public Tower Create(int towerId, Vector3 position, Transform parent);
    }
}