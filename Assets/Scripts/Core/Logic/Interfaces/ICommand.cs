namespace MiniBricks.Core.Logic.Interfaces {
    public interface ICommand {
        int TowerId { get; }
        void Execute(Game game);
    }
}