using MiniBricks.Controllers;
using MiniBricks.Game.Entities;

namespace MiniBricks.Game.Commands {
    public class RightCommand : ICommand {
        public RightCommand(int towerId) {
            TowerId = towerId;
        }

        public int TowerId { get; }
        
        public void Execute(Tower tower) {
            tower.GetCurrentPiece()?.Move(+1);
        }
    }
}