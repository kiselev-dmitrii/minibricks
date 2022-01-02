using MiniBricks.Tetris;

namespace MiniBricks.Game.Commands {
    public class LeftCommand : ICommand {
        public void Execute(Piece piece) {
            piece.Move(-1);
        }
    }
}