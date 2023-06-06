using Hwdtech;

namespace SpaceBattle.Lib;

public class ExceptionHandlerStrat : IStrategy
{
    public object RunStrategy(params object[] args)
    {
        ICommand cmd = (ICommand)args[0];
        Exception exc = (Exception)args[1];

        int cmdHash = cmd.GetType().GetHashCode();
        int excHash = exc.GetType().GetHashCode();

        var excTree = IoC.Resolve<Dictionary<int, Dictionary<int, IStrategy>>>("Exception.GetTree");

        Dictionary<int, IStrategy>? excSubTree;

        if (!excTree.TryGetValue(cmdHash, out excSubTree))
        {
            excSubTree = IoC.Resolve<Dictionary<int, IStrategy>>("Exception.NotFoundCommandSubTree");
        }

        IStrategy? excHandler;

        if (!excSubTree.TryGetValue(excHash, out excHandler))
        {
            return IoC.Resolve<IStrategy>("Exception.NotFoundExceptionHandler");
        }
        return excHandler;
    }
}
