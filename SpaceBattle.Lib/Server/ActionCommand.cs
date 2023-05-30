namespace SpaceBattle.Lib;

public class ActionCommand : ICommand
{
    Action action;

    public ActionCommand(Action act)
    {
        this.action = act;
    }

    public void Execute()
    {
        action();
    }
}
