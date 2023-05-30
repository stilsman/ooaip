using Hwdtech;
namespace SpaceBattle.Lib;


public class SoftStopCommandStrategy : IStrategy
{
    public object RunStrategy(params object[] args)
    {
        var id = (string)args[0];
        Action? action;

        action = (args.Count() < 2) ? null : (Action)args[1];

        var mt = IoC.Resolve<ServerThread>("GetThread", id);
        ICommand SSCmd = new SoftStopCommand(id, mt);
        Action act = () =>
        {
            SSCmd.Execute();

            if (action != null)
            {
                var cmd = new ActionCommand(action);
                IoC.Resolve<ICommand>("SendCommand", id, cmd);
                cmd.Execute();
            }
        };
        
        return new ActionCommand(act);
    }
}
