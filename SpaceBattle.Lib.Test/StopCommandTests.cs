using Hwdtech;
using Hwdtech.Ioc;
using System.Collections.Concurrent;
using Xunit;
using Moq;

namespace SpaceBattle.Lib.Test;

public class StopCommandTests
{
    Dictionary<string, ServerThread> dictThread;
    Dictionary<string, IReceiver> dictReceiver;
    Dictionary<string, ISender> dictSender;

    public StopCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        this.dictThread = new Dictionary<string, ServerThread>();
        this.dictReceiver = new Dictionary<string, IReceiver>();
        this.dictSender = new Dictionary<string, ISender>();

        new ServerThreadDependecies(dictThread, dictReceiver, dictSender).Execute();
    }
    public class ServerThreadDependecies : ICommand
    {
        Dictionary<string, ServerThread> dictThreads;
        Dictionary<string, IReceiver> dictReceivers;
        Dictionary<string, ISender> dictSenders;
        public ServerThreadDependecies(Dictionary<string, ServerThread> dt, Dictionary<string, IReceiver> dr, Dictionary<string, ISender> ds)
        {
            this.dictThreads = dt;
            this.dictReceivers = dr;
            this.dictSenders = ds;
        }
        public void Execute()
        {
            var scope = IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root")));
            scope.Execute();

            var getDictThreadsStrategy = new Mock<IStrategy>();
            getDictThreadsStrategy.Setup(s => s.RunStrategy(It.IsAny<object[]>())).Returns((object[] args) => this.dictThreads);

            var getThreadStrategy = new Mock<IStrategy>();
            getThreadStrategy.Setup(s => s.RunStrategy(It.IsAny<object[]>())).Returns((object[] args) => this.dictThreads[(string)args[0]]);

            var setThreadStrategy = new Mock<IStrategy>();
            setThreadStrategy.Setup(c => c.RunStrategy(It.IsAny<object[]>())).Returns((object[] args) =>
            {
                var setThreadCommand = new Mock<ICommand>();
                setThreadCommand.Setup(c => c.Execute()).Callback(() => this.dictThreads.Add((string)args[0], (ServerThread)args[1]));
                return setThreadCommand.Object;
            });

            var getReceiverStrategy = new Mock<IStrategy>();
            getReceiverStrategy.Setup(c => c.RunStrategy(It.IsAny<object[]>())).Returns((object[] args) => this.dictReceivers[(string)args[0]]);

            var setReceiverStrategy = new Mock<IStrategy>();
            setReceiverStrategy.Setup(c => c.RunStrategy(It.IsAny<object[]>())).Returns((object[] args) =>
            {
                var setReceiverCommand = new Mock<ICommand>();
                setReceiverCommand.Setup(c => c.Execute()).Callback(() => this.dictReceivers.Add((string)args[0], new ReceiverAdapter((BlockingCollection<ICommand>)args[1])));
                return setReceiverCommand.Object;
            });

            var getSenderStrategy = new Mock<IStrategy>();
            getSenderStrategy.Setup(c => c.RunStrategy(It.IsAny<object[]>())).Returns((object[] args) => this.dictSenders[(string)args[0]]);

            var setSenderStrategy = new Mock<IStrategy>();
            setSenderStrategy.Setup(c => c.RunStrategy(It.IsAny<object[]>())).Returns((object[] args) =>
            {
                var setSenderCommand = new Mock<ICommand>();
                setSenderCommand.Setup(c => c.Execute()).Callback(() => this.dictSenders.Add((string)args[0], new SenderAdapter((BlockingCollection<ICommand>)args[1])));
                return setSenderCommand.Object;
            });

            var exceptionStrategy = new Mock<IStrategy>();
            exceptionStrategy.Setup(c => c.RunStrategy(It.IsAny<object[]>())).Returns((object[] args) => new Exception());
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "StopThreadException", (object[] args) => exceptionStrategy.Object.RunStrategy(args)).Execute();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetDict", (object[] args) => getDictThreadsStrategy.Object.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SetThread", (object[] args) => setThreadStrategy.Object.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetThread", (object[] args) => getThreadStrategy.Object.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SetReceiver", (object[] args) => setReceiverStrategy.Object.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetReceiver", (object[] args) => getReceiverStrategy.Object.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SetSender", (object[] args) => setSenderStrategy.Object.RunStrategy(args)).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetSender", (object[] args) => getSenderStrategy.Object.RunStrategy(args)).Execute();

            var createAndStartStrategy = new CreateAndStartThreadStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateAndStartThread", (object[] args) => createAndStartStrategy.RunStrategy(args)).Execute();

            var hardStopCmdStrategy = new HardStopCommandStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "HardStopThread", (object[] args) => hardStopCmdStrategy.RunStrategy(args)).Execute();

            var softStopCmdStrategy = new SoftStopCommandStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SoftStopThread", (object[] args) => softStopCmdStrategy.RunStrategy(args)).Execute();

            var sendCmdStrategy = new SendCommandStrategy();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SendCommand", (object[] args) => sendCmdStrategy.RunStrategy(args)).Execute();
        }
    }

    [Fact]
    public void SoftStopCommandTests()
    {
        ManualResetEvent waitHandler = new ManualResetEvent(false);
        string tId = "1";
        var cmd1 = new Mock<ICommand>();
        cmd1.Setup(c => c.Execute()).Callback(() => waitHandler.Set()).Verifiable();
        IoC.Resolve<ICommand>("CreateAndStartThread", tId).Execute();
        var st = new ServerThreadDependecies(dictThread, dictReceiver, dictSender);
        IoC.Resolve<ICommand>("SendCommand", tId, st).Execute();
        var count = 0;
        var SSCmd = IoC.Resolve<ICommand>("SoftStopThread", tId, () => { count += 1; });
        IoC.Resolve<ICommand>("SendCommand", tId, SSCmd).Execute();
        IoC.Resolve<ICommand>("SendCommand", tId, cmd1.Object).Execute();

        waitHandler.WaitOne();
        var rc = IoC.Resolve<IReceiver>("GetReceiver", tId);
        Assert.True(rc.isEmpty());
        Assert.Equal(1, count);

    }

    [Fact]
    public void HardStopCommandTest()
    {
        AutoResetEvent waitHandler = new AutoResetEvent(false);

        var cmd1 = new Mock<ICommand>();
        cmd1.Setup(c => c.Execute()).Verifiable();
        var cmd2 = new Mock<ICommand>();
        cmd2.Setup(c => c.Execute()).Verifiable();
        var cmd3 = new Mock<ICommand>();
        cmd3.Setup(c => c.Execute()).Verifiable();

        string tId = "2";
        

        IoC.Resolve<ICommand>("CreateAndStartThread", tId).Execute();
        var st = new ServerThreadDependecies(dictThread, dictReceiver, dictSender);
        IoC.Resolve<ICommand>("SendCommand", tId, st).Execute();
        waitHandler.Set();
        IoC.Resolve<ICommand>("SendCommand", tId, cmd1.Object).Execute();
        IoC.Resolve<ICommand>("SendCommand", tId, cmd2.Object).Execute();
        IoC.Resolve<ICommand>("HardStopThread", tId).Execute();
        IoC.Resolve<ICommand>("SendCommand", tId, cmd3.Object).Execute();

        waitHandler.WaitOne();
        cmd3.Verify(c => c.Execute(), Times.Never());
        
    }


    [Fact]
    public void ExceptionTest()
    {
        string tId = "3";
        IoC.Resolve<ICommand>("CreateAndStartThread", tId).Execute();


        var hardStopCmd = new HardStopCommand(IoC.Resolve<ServerThread>("GetThread", tId));
        var softStopCmd = new SoftStopCommand(tId, IoC.Resolve<ServerThread>("GetThread", tId));


        Assert.Throws<Exception>(() => hardStopCmd.Execute());
        Assert.Throws<Exception>(() => softStopCmd.Execute());

        IoC.Resolve<ICommand>("HardStopThread", tId).Execute();

    }

    [Fact]
    public void CreateAndStartThreadStrategyTest()
    {
        string tId = "4";
        int count = 0;
        var act = new Action(() => { count += 1; });

        IoC.Resolve<ICommand>("CreateAndStartThread", tId, act).Execute();
        var st = new ServerThreadDependecies(dictThread, dictReceiver, dictSender);
        IoC.Resolve<ICommand>("SendCommand", tId, st).Execute();
        var thread = IoC.Resolve<ServerThread>("GetThread", tId);

        Assert.True(thread.thread.IsAlive);
        Assert.True(count == 1);

    }
}
