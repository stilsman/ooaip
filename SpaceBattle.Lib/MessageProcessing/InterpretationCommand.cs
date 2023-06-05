namespace SpaceBattle.Lib;
using System.Collections.Concurrent;
using Hwdtech;

public class InterpretationCommand : ICommand
{
    private IMessage message;

    public InterpretationCommand(IMessage message)
    {
        this.message = message;
    }

    public void Execute(){
        var obj = IoC.Resolve<IUObject>("GetObjectById", message.gameItemId);
        IoC.Resolve<ICommand>("SetPropertiesCommand", obj, message.properties).Execute();

        var newCommand = IoC.Resolve<ICommand>("Command." + message.type, obj);
        
        IoC.Resolve<ICommand>("SendCommand", message.gameId, newCommand).Execute();
    }
}
