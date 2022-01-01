using MiniBricks.Tetris;
using MiniBricks.Utils;
using NData;
using UnityEngine;

namespace MiniBricks.UI {
    public class GameScreenFactory {
        private readonly PauseWindowFactory pauseWindowFactory;

        public GameScreenFactory(PauseWindowFactory pauseWindowFactory) {
            this.pauseWindowFactory = pauseWindowFactory;
        }

        public GameScreen Create(TowerGame towerGame) {
            return new GameScreen(towerGame, pauseWindowFactory);
        }
    }
    
    public class GameScreen : Window {
        private readonly PauseWindowFactory pauseWindowFactory;
        
        #region Property NumLives
        public Property<int> NumLivesProperty { get; } = new Property<int>();
        public int NumLives {
            get => NumLivesProperty.GetValue();
            set => NumLivesProperty.SetValue(value);
        }
        #endregion
        
        #region Property Height
        public Property<int> HeightProperty { get; } = new Property<int>();
        public int Height {
            get => HeightProperty.GetValue();
            set => HeightProperty.SetValue(value);
        }
        #endregion
        
        private readonly TowerGame towerGame;
        
        public GameScreen(TowerGame towerGame, PauseWindowFactory pauseWindowFactory) : base("UI/GameScreen/GameScreen") {
            this.towerGame = towerGame;
            this.pauseWindowFactory = pauseWindowFactory;
        }

        public void Update() {
            NumLives = 3 - towerGame.NumFalls;
            Height = Mathf.RoundToInt(towerGame.MaxHeight);
        }
        
        public void OnPauseButtonClick() {
            var window = pauseWindowFactory.Create();
            window.SetActive(true);
        }
    }
}