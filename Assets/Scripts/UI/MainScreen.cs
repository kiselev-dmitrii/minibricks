using MiniBricks.Controllers;
using MiniBricks.Utils;

namespace MiniBricks.UI {
    public class MainScreenFactory {
        private readonly LobbyController lobbyController;
        private readonly WindowManager windwWindowManager;

        public MainScreenFactory(LobbyController lobbyController) {
            this.lobbyController = lobbyController;
        }

        public MainScreen Create() {
            return new MainScreen(lobbyController);
        }
    }
    
    public class MainScreen : Window {
        private readonly LobbyController lobbyController;

        public MainScreen(LobbyController lobbyController) : base("UI/MainScreen/MainScreen") {
            this.lobbyController = lobbyController;
        }

        public async void OnStartTrainingButtonClick() {
            await lobbyController.StartGame(GameType.Training);
        }

        public async void OnStartBattleButtonClick() {
            await lobbyController.StartGame(GameType.Battle);
        }
    }
}