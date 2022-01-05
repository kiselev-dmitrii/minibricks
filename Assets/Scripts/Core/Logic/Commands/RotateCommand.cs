using MiniBricks.Core.Logic.Interfaces;

namespace MiniBricks.Core.Logic.Commands {
    public class RotateCommand : ICommand {
        public RotateCommand(int towerId) {
            TowerId = towerId;
        }

        public int TowerId { get; }
        
        public void Execute(Game game) {
            var tower = game.GetTower(TowerId);
            tower.GetCurrentPiece()?.Rotate();
        }
    }
}