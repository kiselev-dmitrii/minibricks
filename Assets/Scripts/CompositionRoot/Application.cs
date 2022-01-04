using System;
using KiselevDmitry.Utils;
using MiniBricks.Controllers;
using MiniBricks.Game;
using MiniBricks.UI;
using MiniBricks.Utils;
using UnityEngine;

namespace MiniBricks.CompositionRoot {
    public class Application : MonoBehaviour {
        [SerializeField]
        private GameDef gameDef;

        private Disposer disposer;
        
        private void Awake() {
            UnityEngine.Application.targetFrameRate = 60;
            
            disposer = new Disposer();
            
            var windowManager = WindowManager.Create("Settings/UI");
            windowManager.SetCurrent();
            
            var pieceFactory = new PieceFactory(gameDef.Piece);

            var lobbyController = new LobbyController();

            var mainScreenFactory = new MainScreenFactory(lobbyController);
            var tickProvider = gameObject.AddComponent<TickProvider>();
            
            var trainingGameLauncher = new TrainingGameLauncher(gameDef.MultiplayerGame, pieceFactory, tickProvider, lobbyController);
            lobbyController.AddGameLauncher(trainingGameLauncher);

            var battleGameLauncher = new BattleGameLauncher(gameDef.MultiplayerGame, pieceFactory, tickProvider, lobbyController);
            lobbyController.AddGameLauncher(battleGameLauncher);

            var mainScreenWatcher = disposer.Add(new MainScreenWatcher(lobbyController, mainScreenFactory));
            mainScreenWatcher.Update();
        }

        private void OnDestroy() {
            disposer.Dispose();
        }
    }
}