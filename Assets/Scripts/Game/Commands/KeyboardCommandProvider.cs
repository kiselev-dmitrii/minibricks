using System;
using UnityEngine;

namespace MiniBricks.Game.Commands {
    public class KeyboardCommandProvider : ICommandProvider {
        public CommandType? GetNextCommand() {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                return CommandType.Left;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                return CommandType.Right;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                return CommandType.Rotate;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                return CommandType.StartAccelerate;
            }
            if (Input.GetKeyUp(KeyCode.DownArrow)) {
                return CommandType.StartAccelerate;
            }

            return null;
        }
    }
}