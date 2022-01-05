using System;
using MiniBricks.CompositionRoot;
using MiniBricks.Controllers;
using MiniBricks.Core.CommandProviders;
using MiniBricks.Core.Logic;
using MiniBricks.Core.Logic.Factories;
using MiniBricks.Core.View;
using MiniBricks.UI;
using MiniBricks.UI.Core;
using MiniBricks.UI.GameOver;
using MiniBricks.Utils;
using Object = UnityEngine.Object;

namespace MiniBricks.Core.Launchers {
    public class TrainingGameLauncher : IGameLauncher {
        private readonly ApplicationDef appDef;
        private readonly TickProvider tickProvider;
        private readonly LobbyController lobbyController;

        public TrainingGameLauncher(ApplicationDef appDef, TickProvider tickProvider, LobbyController lobbyController) {
            this.appDef = appDef;
            this.tickProvider = tickProvider;
            this.lobbyController = lobbyController;
        }

        public GameType Type => GameType.Training;

        public IDisposable Launch() {
            return new TrainingGameRunner(this);
        }
        
        private class TrainingGameRunner : IDisposable, ITickable {
            private readonly TrainingGameLauncher l;
            private readonly ICommandProvider playerInput;
            private readonly GameScreen gameScreen;
            private readonly GameOverWatcher watcher;
            private readonly Game game;
            private readonly CameraView playerCamera;

            public TrainingGameRunner(TrainingGameLauncher l) {
                this.l = l;

                var appDef = l.appDef;
                var gameSettings = appDef.GameSettings;
                var pieceFactory = new PieceFactory(gameSettings);
                var towerFactory = new TowerFactory(gameSettings, pieceFactory);
                game = new Game(appDef.GameSettings, towerFactory);
                
                var player = game.CreateTower();
                player.SetPlatform(appDef.Platforms[0]);
                playerCamera = Object.Instantiate(appDef.Camera, game.Transform);
                playerCamera.SetTarget(player);
                playerInput = new AnyCommandProvider(new ICommandProvider[] {
                    game.Transform.gameObject.AddComponent<MouseCommandProvider>().Initialize(player.Id, playerCamera),
                    new KeyboardCommandProvider(player.Id)
                });
                player.GetComponent<TowerView>().Initialize(game);

                var pauseWindowFactory = new PauseWindowFactory(l.lobbyController, game);
                var gameOverWindowFactory = new GameOverWindowFactory(l.lobbyController);

                gameScreen = new GameScreen();
                gameScreen.AddComponent(new PlayerStateGameScreenComponent(player, pauseWindowFactory));
                gameScreen.SetActive(true);
                
                watcher = new GameOverWatcher(game, player, gameScreen, gameOverWindowFactory);
                
                game.Start();

                l.tickProvider.AddTickable(this);
            }
    
            public void Tick() {
                game.AddCommand(playerInput.GetNextCommand());
                game.Tick();
            }
            
            public void Dispose() {
                l.tickProvider.RemoveTickable(this);
                
                gameScreen.Destroy();
                game.Dispose();
                playerCamera.Destroy();
                watcher.Dispose();
            }
        }


    }
}