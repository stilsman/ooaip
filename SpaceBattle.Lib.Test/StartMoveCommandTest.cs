using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using Xunit;

namespace SpaceBattle.Lib.Test;

public class StartMoveCommandTest
{
    public StartMoveCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        var mockCommand = new Mock<ICommand>();
        mockCommand.Setup(a => a.Execute());
        var regStrategy = new Mock<IStrategy>();
        regStrategy.Setup(_s => _s.Execute(It.IsAny<object[]>())).Returns(mockCommand.Object);
    
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "General.SetProperty", (object[] args) => regStrategy.Object.Execute(args)).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Command.Move", (object[] args) => regStrategy.Object.Execute(args)).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Queue.Push", (object[] args) => regStrategy.Object.Execute(args)).Execute();
    }

    [Fact]
    public void PosTestStartMoveCommand()
    {
        var m = new Mock<IMoveCommandStartable>();
        m.SetupGet(a => a.UObject).Returns(new Mock<IUObject>().Object).Verifiable();
        m.SetupGet(a => a.dict).Returns(new Dictionary<string, object>() { { "speed", new Vector(It.IsAny<int>(), It.IsAny<int>()) } }).Verifiable();
        ICommand startMoveCommand = new StartMoveCommand(m.Object);
        startMoveCommand.Execute();
        m.Verify();
    }
    [Fact]
    public void NegTestStartMoveCommand_UnableToGetUObject()
    {
        var m = new Mock<IMoveCommandStartable>();
        m.SetupGet(a => a.UObject).Throws<Exception>().Verifiable();
        m.SetupGet(a => a.dict).Returns(new Dictionary<string, object>() { { "speed", new Vector(It.IsAny<int>(), It.IsAny<int>()) } }).Verifiable();
        ICommand startMoveCommand = new StartMoveCommand(m.Object);
        Assert.Throws<Exception>(() => startMoveCommand.Execute());
    }
    [Fact]
    public void NegTestStartMoveCommand_UnableToGetSpeed()
    {
        var m = new Mock<IMoveCommandStartable>();
        m.SetupGet(a => a.UObject).Returns(new Mock<IUObject>().Object).Verifiable();
        m.SetupGet(a => a.dict).Throws<Exception>().Verifiable();
        ICommand startMoveCommand = new StartMoveCommand(m.Object);
        Assert.Throws<Exception>(() => startMoveCommand.Execute());
    }
}
