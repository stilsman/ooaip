using Hwdtech;

namespace SpaceBattle.Lib;

public class StartMoveCommand : ICommand
{
    IMoveCommandStartable obj { get; }

    public StartMoveCommand(IMoveCommandStartable UObject)
    {
        obj = UObject;
    }

    public void Execute()
    {
        obj.dict.ToList().ForEach(o => IoC.Resolve<ICommand>("General.SetProperty", obj.UObject, o.Key, o.Value).Execute());
        ICommand MCommand = IoC.Resolve<ICommand>("Command.Move", obj.UObject);
        IoC.Resolve<ICommand>("General.SetProperty", obj.UObject, "Commands.Movement", MCommand).Execute();
        IoC.Resolve<ICommand>("Queue.Push", MCommand).Execute();
    }
}