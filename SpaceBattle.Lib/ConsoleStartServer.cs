namespace SpaceBattle.Lib;
using Hwdtech;
using System.Collections.Concurrent;

public class StartServerCommand : ICommand
{
    private int numOfThread;
    public StartServerCommand(int numOfThread)
    {
        this.numOfThread = numOfThread;
    }
    public void Execute()
    {
        for (int i = 0; i < numOfThread; i++)
        {   
            IoC.Resolve<ICommand>("Thread.CreateAndStartThread").Execute();
        }
    }
}
