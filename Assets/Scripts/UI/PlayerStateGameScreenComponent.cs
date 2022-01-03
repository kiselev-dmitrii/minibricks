using MiniBricks.Tetris;
using NData;

namespace MiniBricks.UI {
    public class PlayerStateGameScreenComponent : GameScreenComponent {
        public override string Path => "UI/GameScreen/PlayerStateGameScreenComponent";
        
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
        
        private readonly TowerGame tower;
        private readonly PauseWindowFactory pauseWindowFactory;
        
        public PlayerStateGameScreenComponent(TowerGame tower, PauseWindowFactory pauseWindowFactory) {
            this.tower = tower;
            this.pauseWindowFactory = pauseWindowFactory;

            tower.NumLivesChanged += OnNumLivesChanged;
            tower.HeightChanged += OnTowerHeightChanged;
            
            OnTowerHeightChanged(tower);
            OnNumLivesChanged(tower);
        }

        public override void Dispose() {
            tower.NumLivesChanged -= OnNumLivesChanged;
            tower.HeightChanged -= OnTowerHeightChanged;
        }

        #region Handlers
        public void OnPauseButtonClick() {
            var window = pauseWindowFactory.Create(tower);
            window.SetActive(true);
        }
        
        private void OnTowerHeightChanged(TowerGame _) {
            Height = tower.GetMaxHeight();
        }

        private void OnNumLivesChanged(TowerGame _) {
            NumLives = tower.GetNumLives();
        }
        #endregion
    }
}