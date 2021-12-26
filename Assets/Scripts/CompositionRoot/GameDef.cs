using MiniBricks.Game;
using MiniBricks.Tetris;
using UnityEngine;

namespace MiniBricks {
    [CreateAssetMenu(menuName = "Game/GameDef")]
    public class GameDef : ScriptableObject {
        public TowerGameDef TowerGame;
        public PieceDef Piece;
    }
}