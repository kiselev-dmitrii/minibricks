using MiniBricks.Core.Logic.Interfaces;

namespace MiniBricks.Core.Logic.Commands {
    public class LeftCommand : ICommand {
        public LeftCommand(int towerId) {
            TowerId = towerId;
        }
        
        public int TowerId { get; }
        
        public void Execute(Game game) {
            var tower = game.GetTower(TowerId);
            tower.GetCurrentPiece()?.Move(-1);
        }

    }
}