using MiniBricks.Game;
using MiniBricks.Game.Entities;
using UnityEngine;

namespace MiniBricks {
    [CreateAssetMenu(menuName = "Game/GameDef")]
    public class GameDef : ScriptableObject {
        public MultiplayerGameDef MultiplayerGame;
        public PieceDef Piece;
    }
}