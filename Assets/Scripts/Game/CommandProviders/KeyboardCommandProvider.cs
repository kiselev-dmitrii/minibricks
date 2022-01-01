using MiniBricks.Game.Commands;
using MiniBricks.Tetris;
using UnityEngine;

namespace MiniBricks.Game.CommandProviders {
    public class KeyboardCommandProvider : ICommandProvider {
        public ICommand GetNextCommand() {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                return new LeftCommand();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                return new RightCommand();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                return new RotateCommand();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                return new StartAccelerateCommand();
            }
            if (Input.GetKeyUp(KeyCode.DownArrow)) {
                return new StopAccelerateCommand();
            }

            return null;
        }
    }
}