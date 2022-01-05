using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KiselevDmitry.Utils;
using MiniBricks.Utils;

namespace MiniBricks.Controllers {
    public enum GameState {
        Menu,
        InGame
    }

    public enum GameType {
        Training,
        Battle
    }
    
    public interface IGameLauncher {
        GameType Type { get; }
        IDisposable Launch();
    }
    
    public class LobbyController {
        private readonly Dictionary<GameType, IGameLauncher> launchers;
        private IDisposable gameContext;
        
        public GameState GameState { get; private set; }
        public event Action GameStateChanged;
        
        public LobbyController() {
            launchers = new Dictionary<GameType, IGameLauncher>();
            GameState = GameState.Menu;
        }

        public void AddGameLauncher(IGameLauncher launcher) {
            launchers.Add(launcher.Type, launcher);
        }
        
        public void StartGame(GameType gameType) {
            var launcher = launchers.Get(gameType);
            if (launcher == null) {
                throw new ArgumentException($"There is no launcher of {gameType} game");
            }
            
            gameContext = launcher.Launch();
            GameState = GameState.InGame;
            GameStateChanged?.Invoke();
        }

        public void LeaveGame() {
            gameContext.Dispose();

            GameState = GameState.Menu;
            GameStateChanged?.Invoke();
        }
    }
}