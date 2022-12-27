namespace SpaceBattle.Lib;

public interface MoveCommandEndable
{
    ICommand command { get; }
    IUObject obj { get; }
    Queue<ICommand> queue { get; }
}
