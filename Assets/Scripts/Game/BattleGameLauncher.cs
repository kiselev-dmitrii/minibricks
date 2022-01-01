using System;
using MiniBricks.Game;
using MiniBricks.Game.Commands;
using MiniBricks.Tetris;
using MiniBricks.UI;
using MiniBricks.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiniBricks.Controllers {    
    public class BattleGameLauncher : IGameLauncher {
        private readonly TowerGameDef towerGameDef;
        private readonly PieceFactory pieceFactory;
        private readonly TickProvider tickProvider;
        private readonly GameScreenFactory gameScreenFactory;

        public BattleGameLauncher(TowerGameDef towerGameDef, PieceFactory pieceFactory, 
            TickProvider tickProvider, GameScreenFactory gameScreenFactory) {
            this.towerGameDef = towerGameDef;
            this.pieceFactory = pieceFactory;
            this.tickProvider = tickProvider;
            this.gameScreenFactory = gameScreenFactory;
        }

        public GameType Type => GameType.Battle;

        public IDisposable Launch() {
            return new BattleGameRunner(this);
        }
        
        private class BattleGameRunner : IDisposable, ITickable {
            private readonly BattleGameLauncher launcher;
            private readonly ICommandProvider tower1Input;
            private readonly TowerGame towerGame1;
            private readonly ICommandProvider tower2Input;
            private readonly TowerGame towerGame2;
            private readonly GameScreen gameScreen;
            
            public BattleGameRunner(BattleGameLauncher launcher) {
                this.launcher = launcher;
                var mapPrefab = Resources.Load<Map>("Maps/Map01");
                
                var map1 = Object.Instantiate(mapPrefab);
                towerGame1 = new TowerGame(launcher.towerGameDef, map1, launcher.pieceFactory);
                tower1Input = new KeyboardCommandProvider();
                
                var map2 = Object.Instantiate(mapPrefab, Vector3.right*100, Quaternion.identity);
                map2.Camera.enabled = false;
                towerGame2 = new TowerGame(launcher.towerGameDef, map2, launcher.pieceFactory);
                tower2Input = new RandomCommandProvider(1);
                
                gameScreen = launcher.gameScreenFactory.Create(towerGame1);
                gameScreen.SetActive(true);
                
                towerGame1.Start();
                towerGame2.Start();
                
                launcher.tickProvider.AddTickable(this);
            }
    
            public void Tick() {
                var cmd1 = tower1Input.GetNextCommand();
                if (cmd1 != null) {
                    towerGame1.ProcessCommand(cmd1.Value);
                }
    
                var cmd2 = tower2Input.GetNextCommand();
                if (cmd2 != null) {
                    towerGame2.ProcessCommand(cmd2.Value);
                }
                
                towerGame1.Tick();
                towerGame2.Tick();
                
                gameScreen.Update();
            }
            
            public void Dispose() {
                gameScreen.Destroy();
                towerGame1.Dispose();
                towerGame2.Dispose();
                launcher.tickProvider.RemoveTickable(this);
            }
        }
    
    }
}