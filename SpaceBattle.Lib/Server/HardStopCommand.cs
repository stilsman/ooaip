using Hwdtech;
namespace SpaceBattle.Lib;

public class HardStopCommand : ICommand
{
    ServerThread stoppingThread;
    public HardStopCommand(ServerThread stoppingThread)
    {
        this.stoppingThread = stoppingThread;
    }

    public void Execute()
    {
        if (Thread.CurrentThread == stoppingThread.thread)
        {
            stoppingThread.Stop();
        }
        else
        {
            throw IoC.Resolve<Exception>("HardStopThreadException");
        }
    }
}
