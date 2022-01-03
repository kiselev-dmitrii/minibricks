using System;
using MiniBricks.Game;
using MiniBricks.Game.CommandProviders;
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
        private readonly LobbyController lobbyController;

        public BattleGameLauncher(TowerGameDef towerGameDef, PieceFactory pieceFactory, 
            TickProvider tickProvider, LobbyController lobbyController) {
            this.towerGameDef = towerGameDef;
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

            private readonly ICommandProvider input1;
            private readonly Map map1;
            private readonly TowerGame game1;

            private readonly ICommandProvider input2;
            private readonly Map map2;
            private readonly TowerGame game2;
            private readonly GameScreen gameScreen;
            
            public BattleGameRunner(BattleGameLauncher l) {
                this.l = l;
                var mapPrefab = Resources.Load<Map>("Maps/Map01");
                
                var pauseWindowFactory = new PauseWindowFactory(l.lobbyController);
                
                input1 = new KeyboardCommandProvider();
                map1 = Object.Instantiate(mapPrefab);
                game1 = new TowerGame(l.towerGameDef, map1, l.pieceFactory);
                map1.Camera.GetComponent<FollowCamera>().Initialize(game1, map1);

                input2 = new RandomCommandProvider(0.1f);
                map2 = Object.Instantiate(mapPrefab, Vector3.right*100, Quaternion.identity);
                game2 = new TowerGame(l.towerGameDef, map2, l.pieceFactory);
                map2.Camera.GetComponent<FollowCamera>().Initialize(game2, map2);
                
                gameScreen = new GameScreen();
                gameScreen.AddComponent(new PlayerStateGameScreenComponent(game1, pauseWindowFactory));
                gameScreen.AddComponent(new EnemyStateGameScreenComponent(game2));
                gameScreen.SetActive(true);
                
                game1.Start();
                game2.Start();
                
                l.tickProvider.AddTickable(this);
            }
    
            public void Tick() {
                game1.AddCommand(input1.GetNextCommand());
                game1.Tick();
                
                game2.AddCommand(input2.GetNextCommand());
                game2.Tick();
            }
            
            public void Dispose() {
                gameScreen.Destroy();
                game1.Dispose();
                game2.Dispose();
                l.tickProvider.RemoveTickable(this);
                GameObject.Destroy(map1.gameObject);
                GameObject.Destroy(map2.gameObject);
            }
        }
    
    }
}