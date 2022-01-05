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
    public class BattleGameLauncher : IGameLauncher {
        private readonly ApplicationDef appDef;
        private readonly TickProvider tickProvider;
        private readonly LobbyController lobbyController;

        public BattleGameLauncher(ApplicationDef appDef, TickProvider tickProvider, LobbyController lobbyController) {
            this.appDef = appDef;
            this.tickProvider = tickProvider;
            this.lobbyController = lobbyController;
        }

        public GameType Type => GameType.Battle;

        public IDisposable Launch() {
            return new BattleGameRunner(this);
        }
        
        private class BattleGameRunner : IDisposable, ITickable {
            private readonly BattleGameLauncher l;

            private readonly Game game;
            
            private readonly CameraView playerCamera;
            private readonly ICommandProvider playerInput;

            private readonly CameraView enemyCamera;
            private readonly ICommandProvider enemyInput;

            private readonly GameScreen gameScreen;
            private readonly GameOverWatcher watcher;
            
            public BattleGameRunner(BattleGameLauncher l) {
                this.l = l;

                var appDef = l.appDef;
                var gameSettings = appDef.GameSettings;
                var pieceFactory = new PieceFactory(gameSettings);
                var towerFactory = new TowerFactory(gameSettings, pieceFactory);
                game = new Game(gameSettings, towerFactory); 
                
                var player = game.CreateTower();
                player.SetPlatform(appDef.Platforms[0]);
                playerCamera = Object.Instantiate(appDef.Camera, game.Transform);
                playerCamera.SetTarget(player);
                playerInput = new AnyCommandProvider(new ICommandProvider[] {
                    game.Transform.gameObject.AddComponent<MouseCommandProvider>().Initialize(player.GetId(), playerCamera),
                    new KeyboardCommandProvider(player.GetId())
                });
                player.GetComponent<TowerView>().Initialize(game);
                
                var enemy = game.CreateTower();
                enemy.SetPlatform(appDef.Platforms[1]);
                enemyCamera = Object.Instantiate(appDef.Camera, game.Transform);
                enemyCamera.SetTarget(enemy);
                enemyCamera.SetRenderTextureOutput(270, 480);
                enemyInput = new RandomCommandProvider(enemy.GetId());
                enemy.GetComponent<TowerView>().Initialize(game);
                
                var pauseWindowFactory = new PauseWindowFactory(l.lobbyController, game);
                var gameOverWindowFactory = new GameOverWindowFactory(l.lobbyController);

                gameScreen = new GameScreen();
                gameScreen.AddComponent(new PlayerStateGameScreenComponent(player, pauseWindowFactory));
                gameScreen.AddComponent(new EnemyStateGameScreenComponent(game, enemy, enemyCamera.GetRenderTexture()));
                gameScreen.SetActive(true);
                
                watcher = new GameOverWatcher(game, player, gameScreen, gameOverWindowFactory);
                
                game.Start();
                
                l.tickProvider.AddTickable(this);
            }
    
            public void Tick() {
                game.AddCommand(playerInput.GetNextCommand());
                game.AddCommand(enemyInput.GetNextCommand());
                game.Tick();
            }
            
            public void Dispose() {
                l.tickProvider.RemoveTickable(this);
                gameScreen.Destroy();
                game.Dispose();
                playerCamera.Destroy();
                enemyCamera.Destroy();
                watcher.Dispose();
            }
        }
    
    }
}