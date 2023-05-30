using Hwdtech;
namespace SpaceBattle.Lib;

public class HardStopCommandStrategy : IStrategy
{
    public object RunStrategy(params object[] args)
    {
        var id = (string)args[0];
        Action? action;

        if (args.Count() < 2)
            action = null;
        else
            action = (Action)args[1];

        ServerThread sThread = IoC.Resolve<ServerThread>("GetThread", id);
        ICommand hStopCommand = new HardStopCommand(sThread);

        Action act = () =>
        {
            hStopCommand.Execute();
            if (action != null)
                action();
        };
        ICommand returnCmd = IoC.Resolve<ICommand>("SendCommand", id, new ActionCommand(act));
        return returnCmd;
    }
}
