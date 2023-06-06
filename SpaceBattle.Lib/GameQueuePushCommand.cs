namespace SpaceBattle.Lib;

public class GameQueuePushCommand : ICommand
{
    Queue<ICommand> commandQueue;
    ICommand command;
    public GameQueuePushCommand(Queue<ICommand> commandQueue, ICommand command)
    {
        this.commandQueue = commandQueue;
        this.command = command;
    }
    public void Execute()
    {
        commandQueue.Enqueue(command);
    }
}
