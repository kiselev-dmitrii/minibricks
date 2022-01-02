using MiniBricks.Game.CommandProviders;

namespace MiniBricks.Tetris {
    public interface ICommandProvider {
        public ICommand GetNextCommand();
    }
    
    public class InputFeature : IFeature {
        private readonly GameSimulation game;
        private readonly ICommandProvider commandProvider;
        private GameData data;

        public InputFeature(GameSimulation game, ICommandProvider commandProvider) {
            this.game = game;
            this.commandProvider = commandProvider;
        }
        
        public void GameStarted() {
            data = game.Data;
        }

        public void Tick() {
            var command = commandProvider.GetNextCommand();
            if (command == null) {
                return;
            }
            if (data.CurrentPiece == null) {
                return;
            }
            
            command.Execute(data.CurrentPiece);
        }

        public void GameFinished(GameResult result) {
        }
    }
}