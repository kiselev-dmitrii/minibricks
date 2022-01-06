using MiniBricks.Controllers;
using MiniBricks.Core.Logic;
using MiniBricks.Utils;

namespace MiniBricks.UI {
    public class PauseWindowFactory {
        private readonly LobbyController lobbyController;
        private readonly Game game;

        public PauseWindowFactory(LobbyController lobbyController, Game game) {
            this.lobbyController = lobbyController;
            this.game = game;
        }

        public PauseWindow Create() {
            return new PauseWindow(game, lobbyController);
        }
    }
    
    public class PauseWindow : Window {
        private readonly Game game;
        private readonly LobbyController lobbyController;

        public PauseWindow(Game game, LobbyController lobbyController) : base("UI/PauseWindow/PauseWindow") {
            this.game = game;
            this.lobbyController = lobbyController;
        }

        protected override void OnActivated() {
            game.Pause();
        }

        protected override void OnDeactivated() {
            game.Unpause();
        }

        public void OnQuitButtonClick() {
            Destroy();
            lobbyController.LeaveGame();
        }
        
        public void OnContinueButtonClick() {
            Destroy();
        }
    }
}