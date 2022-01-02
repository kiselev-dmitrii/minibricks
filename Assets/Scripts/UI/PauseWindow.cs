using MiniBricks.Controllers;
using MiniBricks.Tetris;
using MiniBricks.Utils;

namespace MiniBricks.UI {
    public class PauseWindowFactory {
        private readonly LobbyController lobbyController;

        public PauseWindowFactory(LobbyController lobbyController) {
            this.lobbyController = lobbyController;
        }

        public PauseWindow Create(GameSimulation game) {
            return new PauseWindow(game, lobbyController);
        }
    }
    
    public class PauseWindow : Window {
        private readonly GameSimulation game;
        private readonly LobbyController lobbyController;

        public PauseWindow(GameSimulation game, LobbyController lobbyController) : base("UI/PauseWindow/PauseWindow") {
            this.game = game;
            this.lobbyController = lobbyController;
        }

        protected override void OnActivated() {
            game.IsPaused = true;
        }

        protected override void OnDeactivated() {
            game.IsPaused = false;
        }

        public async void OnQuitButtonClick() {
            Destroy();
            await lobbyController.LeaveGame();
        }
        
        public void OnContinueButtonClick() {
            Destroy();
        }
    }
}