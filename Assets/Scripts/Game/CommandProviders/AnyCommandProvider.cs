using MiniBricks.Game.Entities;

namespace MiniBricks.Game.CommandProviders {
    public class AnyCommandProvider : ICommandProvider {
        private readonly ICommandProvider[] providers;

        public AnyCommandProvider(ICommandProvider[] providers) {
            this.providers = providers;
        }
        
        public ICommand GetNextCommand() {
            int n = providers.Length;
            for (int i = 0; i < n; i++) {
                var cmd = providers[i].GetNextCommand();
                if (cmd != null) {
                    return cmd;
                }
            }
            return null;
        }
    }
}