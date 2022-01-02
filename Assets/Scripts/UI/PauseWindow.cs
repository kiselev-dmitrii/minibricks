using MiniBricks.Controllers;
using MiniBricks.Tetris;
using MiniBricks.Utils;

namespace MiniBricks.UI {
    public class PauseWindowFactory {
        private readonly LobbyController lobbyController;

        public PauseWindowFactory(LobbyController lobbyController) {
            this.lobbyController = lobbyController;
        }

        public PauseWindow Create(TowerGame game) {
            return new PauseWindow(game, lobbyController);
        }
    }
    
    public class PauseWindow : Window {
        private readonly TowerGame game;
        private readonly LobbyController lobbyController;

        public PauseWindow(TowerGame game, LobbyController lobbyController) : base("UI/PauseWindow/PauseWindow") {
            this.game = game;
            this.lobbyController = lobbyController;
        }

        protected override void OnActivated() {
            game.Pause();
        }

        protected override void OnDeactivated() {
            game.Unpause();
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