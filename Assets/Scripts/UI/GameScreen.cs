using System;
using KiselevDmitry.NData.Bindings;
using MiniBricks.Utils;
using NData;

namespace MiniBricks.UI {
    public abstract class GameScreenComponent : Context, IPathContext, IDisposable {
        public abstract String Path { get; }

        public abstract void Dispose();
    }
    
    public class GameScreen : Window {
        #region Collection Components
        public Collection<GameScreenComponent> Components { get; } = new Collection<GameScreenComponent>(false);
        #endregion        
        
        public GameScreen() : base("UI/GameScreen/GameScreen") {
        }
        
        protected override void OnDestroy() {
            foreach (var component in Components) {
                component.Dispose();
            }
        }

        public void AddComponent(GameScreenComponent component) {
            Components.Add(component);
        }


    }
}