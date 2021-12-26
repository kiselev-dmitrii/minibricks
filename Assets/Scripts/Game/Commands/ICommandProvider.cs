using System;

namespace MiniBricks.Game.Commands {
    public enum CommandType {
        Left,
        Right,
        Rotate,
        StartAccelerate,
        StopAccelerate
    }
    
    public interface ICommandProvider {
        event Action<CommandType> CommandEmitted;
    }
}