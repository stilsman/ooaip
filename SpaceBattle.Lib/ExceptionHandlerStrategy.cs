namespace SpaceBattle.Lib;


public class ExceptionHandlerStrategy : IStrategy
{   
    public object RunStrategy(params object[] args)
    {
        ICommand command = (ICommand)args[0];
        Exception ex = (Exception)args[1];
        string logFileName = "error.log";
        string errorMessage = $"Error in command '{command.GetType().Name}': {ex.Message}";

        using (StreamWriter writer = new StreamWriter(logFileName, true))
        {
            writer.WriteLine(errorMessage);
        }
        return true;
    }
}
