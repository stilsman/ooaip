using Hwdtech;
namespace SpaceBattle.Lib;

public class ServerThread
{
    public Thread thread;
    IReceiver queue;
    bool stop = false;
    Action strategy;

    public ServerThread(IReceiver queue)
    {
        this.queue = queue;
        strategy = () =>
        {
            HandleCommand();
        };

        thread = new Thread(() =>
        {
            while (!stop)
                strategy();
        });
    }

    internal void HandleCommand()
    {
        var cmd = queue.Receive();

        cmd.Execute();
    }

    internal void UpdateBehaviour(Action newBehaviour)
    {
        strategy = newBehaviour;
    }
    public void Stop()
    {
        stop = true;
    }
    public void Execute()
    {
        thread.Start();
    }
}
