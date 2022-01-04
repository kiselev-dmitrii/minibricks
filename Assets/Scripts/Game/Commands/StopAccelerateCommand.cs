using MiniBricks.Controllers;
using MiniBricks.Game.Entities;

namespace MiniBricks.Game.Commands {
    public class StopAccelerateCommand : ICommand {
        public StopAccelerateCommand(int towerId) {
            TowerId = towerId;
        }

        public int TowerId { get; }
        
        public void Execute(Tower tower) {
            tower.GetCurrentPiece()?.SetAccelerated(false);
        }
    }

}