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

        public GameScreen Create(GameSimulation gameSimulation) {
            return new GameScreen(gameSimulation, pauseWindowFactory);
        }
    }
    
    public class GameScreen : Window {       
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
        
        private readonly GameSimulation gameSimulation;
        private readonly PauseWindowFactory pauseWindowFactory;
        private readonly EndGameFeature endGameFeature;
        
        public GameScreen(GameSimulation gameSimulation, PauseWindowFactory pauseWindowFactory) : base("UI/GameScreen/GameScreen") {
            this.endGameFeature = gameSimulation.GetFeature<EndGameFeature>();
            this.gameSimulation = gameSimulation;
            this.pauseWindowFactory = pauseWindowFactory;
        }

        public void Update() {
            NumLives = endGameFeature.GetLives();
            Height = endGameFeature.GetHeight();
        }
        
        public void OnPauseButtonClick() {
            var window = pauseWindowFactory.Create(gameSimulation);
            window.SetActive(true);
        }
    }
}