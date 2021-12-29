namespace MiniBricks.Game.Commands {
    public enum CommandType {
        Left,
        Right,
        Rotate,
        StartAccelerate,
        StopAccelerate
    }
    
    public interface ICommandProvider {
        public CommandType? GetNextCommand();
    }
}