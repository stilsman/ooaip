using Hwdtech;
namespace SpaceBattle.Lib;

public class SoftStopCommand : ICommand
{
    ServerThread stoppingThread;
    string id;
    public SoftStopCommand(string id, ServerThread stoppingThread)
    {
        this.id = id;
        this.stoppingThread = stoppingThread;
    }
    public void Execute()
    {
        if (Thread.CurrentThread == stoppingThread.thread)
        {
            var receiver = IoC.Resolve<IReceiver>("GetReceiver", id);
            new UpdateBehaviourCommand(stoppingThread, () =>
            {
                if (receiver.isEmpty())
                {
                    stoppingThread.Stop();
                }
                else
                {
                    //stoppingThread.HandleCommand();
                    var cmd = receiver.Receive();
                    cmd.Execute();
                }

            }).Execute();
        }
        else
        {
            throw IoC.Resolve<Exception>("SoftStopThreadException");
        }
    }
}
