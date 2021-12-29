using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MiniBricks.Utils;

namespace MiniBricks.Controllers {
    public enum GameState {
        Menu,
        Starting,
        InGame,
        Leaving
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
        
        public async Task StartGame(GameType gameType) {
            var launcher = launchers.Get(gameType);
            if (launcher == null) {
                throw new ArgumentException($"There is no launcher of {gameType} game");
            }
            
            GameState = GameState.Starting;
            GameStateChanged?.Invoke();

            await Task.Delay(1000);

            gameContext = launcher.Launch();
            GameState = GameState.InGame;
            GameStateChanged?.Invoke();
        }

        public async Task LeaveGame() {
            GameState = GameState.Leaving;
            GameStateChanged?.Invoke();
            
            await Task.Delay(1000);
            
            gameContext.Dispose();

            GameState = GameState.Menu;
            GameStateChanged?.Invoke();
        }
    }
}