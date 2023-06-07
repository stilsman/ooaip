namespace SpaceBattle.Lib;

class UpdateBehaviourCommand : ICommand
{
    Action newBehaviour;
    ServerThread thread;

    public UpdateBehaviourCommand(ServerThread thread, Action newBehaviour)
    {
        this.thread = thread;
        this.newBehaviour = newBehaviour;
    }
    public void Execute()
    {
        thread.UpdateBehaviour(newBehaviour);
    }
}
