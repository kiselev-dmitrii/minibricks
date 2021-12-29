using System;
using MiniBricks.Controllers;
using MiniBricks.Game;
using MiniBricks.UI;
using MiniBricks.Utils;
using UnityEngine;

namespace MiniBricks {
    public class Application : MonoBehaviour {
        [SerializeField]
        private GameDef gameDef;
        
        private void Awake() {
            var pieceFactory = new PieceFactory(gameDef.Piece);

            var lobbyController = new LobbyController();
            var tickProvider = gameObject.AddComponent<TickProvider>();
            
            var trainingGameLauncher = new TrainingGameLauncher(gameDef.TowerGame, pieceFactory, lobbyController, tickProvider);
            lobbyController.AddGameLauncher(trainingGameLauncher);

            var battleGameLauncher = new BattleGameLauncher(gameDef.TowerGame, pieceFactory, tickProvider);
            lobbyController.AddGameLauncher(battleGameLauncher);
            
            var mainScreen = new MainScreen(lobbyController);
            mainScreen.SetActive(true);

            lobbyController.GameStateChanged += () => {
                switch (lobbyController.GameState) {
                    case GameState.Menu:
                        mainScreen.SetActive(true);
                        break;
                    case GameState.Starting:
                        mainScreen.SetActive(false);
                        break;
                    case GameState.InGame:
                        mainScreen.SetActive(false);
                        break;
                    case GameState.Leaving:
                        mainScreen.SetActive(true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }
    }
}