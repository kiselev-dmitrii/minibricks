using System;
using System.Threading.Tasks;
using KiselevDmitry.Utils;
using MiniBricks.Utils;

namespace MiniBricks.Controllers {
    public enum GameState {
        Menu,
        Starting,
        InGame,
        Leaving
    }
    
    public class LobbyController {
        private readonly GameContextFactory gameContextFactory;
        private GameContext gameContext;
        
        public LobbyController(GameContextFactory gameContextFactory) {
            this.gameContextFactory = gameContextFactory;
            GameState = GameState.Menu;
        }
        
        public GameState GameState { get; private set; }
        public event Action GameStateChanged;

        public async Task StartGame() {
            GameState = GameState.Starting;
            GameStateChanged?.Invoke();

            await Task.Delay(1000);

            gameContext = gameContextFactory.Create();
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
    
    public class GameContext : IDisposable {
        private readonly Disposer disposer;
        private Action onDispose;
        
        public GameContext() {
            disposer = new Disposer();
        }

        public void RegisterDisposable(IDisposable disposable) {
            disposer.Add(disposable);
        }

        public void RegisterDisposable(Action callback) {
            onDispose += callback;
        }
        
        public void Dispose() {
            onDispose?.Invoke();
            disposer.Dispose();
        }
    }

    public class GameContextFactory {
        public GameContext Create() {
            var gameContext = new GameContext();
            
            
            return gameContext;
        }
    }
}