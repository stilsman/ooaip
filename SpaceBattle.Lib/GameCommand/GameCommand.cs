using Hwdtech;
using System.Diagnostics;

namespace SpaceBattle.Lib;

public class GameCommand : ICommand
{
    private IReceiver receiver;
    private object scope;
    private Stopwatch time;

    public GameCommand(object scope, IReceiver receiver)
    {
        this.scope = scope;
        this.receiver = receiver;
        time = new Stopwatch();
    }
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();
        var gameTick = IoC.Resolve<int>("Game.GetTick");
        time.Start();
        while (time.ElapsedMilliseconds <= gameTick)
        {
            if (!receiver.isEmpty())
            {
                var cmd = this.receiver.Receive();
                try
                {
                    cmd.Execute();
                }
                catch (Exception err)
                {
                    var exceptinHandlerStrategy = IoC.Resolve<IStrategy>("Exception.FindHandlerStrategy", cmd, err);
                    exceptinHandlerStrategy.RunStrategy();
                }
            }
            else break;
        }
        time.Reset();
    }
}
