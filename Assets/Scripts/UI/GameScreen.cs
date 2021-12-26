using MiniBricks.Tetris;
using MiniBricks.Utils;
using NData;
using UnityEngine;

namespace MiniBricks.UI {
    public class GameScreen : Window {
        private readonly TowerGame towerGame;

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
        
        public GameScreen(TowerGame towerGame) : base("UI/GameScreen/GameScreen") {
            this.towerGame = towerGame;
        }

        public void Update() {
            NumLives = 3 - towerGame.NumFalls;
            Height = Mathf.RoundToInt(towerGame.MaxHeight);
        }
        
        public void OnPauseButtonClick() {
            
        }
    }
}