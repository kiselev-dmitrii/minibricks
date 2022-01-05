using KiselevDmitry.Utils;
using MiniBricks.Controllers;
using MiniBricks.Core.Launchers;
using MiniBricks.UI;
using MiniBricks.UI.Main;
using MiniBricks.Utils;
using UnityEngine;

namespace MiniBricks.CompositionRoot {
    public class Application : MonoBehaviour {
        [SerializeField]
        private ApplicationDef applicationDef;

        private Disposer disposer;
        
        private void Awake() {
            UnityEngine.Application.targetFrameRate = 60;
            
            disposer = new Disposer();
            
            var windowManager = WindowManager.Create("Settings/UI");
            windowManager.SetCurrent();
            
            var lobbyController = new LobbyController();

            var mainScreenFactory = new MainScreenFactory(lobbyController);
            var tickProvider = gameObject.AddComponent<TickProvider>();
            
            var trainingGameLauncher = new TrainingGameLauncher(applicationDef, tickProvider, lobbyController);
            lobbyController.AddGameLauncher(trainingGameLauncher);

            var battleGameLauncher = new BattleGameLauncher(applicationDef , tickProvider, lobbyController);
            lobbyController.AddGameLauncher(battleGameLauncher);

            var mainScreenWatcher = disposer.Add(new MainScreenWatcher(lobbyController, mainScreenFactory));
            mainScreenWatcher.Update();
        }

        private void OnDestroy() {
            disposer.Dispose();
        }
    }
}