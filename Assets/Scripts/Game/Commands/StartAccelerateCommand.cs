using MiniBricks.Controllers;
using MiniBricks.Game.Entities;

namespace MiniBricks.Game.Commands {
    public class StartAccelerateCommand : ICommand {
        public StartAccelerateCommand(int towerId) {
            TowerId = towerId;
        }
        
        public int TowerId { get; }
        
        public void Execute(Tower tower) {
            tower.GetCurrentPiece()?.SetAccelerated(true);
        }
    }
}