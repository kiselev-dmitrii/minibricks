using MiniBricks.Game.Commands;
using MiniBricks.Game.Entities;
using MiniBricks.Utils;
using UnityEngine;

namespace MiniBricks.Game.CommandProviders {
    public class RandomCommandProvider : ICommandProvider {     
        private readonly float delay;
        private readonly ICommand[] commands;
        
        private float lastCommandTime;

        public RandomCommandProvider(int towerId, float delay = 0.5f) {
            this.delay = delay;
            commands = new ICommand[] {
                new LeftCommand(towerId),
                new RightCommand(towerId),
                new RotateCommand(towerId)
            };
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