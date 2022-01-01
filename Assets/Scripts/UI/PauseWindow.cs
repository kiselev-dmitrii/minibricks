using MiniBricks.Controllers;
using MiniBricks.Utils;

namespace MiniBricks.UI {
    public class PauseWindowFactory {
        private readonly LobbyController lobbyController;

        public PauseWindowFactory(LobbyController lobbyController) {
            this.lobbyController = lobbyController;
        }

        public PauseWindow Create() {
            return new PauseWindow(lobbyController);
        }
    }
    
    public class PauseWindow : Window {
        private readonly LobbyController lobbyController;

        public PauseWindow(LobbyController lobbyController) : base("UI/PauseWindow/PauseWindow") {
            this.lobbyController = lobbyController;
        }

        public void OnBackButtonClick() {
            Destroy();
        }

        public async void OnQuitButtonClick() {
            await lobbyController.LeaveGame();
        }
    }
}