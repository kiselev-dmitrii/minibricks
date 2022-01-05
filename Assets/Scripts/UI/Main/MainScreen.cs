using MiniBricks.Controllers;
using MiniBricks.Utils;

namespace MiniBricks.UI.Main {
    public class MainScreenFactory {
        private readonly LobbyController lobbyController;

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

        public void OnStartTrainingButtonClick() {
            lobbyController.StartGame(GameType.Training);
        }

        public void OnStartBattleButtonClick() {
            lobbyController.StartGame(GameType.Battle);
        }
    }
}