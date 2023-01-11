using Hwdtech;

namespace SpaceBattle.Lib;
public class ExceptionHandlerFindStrategy : IStrategy
{
    public object RunStrategy(params object[] argv)
    {
        ICommand command = (ICommand)argv[0];
        Exception exception = (Exception)argv[1];

        int commandTypeHash = command.GetType().GetHashCode();
        int exceptionTypeHash = exception.GetType().GetHashCode();

        var ExceptionHandlers = IoC.Resolve<Dictionary<int, Dictionary<int, IStrategy>>>("Handler.Exception");
        Dictionary<int, IStrategy>? exceptions;
        if (!ExceptionHandlers.TryGetValue(commandTypeHash, out exceptions))
        {
            exceptions = IoC.Resolve<Dictionary<int, IStrategy>>("Exception.Get.NotFoundCommand");
        }
        IStrategy? ExceptionHandler;
        exceptions.TryGetValue(exceptionTypeHash, out ExceptionHandler);
        if (!exceptions.TryGetValue(exceptionTypeHash, out ExceptionHandler))
        {
            return IoC.Resolve<IStrategy>("Exception.Get.NotFoundExcepetionHandler");
        }

        return ExceptionHandler;
    }
}