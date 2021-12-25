using System;
using UnityEngine;

namespace MiniBricks.Commands {
    public class KeyboardCommandProvider : MonoBehaviour, ICommandProvider {
        public event Action<CommandType> CommandEmitted;

        public void Update() {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                CommandEmitted?.Invoke(CommandType.Left);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                CommandEmitted?.Invoke(CommandType.Right);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                CommandEmitted?.Invoke(CommandType.Rotate);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                CommandEmitted?.Invoke(CommandType.StartAccelerate);
            }
            if (Input.GetKeyUp(KeyCode.DownArrow)) {
                CommandEmitted?.Invoke(CommandType.StopAccelerate);   
            }
        }
    }
}