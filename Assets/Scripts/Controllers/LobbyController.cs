using System;
using System.Threading.Tasks;

namespace MiniBricks.Controllers {
    public enum GameState {
        Menu,
        Starting,
        InGame,
        Leaving
    }
    
    public class LobbyController {
        private GameContextFactory gameContextFactory;
        private GameContext gameContext;
        
        public LobbyController() {
            GameState = GameState.Menu;
        }
        
        public GameState GameState { get; private set; }
        public event Action GameStateChanged;

        public void SetGameContextFactory(GameContextFactory value) {
            gameContextFactory = value;
        }
        
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
}