using MiniBricks.Controllers;
using MiniBricks.UI;
using UnityEngine;

namespace MiniBricks {
    public class Application : MonoBehaviour {
        private void Awake() {
            var gameContextFactory = new GameContextFactory();
            var lobbyController = new LobbyController(gameContextFactory);
            var mainScreen = new MainScreen(lobbyController);
            mainScreen.SetActive(true);
        }
    }
}