namespace SpaceBattle.Lib;
using Hwdtech;

public class DeleteGameCommand : ICommand
{
    private string gameId;
    public DeleteGameCommand(string gameId)
    {
        this.gameId = gameId;
    }
    public void Execute()
    {
        Dictionary<string, object> scopeMap = IoC.Resolve<Dictionary<string, object>>("Game.ScopeMap");
        scopeMap.Remove(gameId);
    }
}
