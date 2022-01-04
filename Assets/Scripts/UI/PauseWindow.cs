using MiniBricks.Controllers;
using MiniBricks.Game.Entities;
using MiniBricks.Utils;

namespace MiniBricks.UI {
    public class PauseWindowFactory {
        private readonly LobbyController lobbyController;
        private readonly MultiplayerGame multiplayerGame;

        public PauseWindowFactory(LobbyController lobbyController, MultiplayerGame multiplayerGame) {
            this.lobbyController = lobbyController;
            this.multiplayerGame = multiplayerGame;
        }

        public PauseWindow Create() {
            return new PauseWindow(multiplayerGame, lobbyController);
        }
    }
    
    public class PauseWindow : Window {
        private readonly MultiplayerGame multiplayerGame;
        private readonly LobbyController lobbyController;

        public PauseWindow(MultiplayerGame multiplayerGame, LobbyController lobbyController) : base("UI/PauseWindow/PauseWindow") {
            this.multiplayerGame = multiplayerGame;
            this.lobbyController = lobbyController;
        }

        protected override void OnActivated() {
            multiplayerGame.Pause();
        }

        protected override void OnDeactivated() {
            multiplayerGame.Unpause();
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