namespace SpaceBattle.Lib;

public class ActionComand : ICommand
{
    Action action;

    public ActionComand(Action act)
    {
        this.action = act;
    }

    public void Execute()
    {
        action();
    }
}
