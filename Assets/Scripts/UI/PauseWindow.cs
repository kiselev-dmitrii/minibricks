using MiniBricks.Controllers;
using MiniBricks.Utils;

namespace MiniBricks.UI {
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