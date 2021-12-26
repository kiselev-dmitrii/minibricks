using System;
using System.Collections.Generic;

namespace KiselevDmitry.Utils {
    public class Disposer {
        private readonly List<IDisposable> disposables;

        public Disposer() {
            disposables = new List<IDisposable>();
        }

        public T Add<T>(T disposable) where T : IDisposable {
            disposables.Add(disposable);
            return disposable;
        }

        public void Dispose() {
            foreach (var disposable in disposables) {
                disposable.Dispose();
            }
            disposables.Clear();
        }
    }
}