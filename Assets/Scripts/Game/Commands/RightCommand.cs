using MiniBricks.Tetris;

namespace MiniBricks.Game.Commands {
    public class RightCommand : ICommand {
        public void Execute(Piece piece) {
            piece.Move(1);
        }
    }
}