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

        IoC.Resolve<IInjectable>(
            "Game.Commands.SetupProperty",
            command.obj
        ).Inject(IoC.Resolve<ICommand>("Game.Commands.Empty"));
    }
}
