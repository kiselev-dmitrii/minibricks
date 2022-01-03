using System;
using System.Collections.Generic;
using MiniBricks.Game.Commands;
using MiniBricks.Tetris;
using MiniBricks.Utils;
using NData;
using UnityEngine;

namespace MiniBricks.UI {
    public class EnemyStateGameScreenComponent : GameScreenComponent {
        public override string Path => "UI/GameScreen/EnemyStateGameScreenComponent";        
        
        #region Property LastMoves
        public Property<String> LastMovesProperty { get; } = new Property<String>();
        public String LastMoves {
            get => LastMovesProperty.GetValue();
            set => LastMovesProperty.SetValue(value);
        }
        #endregion
        
        #region Property Height
        public Property<int> HeightProperty { get; } = new Property<int>();
        public int Height {
            get => HeightProperty.GetValue();
            set => HeightProperty.SetValue(value);
        }
        #endregion
        
        #region Property NumLives
        public Property<int> NumLivesProperty { get; } = new Property<int>();
        public int NumLives {
            get => NumLivesProperty.GetValue();
            set => NumLivesProperty.SetValue(value);
        }
        #endregion
        
        #region Property CameraView
        public Property<Texture> CameraViewProperty { get; } = new Property<Texture>();
        public Texture CameraView {
            get => CameraViewProperty.GetValue();
            set => CameraViewProperty.SetValue(value);
        }
        #endregion
        
        private readonly TowerGame tower;
        private readonly Map map;
        private readonly RenderTexture renderTexture;

        private static readonly Dictionary<Type, String> commandRepresentations = new Dictionary<Type, String>() {
            { typeof(LeftCommand), "→" },
            { typeof(RightCommand), "←" },
            { typeof(RotateCommand), "↻" },
            { typeof(StartAccelerateCommand), "↓"}
        };

        public EnemyStateGameScreenComponent(TowerGame tower) {
            this.tower = tower;
            tower.CommandExecuted += OnTowerCommandExecuted;
            tower.HeightChanged += OnHeightChanged;
            tower.NumLivesChanged += OnNumLivesChanged;

            map = tower.GetMap();
            renderTexture = new RenderTexture(270, 480, 0, RenderTextureFormat.ARGB32);
            renderTexture.Create();
            map.Camera.targetTexture = renderTexture;
            CameraView = renderTexture;
            
            OnHeightChanged(tower);
            OnNumLivesChanged(tower);
        }

        public override void Dispose() {
            tower.CommandExecuted -= OnTowerCommandExecuted;
            tower.HeightChanged -= OnHeightChanged;
            tower.NumLivesChanged -= OnNumLivesChanged;

            map.Camera.targetTexture = null;
            renderTexture.Release();
        }
        
        private void OnTowerCommandExecuted(ICommand command) {
            var representation = commandRepresentations.Get(command.GetType());
            if (representation == null) {
                return;
            }

            LastMoves += representation;
            if (LastMoves.Length > 6) {
                LastMoves = LastMoves.Remove(0);
            }
        }
        
        private void OnHeightChanged(TowerGame _) {
            Height = tower.GetMaxHeight();
        }
        
        private void OnNumLivesChanged(TowerGame _) {
            NumLives = tower.GetNumLives();
        }
    }
}