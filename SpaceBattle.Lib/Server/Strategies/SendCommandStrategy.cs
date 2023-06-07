using Hwdtech;
namespace SpaceBattle.Lib;

public class SendCommandStrategy : IStrategy
{
    public object RunStrategy(params object[] args)
    {
        string id = (string)args[0];
        ICommand commands = (ICommand)args[1];
        ISender sender = IoC.Resolve<ISender>("GetSender", id);
        Action action = () =>
        {
            sender.Send(commands);
        };
        return new ActionComand(action);
    }
}
