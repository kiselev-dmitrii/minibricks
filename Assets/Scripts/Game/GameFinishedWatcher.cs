using System;
using MiniBricks.Game.Entities;
using MiniBricks.UI;

namespace MiniBricks.Controllers {
    public class GameFinishedWatcher : IDisposable {
        private readonly MultiplayerGame multiplayerGame;
        private readonly Tower playerTower;
        private readonly GameScreen gameScreen;
        private readonly GameOverWindowFactory gameOverWindowFactory;

        public GameFinishedWatcher(MultiplayerGame multiplayerGame, Tower playerTower, GameScreen gameScreen,
            GameOverWindowFactory gameOverWindowFactory) {
            this.multiplayerGame = multiplayerGame;
            this.playerTower = playerTower;
            this.gameScreen = gameScreen;
            this.gameOverWindowFactory = gameOverWindowFactory;
            multiplayerGame.TowerDeactivated += OnTowerDeactivated;
        }

        public void Dispose() {
            multiplayerGame.TowerDeactivated -= OnTowerDeactivated;
        }

        private void OnTowerDeactivated(Tower tower, GameResult gameResult) {
            if (tower != playerTower) {
                return;
            }

            multiplayerGame.Pause();
            gameScreen.SetActive(false);
            var window = gameOverWindowFactory.Create(gameResult);

            var towerResults = multiplayerGame.GetTowerResults();
            foreach (var towerResult in towerResults) {
                var t = towerResult.Tower;
                var name = $"Tower{t.GetId()}";
                bool isPlayer = playerTower == t;
                window.AddUser(name, t.GetNumFalls(), t.GetMaxHeight(), isPlayer);
            }
            window.SetActive(true);
        }
    }
}