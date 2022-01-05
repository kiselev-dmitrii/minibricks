using System;
using System.Collections.Generic;
using MiniBricks.Core.Logic;
using MiniBricks.Core.Logic.Commands;
using MiniBricks.Core.Logic.Interfaces;
using NData;
using UnityEngine;

namespace MiniBricks.UI.Core {
    public enum ActionType {
        Right = 0,
        Left = 1,
        Rotate = 2
    }
    
    public class ActionItem : Context {
        #region Property Type
        public Property<int> TypeProperty { get; } = new Property<int>();
        public ActionType Type {
            get => (ActionType)TypeProperty.GetValue();
            set => TypeProperty.SetValue((int)value);
        }
        #endregion

        public ActionItem(ActionType type) {
            Type = type;
        }
    }
    
    public class EnemyStateGameScreenComponent : GameScreenComponent {
        public override string Path => "UI/GameScreen/EnemyStateGameScreenComponent/EnemyStateGameScreenComponent";        
        
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

        #region Collection MoveHistory
        public Collection<ActionItem> MoveHistory { get; } = new Collection<ActionItem>(false);
        #endregion
        
        private readonly Game game;
        private readonly Tower tower;
        private const int maxMoveHistoryLength = 4;

        private static readonly Dictionary<Type, ActionType> actionTypes = new Dictionary<Type, ActionType>() {
            { typeof(LeftCommand), ActionType.Left },
            { typeof(RightCommand), ActionType.Right },
            { typeof(RotateCommand), ActionType.Rotate },
        };

        public EnemyStateGameScreenComponent(Game game, Tower tower, RenderTexture cameraOutput) {
            this.game = game;
            this.tower = tower;
        
            CameraView = cameraOutput;
            
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

            if (!actionTypes.TryGetValue(command.GetType(), out var actionType)) {
                return;
            }
            
            MoveHistory.Add(new ActionItem(actionType));
            if (MoveHistory.Count > maxMoveHistoryLength) {
                MoveHistory.Remove(0);
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