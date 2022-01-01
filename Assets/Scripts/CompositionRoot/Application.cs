using System;
using MiniBricks.Controllers;
using MiniBricks.Game;
using MiniBricks.UI;
using MiniBricks.Utils;
using UnityEngine;

namespace MiniBricks.CompositionRoot {
    public class Application : MonoBehaviour {
        [SerializeField]
        private GameDef gameDef;
        
        private void Awake() {
            var windowManager = WindowManager.Create("Settings/UI");
            windowManager.SetCurrent();
            
            var pieceFactory = new PieceFactory(gameDef.Piece);

            var lobbyController = new LobbyController();

            var mainScreenFactory = new MainScreenFactory(lobbyController);
            var pauseScreenFactory = new PauseWindowFactory(lobbyController);
            var gameScreenFactory = new GameScreenFactory(pauseScreenFactory);
            
            var tickProvider = gameObject.AddComponent<TickProvider>();
            
            var trainingGameLauncher = new TrainingGameLauncher(gameDef.TowerGame, pieceFactory, tickProvider, gameScreenFactory);
            lobbyController.AddGameLauncher(trainingGameLauncher);

            var battleGameLauncher = new BattleGameLauncher(gameDef.TowerGame, pieceFactory, tickProvider, gameScreenFactory);
            lobbyController.AddGameLauncher(battleGameLauncher);
            
            var mainScreen = mainScreenFactory.Create();
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