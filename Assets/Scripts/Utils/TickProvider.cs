using System.Collections.Generic;
using UnityEngine;

namespace MiniBricks.Utils {
    public interface ITickable {
        public void Tick();
    }
    
    public class TickProvider : MonoBehaviour {
        private readonly List<ITickable> tickables;
        
        public TickProvider() {
            tickables = new List<ITickable>();
        }

        public void AddTickable(ITickable tickable) {
            tickables.Add(tickable);
        }

        public void RemoveTickable(ITickable tickable) {
            tickables.Remove(tickable);
        }

        private void Update() {
            for (int i = 0; i < tickables.Count; i++) {
                tickables[i].Tick();
            }
        }
    }
}