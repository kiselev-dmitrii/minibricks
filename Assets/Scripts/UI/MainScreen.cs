using MiniBricks.Controllers;
using MiniBricks.Utils;

namespace MiniBricks.UI {
    public class MainScreen : Window {
        private readonly LobbyController lobbyController;

        public MainScreen(LobbyController lobbyController) : base("UI/MainScreen/MainScreen") {
            this.lobbyController = lobbyController;
        }

        public async void OnStartTrainingButtonClick() {
            await lobbyController.StartGame();
        }

        public void OnStartBattleButtonClick() {
            
        }
    }
}