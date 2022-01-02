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

        public GameScreen Create(TowerGame game) {
            return new GameScreen(game, pauseWindowFactory);
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
        
        private readonly TowerGame game;
        private readonly PauseWindowFactory pauseWindowFactory;
        
        public GameScreen(TowerGame game, PauseWindowFactory pauseWindowFactory) : base("UI/GameScreen/GameScreen") {
            this.game = game;
            this.pauseWindowFactory = pauseWindowFactory;
        }

        public void Update() {
            NumLives = game.GetNumLives();
            Height = Mathf.FloorToInt(game.GetMaxHeight());
        }
        
        public void OnPauseButtonClick() {
            var window = pauseWindowFactory.Create(game);
            window.SetActive(true);
        }
    }
}