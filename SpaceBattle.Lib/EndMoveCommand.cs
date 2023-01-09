namespace SpaceBattle.Lib;
using Hwdtech;

public class EndMoveCommand : ICommand
{
    IMoveCommandEndable command;
    public EndMoveCommand(IMoveCommandEndable com)
    {
        command = com;
    }
    public void Execute()
    {
        IoC.Resolve<ICommand>(
            "Game.Commands.DeleteProperty",
            command.obj,
            command.obj.GetProperty("Move")
        ).Execute();

        IoC.Resolve<ICommand>(
            "Game.Commands.Inject",
            command.command,
            command.queue,
            IoC.Resolve<ICommand>("Game.Commands.Empty")
        ).Execute();

    }
}
