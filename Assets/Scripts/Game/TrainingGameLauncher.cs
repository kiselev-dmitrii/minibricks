using System;
using MiniBricks.Game;
using MiniBricks.Game.CommandProviders;
using MiniBricks.Tetris;
using MiniBricks.UI;
using MiniBricks.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiniBricks.Controllers {
    public class TrainingGameLauncher : IGameLauncher {
        private readonly TowerGameDef towerGameDef;
        private readonly PieceFactory pieceFactory;
        private readonly TickProvider tickProvider;
        private readonly LobbyController lobbyController;

        public TrainingGameLauncher(TowerGameDef towerGameDef, PieceFactory pieceFactory, 
                                    TickProvider tickProvider, LobbyController lobbyController) {
            this.towerGameDef = towerGameDef;
            this.pieceFactory = pieceFactory;
            this.tickProvider = tickProvider;
            this.lobbyController = lobbyController;
        }

        public GameType Type => GameType.Training;

        public IDisposable Launch() {
            return new TrainingGameRunner(this);
        }
        
        private class TrainingGameRunner : IDisposable, ITickable {
            private readonly TrainingGameLauncher l;
            private readonly ICommandProvider input;
            private readonly Map map;
            private readonly TowerGame game;
            private readonly GameScreen gameScreen;
            private readonly GameOverWatcher watcher;
            
            public TrainingGameRunner(TrainingGameLauncher l) {
                this.l = l;

                var pauseWindowFactory = new PauseWindowFactory(l.lobbyController);
                var gameScreenFactory = new GameScreenFactory(pauseWindowFactory);
                var gameOverWindowFactory = new GameOverWindowFactory(l.lobbyController);
                
                var mapPrefab = Resources.Load<Map>("Maps/Map01");
                
                input = new KeyboardCommandProvider();
                map = Object.Instantiate(mapPrefab);
                game = new TowerGame(l.towerGameDef, map, l.pieceFactory);
            
                map.Camera.GetComponent<FollowCamera>().Initialize(game, map);
                
                gameScreen = gameScreenFactory.Create(game);
                gameScreen.SetActive(true);
                
                game.Start();

                watcher = new GameOverWatcher(game, gameScreen, gameOverWindowFactory);
                
                l.tickProvider.AddTickable(this);
            }
    
            public void Tick() {
                game.AddCommand(input.GetNextCommand());
                game.Tick();
                
                gameScreen.Update();
            }
            
            public void Dispose() {
                gameScreen.Destroy();
                game.Dispose();
                l.tickProvider.RemoveTickable(this);
                GameObject.Destroy(map.gameObject);
                watcher.Dispose();
            }
        }

        private class GameOverWatcher : IDisposable {
            private readonly TowerGame towerGame;
            private readonly GameScreen gameScreen;
            private readonly GameOverWindowFactory gameOverWindowFactory;

            public GameOverWatcher(TowerGame towerGame, GameScreen gameScreen, GameOverWindowFactory gameOverWindowFactory) {
                this.towerGame = towerGame;
                this.gameScreen = gameScreen;
                this.gameOverWindowFactory = gameOverWindowFactory;
                towerGame.StateChanged += OnGameStateChanged;
            }
            
            public void Dispose() {
                towerGame.StateChanged -= OnGameStateChanged;
            }

            private void OnGameStateChanged(TowerGame _) {
                gameScreen.SetActive(false);
                var window = gameOverWindowFactory.Create(towerGame);
                window.AddUser("Player", towerGame.GetNumFalls(), towerGame.GetMaxHeight(), true);
                window.SetActive(true);
            }
        }
    }
    
    
}