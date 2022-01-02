using System;
using MiniBricks.Controllers;
using MiniBricks.Tetris;
using MiniBricks.Utils;
using NData;

namespace MiniBricks.UI {
    public class GameOverWindowFactory {
        private readonly LobbyController lobbyController;

        public GameOverWindowFactory(LobbyController lobbyController) {
            this.lobbyController = lobbyController;
        }
        
        public GameOverWindow Create(TowerGame game) {
            return new GameOverWindow(game, lobbyController);
        }
    }

    public class UserItem : Context {
        #region Property Name
        public Property<String> NameProperty { get; } = new Property<String>();
        public String Name {
            get => NameProperty.GetValue();
            set => NameProperty.SetValue(value);
        }
        #endregion
        
        #region Property NumFalls
        public Property<int> NumFallsProperty { get; } = new Property<int>();
        public int NumFalls {
            get => NumFallsProperty.GetValue();
            set => NumFallsProperty.SetValue(value);
        }
        #endregion
        
        #region Property Height
        public Property<int> HeightProperty { get; } = new Property<int>();
        public int Height {
            get => HeightProperty.GetValue();
            set => HeightProperty.SetValue(value);
        }
        #endregion
        
        #region Property IsPlayer
        public Property<bool> IsPlayerProperty { get; } = new Property<bool>();
        public bool IsPlayer {
            get => IsPlayerProperty.GetValue();
            set => IsPlayerProperty.SetValue(value);
        }
        #endregion

        public UserItem(String name, int numFalls, int height, bool isPlayer) {
            Name = name;
            NumFalls = numFalls;
            Height = height;
            IsPlayer = isPlayer;
        }
    }
    
    public class GameOverWindow : Window {
        #region Property IsVictory
        public Property<bool> IsVictoryProperty { get; } = new Property<bool>();
        public bool IsVictory {
            get => IsVictoryProperty.GetValue();
            set => IsVictoryProperty.SetValue(value);
        }
        #endregion
        
        #region Collection Users
        public Collection<UserItem> Users = new Collection<UserItem>(false);
        #endregion
        
        private readonly LobbyController lobbyController;
        
        public GameOverWindow(TowerGame game, LobbyController lobbyController) : base("UI/GameOverWindow/GameOverWindow") {
            this.lobbyController = lobbyController;
            IsVictory = game.GetResult() == GameResult.Victory;
        }

        public void AddUser(String name, int falls, int height, bool isPlayer) {
            Users.Add(new UserItem(name, falls, height, isPlayer));
        }

        public void OnContinueButtonClick() {
            Destroy();
            lobbyController.LeaveGame();
        }
    }
}