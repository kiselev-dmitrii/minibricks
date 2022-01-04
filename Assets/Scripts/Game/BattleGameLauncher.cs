using System;
using MiniBricks.Game;
using MiniBricks.Game.CommandProviders;
using MiniBricks.Game.Entities;
using MiniBricks.Tetris;
using MiniBricks.UI;
using MiniBricks.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiniBricks.Controllers {
    public class BattleGameLauncher : IGameLauncher {
        private readonly MultiplayerGameDef gameDef;
        private readonly PieceFactory pieceFactory;
        private readonly TickProvider tickProvider;
        private readonly LobbyController lobbyController;

        public BattleGameLauncher(MultiplayerGameDef gameDef, PieceFactory pieceFactory, 
            TickProvider tickProvider, LobbyController lobbyController) {
            this.gameDef = gameDef;
            this.pieceFactory = pieceFactory;
            this.tickProvider = tickProvider;
            this.lobbyController = lobbyController;
        }

        public GameType Type => GameType.Battle;

        public IDisposable Launch() {
            return new BattleGameRunner(this);
        }
        
        private class BattleGameRunner : IDisposable, ITickable {
            private readonly BattleGameLauncher l;

            private readonly MultiplayerGame game;
            
            private readonly CameraView playerCamera;
            private readonly ICommandProvider playerInput;

            private readonly CameraView enemyCamera;
            private readonly ICommandProvider enemyInput;

            private readonly GameScreen gameScreen;
            private readonly GameFinishedWatcher watcher;
            
            public BattleGameRunner(BattleGameLauncher l) {
                this.l = l;

                game = new MultiplayerGame(l.gameDef, l.pieceFactory); 
                
                var player = game.CreateTower();
                player.SetPlatform("Entities/Platforms/Platform1");
                playerCamera = GameObjectUtils.Instantiate<CameraView>("Entities/Camera", game.Transform);
                playerCamera.SetTarget(player);
                playerInput = new AnyCommandProvider(new ICommandProvider[] {
                    game.Transform.gameObject.AddComponent<MouseCommandProvider>().Initialize(player.GetId(), playerCamera),
                    new KeyboardCommandProvider(player.GetId())
                });
                player.GetComponent<TowerView>().Initialize(game);
                
                var enemy = game.CreateTower();
                enemy.SetPlatform("Entities/Platforms/Platform2");
                enemyCamera = GameObjectUtils.Instantiate<CameraView>("Entities/Camera", game.Transform);
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
                
                watcher = new GameFinishedWatcher(game, player, gameScreen, gameOverWindowFactory);
                
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