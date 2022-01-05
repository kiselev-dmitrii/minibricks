using MiniBricks.Core.Logic.Interfaces;

namespace MiniBricks.Core.CommandProviders {
    public interface ICommandProvider {
        public ICommand GetNextCommand();
    }
}