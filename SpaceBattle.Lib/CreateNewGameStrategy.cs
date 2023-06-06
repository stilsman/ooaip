namespace SpaceBattle.Lib;
using Hwdtech;

public class CreateNewGameStrategy : IStrategy
{
    public object RunStrategy(params object[] args)
    {   
        string gameId = (string) args[0];

        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));

        Queue<ICommand> commandQueue = new Queue<ICommand>();
        IReceiver receiver = new QueueAdapter(commandQueue);

        Dictionary<string, Queue<ICommand>> gamesDictionary = IoC.Resolve<Dictionary<string, Queue<ICommand>>>("Game.Get.GamesDictioanary");
        gamesDictionary.Add(gameId, commandQueue);

        return new GameCommand(scope, receiver);
    }
}
