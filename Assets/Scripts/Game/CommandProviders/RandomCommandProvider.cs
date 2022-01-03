using MiniBricks.Game.Commands;
using MiniBricks.Tetris;
using MiniBricks.Utils;
using UnityEngine;

namespace MiniBricks.Game.CommandProviders {
    public class RandomCommandProvider : ICommandProvider {
        private static readonly ICommand[] commands = {
            new LeftCommand(),
            new RightCommand(),
            new RotateCommand()
        };
        
        private readonly float delay;
        private float lastCommandTime;

        public RandomCommandProvider(float delay) {
            this.delay = delay;
            lastCommandTime = 0;
        }
        
        public ICommand GetNextCommand() {
            var curTime = Time.time;
            if (curTime - lastCommandTime > delay) {
                lastCommandTime = curTime;
                return commands.GetRandom();
            }

            return null;
        }
    }
}