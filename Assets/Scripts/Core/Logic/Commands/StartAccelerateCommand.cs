using MiniBricks.Core.Logic.Interfaces;

namespace MiniBricks.Core.Logic.Commands {
    public class StartAccelerateCommand : ICommand {
        public StartAccelerateCommand(int towerId) {
            TowerId = towerId;
        }
        
        public int TowerId { get; }
        
        public void Execute(Game game) {
            var tower = game.GetTower(TowerId);
            tower.GetCurrentPiece()?.SetSpeed(game.Settings.AcceleratedFallSpeed);
        }
    }
}