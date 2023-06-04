namespace SpaceBattle.Lib;
using Hwdtech;
using System.Collections.Concurrent;

public class ServerProgram
{
    public static void Main(string[] args){
        int numOfThread = int.Parse(args[0]);

        Console.WriteLine("Launching server...");

        IoC.Resolve<ICommand>("Thread.ConsoleStartServer", numOfThread).Execute();
        
        Console.WriteLine("All threads are functioning");

        Console.WriteLine("Press any key to stop the server...");
        Console.Read();

        Console.WriteLine("Stopping server...");

        IoC.Resolve<ICommand>("Thread.ConsoleStopServer").Execute();

        Console.WriteLine("Exiting. Press any key to exit...");
        Console.Read();
    }
}
