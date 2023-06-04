namespace SpaceBattle.Lib;
using Hwdtech;

public class StopServerCommand : ICommand
{
    public void Execute()
    {   
        Dictionary<string, string> myThreads = IoC.Resolve<Dictionary<string, string>>("Thread.GetDictionary");
        foreach (string threadId in myThreads.Keys)
        {
            IoC.Resolve<ICommand>("Thread.HardStopTheThreads", threadId).Execute();
        }
    }
}
