using System;
using System.Collections.Generic;
using KiselevDmitry.Utils;
using MiniBricks.Core.Logic;
using MiniBricks.Core.Logic.Commands;
using MiniBricks.Core.Logic.Interfaces;
using MiniBricks.Utils;
using NData;
using UnityEngine;

namespace MiniBricks.UI.Core {
    public class EnemyStateGameScreenComponent : GameScreenComponent {
        public override string Path => "UI/GameScreen/EnemyStateGameScreenComponent";        
        
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

        #region Property MoveHistory
        public Property<String> MoveHistoryProperty { get; } = new Property<String>();
        public String MoveHistory {
            get => MoveHistoryProperty.GetValue();
            set => MoveHistoryProperty.SetValue(value);
        }
        #endregion
        
        private readonly MiniBricks.Core.Logic.Game game;
        private readonly Tower tower;
        private const int maxMoveHistoryLength = 6;

        private static readonly Dictionary<Type, String> commandRepresentations = new Dictionary<Type, String>() {
            { typeof(LeftCommand), "→" },
            { typeof(RightCommand), "←" },
            { typeof(RotateCommand), "↻" },
            { typeof(StartAccelerateCommand), "↓"}
        };

        public EnemyStateGameScreenComponent(MiniBricks.Core.Logic.Game game, Tower tower, RenderTexture cameraOutput) {
            this.game = game;
            this.tower = tower;
        
            CameraView = cameraOutput;
            MoveHistory = String.Empty;
            
            OnTowerMaxHeightChanged(tower);
            OnTowerNumLivesChanged(tower);
            
            game.CommandExecuted += OnGameCommandExecuted;
            tower.MaxHeightChanged += OnTowerMaxHeightChanged;
            tower.NumLivesChanged += OnTowerNumLivesChanged;
        }

        public override void Dispose() {
            game.CommandExecuted -= OnGameCommandExecuted;
            tower.MaxHeightChanged -= OnTowerMaxHeightChanged;
            tower.NumLivesChanged -= OnTowerNumLivesChanged;
        }
        
        private void OnGameCommandExecuted(ICommand command) {
            if (command.TowerId != tower.Id) {
                return;
            }
            
            var representation = commandRepresentations.Get(command.GetType());
            if (representation == null) {
                return;
            }

            MoveHistory = MoveHistory.Insert(0, representation);
            int moveHistoryLength = MoveHistory.Length;
            if (moveHistoryLength > maxMoveHistoryLength) {
                MoveHistory = MoveHistory.Remove(moveHistoryLength-1, 1);
            }
        }
        
        private void OnTowerMaxHeightChanged(Tower _) {
            Height = Mathf.RoundToInt(tower.MaxHeight);
        }
        
        private void OnTowerNumLivesChanged(Tower _) {
            NumLives = tower.NumLives;
        }
    }
}