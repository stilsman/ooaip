using System.Collections.Concurrent;
using Hwdtech;
namespace SpaceBattle.Lib;

public class CreateAndStartThreadStrategy : IStrategy
{
    public object RunStrategy(params object[] args)
    {
        var id = (string)args[0];
        Action? action;

        action = (args.Count() < 2) ? null : (Action)args[1];

        Action act = () =>
        {
            BlockingCollection<ICommand> queue = new BlockingCollection<ICommand>();
            IoC.Resolve<ICommand>("SetReceiver", id, queue).Execute();
            ServerThread thread = new ServerThread(IoC.Resolve<IReceiver>("GetReceiver", id));
            IoC.Resolve<ICommand>("SetSender", id, queue).Execute();
            IoC.Resolve<ICommand>("SetThread", id, thread).Execute();
            thread.Execute();

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
