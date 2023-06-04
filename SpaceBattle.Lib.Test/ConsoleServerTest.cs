namespace SpaceBattle.Lib.Test;

using Xunit;
using Moq;
using System;
using System.IO;
using System.Collections.Generic;
using Hwdtech;

public class Test_ServerStart{
    public object globalScope;        
    int threadsStartCount = 0;
    int threadsStopCount = 0;
    bool starting = false;
    bool stopping = false;
    public Test_ServerStart() {

        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));

        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "Thread.CreateAndStartThread", (object[] args) => {
            return new ActionCommand( () => {threadsStartCount++;});
        }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Thread.GetDictionary", (object[] args) => {
            Dictionary<string, string> threads = new Dictionary<string, string>() { 
                {"123", "T111"}, 
                {"456", "T222"},
                {"789", "T333"}
            };
            return threads;
        }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Thread.HardStopTheThreads", (object[] args) => {
            return new ActionCommand( () => {threadsStopCount++;});
        }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Thread.ConsoleStartServer", (object[] args) => {
            starting = true;
            return new EmptyCommand();
        }).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "Thread.ConsoleStopServer", (object[] args) => {
            stopping = true;
            return new EmptyCommand();
        }).Execute();

        globalScope = scope;
        
    }
    [Fact]
    public void CreateAndStartThreadTest()
    {

        int numOfThread = 5;
        var startServerCommand = new StartServerCommand(numOfThread);
        startServerCommand.Execute();
        
        Assert.Equal(numOfThread, threadsStartCount);
    }
    [Fact]
    public void StopThreadTest()
    {
        Dictionary<string, string> myThreads = IoC.Resolve<Dictionary<string, string>>("Thread.GetDictionary");

        var StopServerCommand = new StopServerCommand();
        StopServerCommand.Execute();

        Assert.Equal(myThreads.Count, threadsStopCount);
    }
    [Fact]
    public void ConsoleTest()
    {
        int numOfThread = 3;
        var args = new[] { "3" }; 
        var consoleInput = new StringReader("sampletext~~");
        var consoleOutput = new StringWriter();
        var originalInput = Console.In;
        var originalOutput = Console.Out;
        Console.SetIn(consoleInput);
        Console.SetOut(consoleOutput);

        ServerProgram.Main(args);

        var output = consoleOutput.ToString();
        Console.SetIn(originalInput);
        Console.SetOut(originalOutput);
        
        Assert.Contains("Launching server..", output);
        IoC.Resolve<SpaceBattle.Lib.ICommand>("Thread.ConsoleStartServer", numOfThread).Execute();
        Assert.True(true == starting);
        Assert.Contains("All threads are functioning", output);
        Assert.Contains("Stopping server...", output);
        IoC.Resolve<SpaceBattle.Lib.ICommand>("Thread.ConsoleStopServer").Execute();
        Assert.True(true == stopping);
        Assert.Contains("Exiting. Press any key to exit...", output);
    }
    [Fact]
    public void ExceptionHandlerStrategyTest()
    {
        SpaceBattle.Lib.ICommand command = Mock.Of<SpaceBattle.Lib.ICommand>();
        Exception exception = new Exception("Test exception");
        string logFileName = "error.log";
        string errorMessage = $"Error in command '{command.GetType().Name}': {exception.Message}";

        var strategy = new ExceptionHandlerStrategy();

        strategy.RunStrategy(command, exception);

        Assert.True(File.Exists(logFileName));
        string[] lines = File.ReadAllLines(logFileName);
        Assert.Contains(errorMessage, lines[0]);
    }
}
