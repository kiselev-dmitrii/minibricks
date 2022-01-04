using MiniBricks.Controllers;
using MiniBricks.Game.Entities;

namespace MiniBricks.Game.Commands {
    public class LeftCommand : ICommand {
        public LeftCommand(int towerId) {
            TowerId = towerId;
        }
        
        public int TowerId { get; }
        
        public void Execute(Tower tower) {
            tower.GetCurrentPiece()?.Move(-1);
        }
        

    }
}