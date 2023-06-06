namespace SpaceBattle.Lib;

public class ActionCommand : ICommand
{
    Action action;

    public ActionCommand(Action action)
    {
        this.action = action;
    }
    public void Execute()
    {
        this.action();
    }
}
