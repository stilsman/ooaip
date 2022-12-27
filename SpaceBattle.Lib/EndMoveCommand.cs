namespace SpaceBattle.Lib;
using Hwdtech;


public class EndMoveCommand : ICommand
{
    MoveCommandEndable command;
    public EndMoveCommand(MoveCommandEndable com)
    {
        command = com;
    }
    public void Execute()
    {

        Hwdtech.IoC.Resolve<ICommand>(
            "Game.Commands.DeleteProperty",
            command.obj,
            command.obj.GetProperty("Move")
        ).Execute();

        Hwdtech.IoC.Resolve<ICommand>(
            "Game.Queue.Enqeue",
            Hwdtech.IoC.Resolve<IInjectable>(
                "Game.Inject.Empty",
                command.obj,
                command.command
            ).Inject()
        );


    }
}
