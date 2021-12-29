using System;
using System.Linq;
using MiniBricks.Utils;
using UnityEngine;

namespace MiniBricks.Game.Commands {
    public class RandomCommandProvider : ICommandProvider {
        private static readonly CommandType[] commands = Enum
            .GetValues(typeof(CommandType))
            .Cast<CommandType>()
            .ToArray();
        
        private readonly float delay;
        private float lastCommandTime;

        public RandomCommandProvider(float delay) {
            this.delay = delay;
            lastCommandTime = 0;
        }
        
        public CommandType? GetNextCommand() {
            var curTime = Time.time;
            if (curTime - lastCommandTime > delay) {
                lastCommandTime = curTime;
                return commands.GetRandom();
            }

            return null;
        }
    }
}