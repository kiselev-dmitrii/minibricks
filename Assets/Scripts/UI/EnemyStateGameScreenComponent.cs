using System;
using System.Collections.Generic;
using MiniBricks.Controllers;
using MiniBricks.Game.Commands;
using MiniBricks.Game.Entities;
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

        private readonly MultiplayerGame game;
        private readonly Tower tower;

        private static readonly Dictionary<Type, String> commandRepresentations = new Dictionary<Type, String>() {
            { typeof(LeftCommand), "→" },
            { typeof(RightCommand), "←" },
            { typeof(RotateCommand), "↻" },
            { typeof(StartAccelerateCommand), "↓"}
        };

        public EnemyStateGameScreenComponent(MultiplayerGame game, Tower tower, RenderTexture cameraOutput) {
            this.game = game;
            this.tower = tower;
        
            CameraView = cameraOutput;
            
            OnTowerHeightChanged(tower);
            OnTowerNumLivesChanged(tower);
            
            game.CommandExecuted += OnGameCommandExecuted;
            tower.HeightChanged += OnTowerHeightChanged;
            tower.NumLivesChanged += OnTowerNumLivesChanged;
        }

        public override void Dispose() {
            game.CommandExecuted -= OnGameCommandExecuted;
            tower.HeightChanged -= OnTowerHeightChanged;
            tower.NumLivesChanged -= OnTowerNumLivesChanged;
        }
        
        private void OnGameCommandExecuted(ICommand command) {
            if (command.TowerId != tower.GetId()) {
                return;
            }
            
            var representation = commandRepresentations.Get(command.GetType());
            if (representation == null) {
                return;
            }

            LastMoves += representation;
            if (LastMoves.Length > 6) {
                LastMoves = LastMoves.Remove(0);
            }
        }
        
        private void OnTowerHeightChanged(Tower _) {
            Height = tower.GetMaxHeight();
        }
        
        private void OnTowerNumLivesChanged(Tower _) {
            NumLives = tower.GetNumLives();
        }
    }
}