using Hwdtech;
namespace SpaceBattle.Lib;

public class HardStopCommandStrategy : IStrategy
{
    public object RunStrategy(params object[] args)
    {
        var id = (string)args[0];
        Action? action;

        action = (args.Count() < 2) ? null : (Action)args[1];

        ServerThread mt = IoC.Resolve<ServerThread>("GetThread", id);
        ICommand HSCmd = new HardStopCommand(mt);

        Action act = () =>
        {

            HSCmd.Execute();
            if (action != null)
            {
                action();
            }
        };
        ICommand returnCmd = IoC.Resolve<ICommand>("SendCommand", id, new ActionCommand(act));
        return returnCmd;
    }
}
