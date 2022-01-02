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
        private readonly GameScreenFactory gameScreenFactory;

        public TrainingGameLauncher(TowerGameDef towerGameDef, PieceFactory pieceFactory, 
                                    TickProvider tickProvider, GameScreenFactory gameScreenFactory) {
            this.towerGameDef = towerGameDef;
            this.pieceFactory = pieceFactory;
            this.tickProvider = tickProvider;
            this.gameScreenFactory = gameScreenFactory;
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
            
            public TrainingGameRunner(TrainingGameLauncher l) {
                this.l = l;
                var mapPrefab = Resources.Load<Map>("Maps/Map01");
                
                input = new KeyboardCommandProvider();
                map = Object.Instantiate(mapPrefab);
                game = new TowerGame(l.towerGameDef, map, l.pieceFactory);
            
                map.Camera.GetComponent<FollowCamera>().Initialize(game, map);
                
                gameScreen = l.gameScreenFactory.Create(game);
                gameScreen.SetActive(true);
                
                game.Start();
                
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
            }
        }
    }
}