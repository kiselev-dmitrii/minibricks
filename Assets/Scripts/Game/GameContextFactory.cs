using MiniBricks.Game;
using MiniBricks.Game.Commands;
using MiniBricks.Tetris;
using MiniBricks.UI;
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
        private readonly LobbyController lobbyController;

        public GameContextFactory(TowerGameDef towerGameDef, IPieceFactory pieceFactory, LobbyController lobbyController) {
            this.towerGameDef = towerGameDef;
            this.pieceFactory = pieceFactory;
            this.lobbyController = lobbyController;
        }
        
        public GameContext Create() {
            var gameContext = new GameContext();

            var gameObject = new GameObject("Game");
            gameContext.AddDisposeCallback(() => {
                Object.Destroy(gameObject);
            });
            
            var mapPrefab = Resources.Load<Map>("Maps/Map01");
            var map = Object.Instantiate(mapPrefab);
            gameContext.AddDisposeCallback(() => {
                Object.Destroy(map.gameObject);
            });
            
            var towerGame = new TowerGame(towerGameDef, map, pieceFactory);
            gameContext.AddDisposable(towerGame);

            var commandProvider = gameObject.AddComponent<KeyboardCommandProvider>();
            commandProvider.CommandEmitted += (command) => {
                towerGame.ProcessCommand(command);
            };

            var gameScreen = new GameScreen(towerGame);
            gameScreen.SetActive(true);
            
            towerGame.Start();
            
            gameObject.AddComponent<GameRunner>().Initialize(towerGame, gameScreen);
            
            return gameContext;
        }
    }

    public class GameRunner : MonoBehaviour {
        private TowerGame towerGame;
        private GameScreen gameScreen;
        
        public void Initialize(TowerGame towerGame, GameScreen gameScreen) {
            this.towerGame = towerGame;
            this.gameScreen = gameScreen;
        }
        
        public void Update() {
            towerGame.Tick();
            gameScreen.Update();
        }
    }
}