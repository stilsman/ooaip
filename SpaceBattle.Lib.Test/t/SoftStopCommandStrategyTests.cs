using Hwdtech;
using Hwdtech.Ioc;
using System.Collections.Concurrent;
using Xunit;
using Moq;
namespace SpaceBattle.Lib.Test;

public class SoftStopCommandStrategyTests
{
    Dictionary<string, ServerThread> dictThread;
    Dictionary<string, IReceiver> dictReceiver;
    Dictionary<string, ISender> dictSender;

    public SoftStopCommandStrategyTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        this.dictThread = new Dictionary<string, ServerThread>();
        this.dictReceiver = new Dictionary<string, IReceiver>();
        //dictReceivers.Add("5", new Mock<IReceiver>().Object);
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
            //dictSenders.Add("5", new Mock<ISender>().Object);


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
    public void SoftStopCommandStrategyWithActionTests()
    {
        ManualResetEvent mre = new ManualResetEvent(false);
        var cmd1 = new Mock<ICommand>();
        cmd1.Setup(c => c.Execute()).Callback(() => mre.Set()).Verifiable();
        IoC.Resolve<ICommand>("CreateAndStartThread", "5").Execute();
        IoC.Resolve<ICommand>("SendCommand", "5", new ServerThreadDependecies(dictThread, dictReceiver, dictSender)).Execute();

        var count = 0;
        var SSCmd = IoC.Resolve<ICommand>("SoftStopThread", "5", () => { count += 1; });
        IoC.Resolve<ICommand>("SendCommand", "5", SSCmd).Execute();
        IoC.Resolve<ICommand>("SendCommand", "5", cmd1.Object).Execute();

        mre.WaitOne();
        var rc = IoC.Resolve<IReceiver>("GetReceiver", "5");
        Assert.True(rc.isEmpty());
        Assert.Equal(1, count);

    }

    [Fact]
    public void SoftStopCommandStrategyWithoutActionTests()
    {
        ManualResetEvent mre = new ManualResetEvent(false);

        var cmd1 = new Mock<ICommand>();
        cmd1.Setup(c => c.Execute()).Callback(() => mre.Set()).Verifiable();

        IoC.Resolve<ICommand>("CreateAndStartThread", "6").Execute();
        IoC.Resolve<ICommand>("SendCommand", "6", new ServerThreadDependecies(dictThread, dictReceiver, dictSender)).Execute();

        var SSCmd = IoC.Resolve<ICommand>("SoftStopThread", "6");
        IoC.Resolve<ICommand>("SendCommand", "6", SSCmd).Execute();
        IoC.Resolve<ICommand>("SendCommand", "6", cmd1.Object).Execute();

        mre.WaitOne();
        cmd1.Verify(c => c.Execute(), Times.Once());

        var receiver = IoC.Resolve<IReceiver>("GetReceiver", "6");
        Assert.True(receiver.isEmpty());

    }
}