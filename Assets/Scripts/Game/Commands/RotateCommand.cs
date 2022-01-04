using MiniBricks.Controllers;
using MiniBricks.Game.Entities;

namespace MiniBricks.Game.Commands {
    public class RotateCommand : ICommand {
        public RotateCommand(int towerId) {
            TowerId = towerId;
        }

        public int TowerId { get; }
        
        public void Execute(Tower tower) {
            tower.GetCurrentPiece()?.Rotate();
        }
    }
}