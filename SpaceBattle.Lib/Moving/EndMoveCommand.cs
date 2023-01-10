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
            "Move"
        ).Execute();

        IoC.Resolve<ICommand>(
            "Game.Commands.Inject",
            command.command,
            IoC.Resolve<ICommand>("Game.Commands.Empty")
        ).Execute();

    }
}
