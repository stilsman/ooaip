using Hwdtech;
namespace SpaceBattle.Lib;

public class SendCommandStrategy : IStrategy
{
    public object RunStrategy(params object[] args)
    {
        string id = (string)args[0];
        ICommand ic = (ICommand)args[1];
        ISender s = IoC.Resolve<ISender>("GetSender", id);
        Action act = () =>
        {
            s.Send(ic);
        };
        return new ActionCommand(act);
    }
}
