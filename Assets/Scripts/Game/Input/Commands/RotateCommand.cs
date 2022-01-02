using MiniBricks.Tetris;

namespace MiniBricks.Game.Commands {
    public class RotateCommand : ICommand {
        public void Execute(Piece piece) {
            piece.Rotate();
        }
    }
}