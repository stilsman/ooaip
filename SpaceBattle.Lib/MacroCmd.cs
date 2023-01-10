using System;
namespace SpaceBattle.Lib;

public class MacroCmd : ICommand
{
    private IList<ICommand> listOfCommands;

    public MacroCmd(IList<ICommand> listOfCommands)
    {
        this.listOfCommands = listOfCommands;
    }

    public void Execute()
    {
        foreach (ICommand cmd in listOfCommands)
        {
            cmd.Execute();
        }
    }
}
