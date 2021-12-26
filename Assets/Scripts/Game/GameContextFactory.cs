using MiniBricks.Game;
using MiniBricks.Game.Commands;
using MiniBricks.Tetris;
using UnityEngine;

namespace MiniBricks.Controllers {
    public class PieceFactory : IPieceFactory {
        private readonly PieceDef pieceDef;

        public PieceFactory(PieceDef pieceDef) {
            this.pieceDef = pieceDef;
        }
            
        public Piece Create(Piece prefab, Vector2 position, Transform parent) {
            var result = GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
            result.Initialize(pieceDef);
            return result;
        }
    }

    public class GameContextFactory {
        private readonly TowerGameDef towerGameDef;
        private readonly IPieceFactory pieceFactory;

        public GameContextFactory(TowerGameDef towerGameDef, IPieceFactory pieceFactory) {
            this.towerGameDef = towerGameDef;
            this.pieceFactory = pieceFactory;
        }
        
        public GameContext Create() {
            var gameContext = new GameContext();

            var game = new GameObject("Game");
            gameContext.AddDisposeCallback(() => {
                Object.Destroy(game);
            });
            

            var mapPrefab = Resources.Load<Map>("Maps/Map01");
            var map = Object.Instantiate(mapPrefab);
            gameContext.AddDisposeCallback(() => {
                Object.Destroy(map.gameObject);
            });
            
            var towerGame = new TowerGame(towerGameDef, map, pieceFactory);
            gameContext.AddDisposable(towerGame);

            var commandProvider = game.AddComponent<KeyboardCommandProvider>();
            commandProvider.CommandEmitted += (command) => {
                towerGame.ProcessCommand(command);
            };
            
            towerGame.Start();
            

            return gameContext;
        }
    }
}