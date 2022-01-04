using System;
using MiniBricks.Controllers;
using MiniBricks.UI;

namespace MiniBricks.CompositionRoot {
    public class MainScreenWatcher : IDisposable {
        private readonly LobbyController lobbyController;
        private readonly MainScreenFactory mainScreenFactory;
        private MainScreen mainScreen;
        
        public MainScreenWatcher(LobbyController lobbyController, MainScreenFactory mainScreenFactory) {
            this.lobbyController = lobbyController;
            this.mainScreenFactory = mainScreenFactory;
            lobbyController.GameStateChanged += Update;
        }
        
        public void Dispose() {
            lobbyController.GameStateChanged -= Update;
        }

        public void Update() {
            if (mainScreen == null && lobbyController.GameState == GameState.Menu) {
                mainScreen = mainScreenFactory.Create();
                mainScreen.SetActive(true);
                return;
            }

            if (mainScreen != null && lobbyController.GameState == GameState.InGame) {
                mainScreen.Destroy();
                mainScreen = null;
            }
        }

    }
}