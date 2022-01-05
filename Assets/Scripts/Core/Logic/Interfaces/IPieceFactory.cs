using UnityEngine;

namespace MiniBricks.Core.Logic.Interfaces {
    public interface IPieceFactory {
        Piece Create(Piece prefab, Vector2 position, Transform parent);
    }
}