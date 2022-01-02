using MiniBricks.Tetris;

namespace MiniBricks.Game.Commands {
    public class StopAccelerateCommand : ICommand {
        public void Execute(Piece piece) {
            piece.SetAccelerated(false);
        }
    }

}