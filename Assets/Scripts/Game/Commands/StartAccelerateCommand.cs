using MiniBricks.Tetris;

namespace MiniBricks.Game.Commands {
    public class StartAccelerateCommand : ICommand {
        public void Execute(Piece piece) {
            piece.SetAccelerated(true);
        }
    }
}