using System;
using MiniBricks.Core.Logic;
using MiniBricks.UI.Core;

namespace MiniBricks.UI.GameOver {
    public class GameOverWatcher : IDisposable {
        private readonly Game game;
        private readonly Tower playerTower;
        private readonly GameScreen gameScreen;
        private readonly GameOverWindowFactory gameOverWindowFactory;

        public GameOverWatcher(Game game, Tower playerTower, GameScreen gameScreen, GameOverWindowFactory gameOverWindowFactory) {
            this.game = game;
            this.playerTower = playerTower;
            this.gameScreen = gameScreen;
            this.gameOverWindowFactory = gameOverWindowFactory;
            game.TowerDeactivated += OnTowerDeactivated;
        }

        public void Dispose() {
            game.TowerDeactivated -= OnTowerDeactivated;
        }

        private void OnTowerDeactivated(Tower tower, GameResult gameResult) {
            if (tower != playerTower) {
                return;
            }

            game.Pause();
            gameScreen.SetActive(false);
            var window = gameOverWindowFactory.Create(gameResult);

            var towerResults = game.GetTowerResults();
            foreach (var towerResult in towerResults) {
                var t = towerResult.Tower;
                var name = $"Tower{t.Id}";
                bool isPlayer = playerTower == t;
                window.AddUser(name, t.NumFalls, t.MaxHeight, isPlayer);
            }
            window.SetActive(true);
        }
    }
}