using System;
using MiniBricks.Game;
using MiniBricks.Game.Commands;
using MiniBricks.Tetris;
using MiniBricks.UI;
using MiniBricks.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiniBricks.Controllers {
    public class TrainingGameLauncher : IGameLauncher {
        private readonly TowerGameDef towerGameDef;
        private readonly IPieceFactory pieceFactory;
        private readonly LobbyController lobbyController;
        private readonly TickProvider tickProvider;

        public TrainingGameLauncher(TowerGameDef towerGameDef, IPieceFactory pieceFactory, 
                                    LobbyController lobbyController, TickProvider tickProvider) {
            this.towerGameDef = towerGameDef;
            this.pieceFactory = pieceFactory;
            this.lobbyController = lobbyController;
            this.tickProvider = tickProvider;
        }

        public GameType Type => GameType.Training;

        public IDisposable Launch() {
            return new TrainingGameRunner(this);
        }
        
        private class TrainingGameRunner : IDisposable, ITickable {
            private readonly TrainingGameLauncher launcher;
            private readonly ICommandProvider input;
            private readonly TowerGame game;
            private readonly GameScreen gameScreen;
            
            public TrainingGameRunner(TrainingGameLauncher launcher) {
                this.launcher = launcher;
                var mapPrefab = Resources.Load<Map>("Maps/Map01");
                
                var map1 = Object.Instantiate(mapPrefab);
                game = new TowerGame(launcher.towerGameDef, map1, launcher.pieceFactory);
                input = new KeyboardCommandProvider();
                gameScreen = new GameScreen(game);
                gameScreen.SetActive(true);
                
                game.Start();
                
                launcher.tickProvider.AddTickable(this);
            }
    
            public void Tick() {
                var cmd1 = input.GetNextCommand();
                if (cmd1 != null) {
                    game.ProcessCommand(cmd1.Value);
                }
                game.Tick();
                gameScreen.Update();
            }
            
            public void Dispose() {
                gameScreen.Destroy();
                game.Dispose();
                launcher.tickProvider.RemoveTickable(this);
            }
        }
    }
}