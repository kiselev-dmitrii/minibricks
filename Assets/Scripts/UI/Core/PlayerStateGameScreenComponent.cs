using MiniBricks.Core.Logic;
using NData;
using UnityEngine;

namespace MiniBricks.UI.Core {
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
        
        private readonly Tower tower;
        private readonly PauseWindowFactory pauseWindowFactory;
        
        public PlayerStateGameScreenComponent(Tower tower, PauseWindowFactory pauseWindowFactory) {
            this.tower = tower;
            this.pauseWindowFactory = pauseWindowFactory;

            tower.NumLivesChanged += OnNumLivesChanged;
            tower.MaxHeightChanged += OnTowerMaxHeightChanged;
            
            OnTowerMaxHeightChanged(tower);
            OnNumLivesChanged(tower);
        }

        public override void Dispose() {
            tower.NumLivesChanged -= OnNumLivesChanged;
            tower.MaxHeightChanged -= OnTowerMaxHeightChanged;
        }

        #region Handlers
        public void OnPauseButtonClick() {
            var window = pauseWindowFactory.Create();
            window.SetActive(true);
        }
        
        private void OnTowerMaxHeightChanged(Tower _) {
            Height = Mathf.RoundToInt(tower.MaxHeight);
        }

        private void OnNumLivesChanged(Tower _) {
            NumLives = tower.NumLives;
        }
        #endregion
    }
}