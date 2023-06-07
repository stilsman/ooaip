using Hwdtech;
using Hwdtech.Ioc;
using System.Collections.Concurrent;
using Xunit;
using Moq;
namespace SpaceBattle.Lib.Test;

public class ExceptionThreadsTests
{
    Dictionary<string, ServerThread> dictThread;
    Dictionary<string, IReceiver> dictReceiver;
    Dictionary<string, ISender> dictSender;

    public ExceptionThreadsTests()
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
            var excThreads = new Mock<IStrategy>();
            excThreads.Setup(c => c.RunStrategy(It.IsAny<object[]>())).Returns((object[] args) => new Exception());
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "StopThreadException", (object[] args) => excThreads.Object.RunStrategy(args)).Execute();

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
    public void HardAndSoftStopThrowsExceptionTest()
    {
        IoC.Resolve<ICommand>("CreateAndStartThread", "8").Execute();
        IoC.Resolve<ICommand>("SendCommand", "8", new ServerThreadDependecies(dictThread, dictReceiver, dictSender)).Execute();

        var hardStopCmd = new HardStopCommand(IoC.Resolve<ServerThread>("GetThread", "8"));
        var softStopCmd = new SoftStopCommand("8", IoC.Resolve<ServerThread>("GetThread", "8"));


        Assert.Throws<Exception>(() => hardStopCmd.Execute());
        Assert.Throws<Exception>(() => softStopCmd.Execute());

        IoC.Resolve<ICommand>("HardStopThread", "8").Execute();

    }
}