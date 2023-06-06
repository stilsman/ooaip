using Hwdtech;
using Hwdtech.Ioc;
namespace SpaceBattle.Lib.Test;
using Xunit;
using Moq;

public class GameCommandTests
{   
    Dictionary<int, Dictionary<int, IStrategy>> exceptionDict = new ();
    Dictionary<int, IStrategy> exceptionNotFoundCommand = new ();
    Mock<IStrategy> exceptionNotFoundExcepetion = new ();
    Dictionary<string, IReceiver> dictReceivers = new();
    Dictionary<string, TimeSpan> dictTimes = new();
    public GameCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        var getTimeStrategy = new Mock<IStrategy>();
        getTimeStrategy.Setup(s => s.RunStrategy(It.IsAny<object[]>())).Returns((object[] args) => dictTimes[(string)args[0]]);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetTime", (object[] args) => getTimeStrategy.Object.RunStrategy(args)).Execute();

        var getReceiverStrategy = new Mock<IStrategy>();
        getReceiverStrategy.Setup(s => s.RunStrategy(It.IsAny<object[]>())).Returns((object[] args) => dictReceivers[(string)args[0]]);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetReceiver", (object[] args) => getReceiverStrategy.Object.RunStrategy(args)).Execute();

            
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.GetTree", (object[] args) => {
            return exceptionDict;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.NotFoundCommandSubTree", (object[] args) => {
            return exceptionNotFoundCommand;
        }).Execute();

        exceptionNotFoundExcepetion.Setup(x => x.RunStrategy()).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.NotFoundExceptionHandler", (object[] args) => {
            return exceptionNotFoundExcepetion.Object;
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ExceptionHandler", (object[] args) => {
            ICommand cmd = (ICommand)args[0];
            Exception exc = (Exception)args[1];

            var exceptionHadler = new ExceptionHandlerStrat().RunStrategy(cmd, exc);

            return exceptionHadler;
        }).Execute();
        
        var getScopeStrategy = new Mock<IStrategy>();
        getScopeStrategy.Setup(s => s.RunStrategy(It.IsAny<object[]>())).Returns((object[] args) => scope);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetScope", (object[] args) => getScopeStrategy.Object.RunStrategy(args)).Execute();  
    }

    [Fact]
    public void GameCommandTest()
    {
        var cmd1 = new Mock<ICommand>();
        cmd1.Setup(c => c.Execute()).Verifiable();
        var cmd2 = new Mock<ICommand>();
        cmd2.Setup(c => c.Execute()).Verifiable();
        var cmd3 = new Mock<ICommand>();
        cmd3.Setup(c => c.Execute()).Verifiable();

        var listCmds = new List<ICommand>() {cmd1.Object, cmd2.Object, cmd3.Object};

        dictReceivers.Add("1", new QueueAdapter(new Queue<ICommand>(listCmds)));
        dictTimes.Add("1", TimeSpan.FromSeconds(3));

        var game = new GameCommand("1");
        game.Execute();

        cmd1.Verify(c => c.Execute(), Times.Once());
        cmd2.Verify(c => c.Execute(), Times.Once());
        cmd3.Verify(c => c.Execute(), Times.Once());
    }
    
    [Fact]
    public void GameCommandExceptionTest()
    {   
        var cmd = new Mock<ICommand>();
        cmd.Setup(c => c.Execute()).Throws<Exception>().Verifiable();
        var listCmds = new List<ICommand>() {cmd.Object};

        dictReceivers.Add("2", new QueueAdapter(new Queue<ICommand>(listCmds)));
        dictTimes.Add("2", TimeSpan.FromSeconds(3));

        var game = new GameCommand("2");
        game.Execute();
        
        cmd.Verify();
    }

    [Fact]
    public void ExceptionHandlerTest()
    {
        int count = 0;
        var exc = new Exception("101");
        var cmd = new Mock<ICommand>();
        cmd.Setup(c => c.Execute()).Throws<Exception>(() => exc);

        var str = new Mock<IStrategy>();
        str.Setup(s => s.RunStrategy()).Callback(() => count++);

        exceptionNotFoundCommand.Add(exc.GetType().GetHashCode(), str.Object);

        IoC.Resolve<IStrategy>("ExceptionHandler", cmd.Object, exc).RunStrategy();

        cmd.Verify();
    }
}
