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
            private readonly Map map1;
            private readonly GameSimulation game;
            private readonly GameScreen gameScreen;
            
            public TrainingGameRunner(TrainingGameLauncher l) {
                this.l = l;
                var mapPrefab = Resources.Load<Map>("Maps/Map01");
                
                var input = new KeyboardCommandProvider();
                
                map1 = Object.Instantiate(mapPrefab);
                game = new GameSimulation();
                game.AddFeature(new PhysicsFeature());
                game.AddFeature(new TetrisFeature(game, map1, l.towerGameDef, l.pieceFactory));
                game.AddFeature(new InputFeature(game, input));
                game.AddFeature(new EndGameFeature(game, l.towerGameDef));
            
                gameScreen = l.gameScreenFactory.Create(game.GetFeature<EndGameFeature>());
                gameScreen.SetActive(true);
                
                game.Start();
                
                l.tickProvider.AddTickable(this);
            }
    
            public void Tick() {
                game.Tick();
                gameScreen.Update();
            }
            
            public void Dispose() {
                gameScreen.Destroy();
                game.Dispose();
                l.tickProvider.RemoveTickable(this);
                GameObject.Destroy(map1.gameObject);
            }
        }
    }
}