using System;
using MiniBricks.Game;
using MiniBricks.Game.CommandProviders;
using MiniBricks.Game.Entities;
using MiniBricks.Tetris;
using MiniBricks.UI;
using MiniBricks.Utils;
using UnityEditor.iOS;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiniBricks.Controllers {
    public class TrainingGameLauncher : IGameLauncher {
        private readonly MultiplayerGameDef gameDef;
        private readonly IPieceFactory pieceFactory;
        private readonly TickProvider tickProvider;
        private readonly LobbyController lobbyController;

        public TrainingGameLauncher(MultiplayerGameDef gameDef, IPieceFactory pieceFactory, 
                                    TickProvider tickProvider, LobbyController lobbyController) {
            this.gameDef = gameDef;
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
            private readonly ICommandProvider playerInput;
            private readonly GameScreen gameScreen;
            private readonly GameFinishedWatcher watcher;
            private readonly MultiplayerGame game;
            private readonly CameraView playerCamera;

            public TrainingGameRunner(TrainingGameLauncher l) {
                this.l = l;

                game = new MultiplayerGame(l.gameDef, l.pieceFactory);
                
                var player = game.CreateTower();
                player.SetPlatform("Entities/Platforms/Platform1");
                playerCamera = GameObjectUtils.Instantiate<CameraView>("Entities/Camera", game.Transform);
                playerCamera.SetTarget(player);
                playerInput = new KeyboardCommandProvider(player.GetId());
                player.GetComponent<TowerView>().Initialize(game);

                var pauseWindowFactory = new PauseWindowFactory(l.lobbyController, game);
                var gameOverWindowFactory = new GameOverWindowFactory(l.lobbyController);

                gameScreen = new GameScreen();
                gameScreen.AddComponent(new PlayerStateGameScreenComponent(player, pauseWindowFactory));
                gameScreen.SetActive(true);
                
                watcher = new GameFinishedWatcher(game, player, gameScreen, gameOverWindowFactory);
                
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