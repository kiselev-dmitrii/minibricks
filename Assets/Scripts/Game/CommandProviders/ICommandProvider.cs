using MiniBricks.Game.Entities;

namespace MiniBricks.Game.CommandProviders {
    public interface ICommandProvider {
        public ICommand GetNextCommand();
    }
}