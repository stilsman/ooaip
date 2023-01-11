using Hwdtech;

namespace SpaceBattle.Lib;

public class CreateLongtermCommandStrategy : IStrategy
{
    public object RunStrategy(params object[] args)
    {
        var nameOfDependency = (string)args[0];
        IUObject obj = (IUObject)args[1];

        var mCommand = IoC.Resolve<ICommand>("Game.CreateMacroCommand", nameOfDependency, obj);

        var repeat_command = IoC.Resolve<ICommand>("Game.Commands.Repeat", mCommand);
        return IoC.Resolve<ICommand>("Game.Queue.PushBack", repeat_command);
    }
}
