using System;

namespace MiniBricks.Commands {
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