using MiniBricks.Game.CommandProviders;

namespace MiniBricks.Tetris {
    public interface ICommandProvider {
        public ICommand GetNextCommand();
    }
    
    public class InputFeature : IFeature {
        private readonly TetrisFeature tetrisFeature;
        private readonly ICommandProvider commandProvider;

        public InputFeature(TetrisFeature tetrisFeature, ICommandProvider commandProvider) {
            this.tetrisFeature = tetrisFeature;
            this.commandProvider = commandProvider;
        }
        
        public void GameStarted() {
        }

        public void Tick() {
            var command = commandProvider.GetNextCommand();
            if (command == null) {
                return;
            }
            if (tetrisFeature.CurrentPiece == null) {
                return;
            }
            
            command.Execute(tetrisFeature.CurrentPiece);
        }

        public void GameFinished(GameResult result) {
        }
    }
}