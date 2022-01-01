using MiniBricks.Tetris;

namespace MiniBricks.Game.CommandProviders {
    public interface ICommandProvider {
        public ICommand GetNextCommand();
    }
}