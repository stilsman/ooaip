using System;
namespace SpaceBattle.Lib;
using Hwdtech;

public class MacroCmdStrategy : IStrategy
{
    public object RunStrategy(params object[] args)
    {
        string key = (string)args[0];
        IUObject obj = (IUObject)args[1];

        IList<string> dependencies = IoC.Resolve<IList<string>>("Operations." +key);

        IList<ICommand> commands = new List<ICommand>();
        foreach (string dep in dependencies)
        {
            commands.Add(IoC.Resolve<ICommand>(dep, obj));
        }

        return new MacroCmd(commands);
    }
}
