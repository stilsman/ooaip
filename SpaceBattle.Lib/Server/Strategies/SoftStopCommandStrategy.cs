using Hwdtech;
namespace SpaceBattle.Lib;


public class SoftStopCommandStrategy : IStrategy
{
    public object RunStrategy(params object[] args)
    {
        var id = (string)args[0];
        Action? action;

        if (args.Count() < 2)
            action = null;
        else
            action = (Action)args[1];

        var sThread = IoC.Resolve<ServerThread>("GetThread", id);
        ICommand sStopCommand = new SoftStopCommand(id, sThread);
        Action act = () =>
        {
            sStopCommand.Execute();

            if (action != null)
            {
                var command = new ActionCommand(action);
                IoC.Resolve<ICommand>("SendCommand", id, command);
                command.Execute();
            }
        };

        return new ActionCommand(act);
    }
}
