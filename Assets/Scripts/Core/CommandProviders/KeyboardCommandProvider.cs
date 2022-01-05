using MiniBricks.Core.Logic.Commands;
using MiniBricks.Core.Logic.Interfaces;
using UnityEngine;

namespace MiniBricks.Core.CommandProviders {
    public class KeyboardCommandProvider : ICommandProvider {
        private readonly int towerId;

        public KeyboardCommandProvider(int towerId) {
            this.towerId = towerId;
        }
        
        public ICommand GetNextCommand() {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                return new LeftCommand(towerId);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                return new RightCommand(towerId);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                return new RotateCommand(towerId);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                return new StartAccelerateCommand(towerId);
            }
            if (Input.GetKeyUp(KeyCode.DownArrow)) {
                return new StopAccelerateCommand(towerId);
            }

            return null;
        }
    }
}