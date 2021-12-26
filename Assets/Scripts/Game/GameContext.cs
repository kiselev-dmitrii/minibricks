using System;
using KiselevDmitry.Utils;

namespace MiniBricks.Controllers {
    public class GameContext : IDisposable {
        private readonly Disposer disposer;
        private Action onDispose;
        
        public GameContext() {
            disposer = new Disposer();
        }

        public void AddDisposable(IDisposable disposable) {
            disposer.Add(disposable);
        }

        public void AddDisposeCallback(Action callback) {
            onDispose += callback;
        }
        
        public void Dispose() {
            onDispose?.Invoke();
            disposer.Dispose();
        }
    }
}