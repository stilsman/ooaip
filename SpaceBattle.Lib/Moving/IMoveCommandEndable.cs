namespace SpaceBattle.Lib;

public interface IMoveCommandEndable
{
    ICommand command { get; }
    IUObject obj { get; }
    Queue<ICommand> queue { get; }
}
