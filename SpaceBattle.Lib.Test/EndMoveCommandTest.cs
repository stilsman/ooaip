using System;
using Xunit;
using Moq;
using Hwdtech;
using Hwdtech.Ioc;
namespace SpaceBattle.Lib.Test;

public class EndMoveCommandTest
{
    public EndMoveCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        var mockCommand = new Mock<ICommand>();
        mockCommand.Setup(x => x.Execute());

        var mockStrategyDelete = new Mock<IStrategy>();
        mockStrategyDelete.Setup(x => x.RunStrategy(It.IsAny<object[]>())).Returns(mockCommand.Object);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.DeleteProperty", (object[] args) => mockStrategyDelete.Object.RunStrategy(args)).Execute();

        var mockStrategyInject = new Mock<IStrategy>();
        mockStrategyInject.Setup(x => x.RunStrategy(It.IsAny<object[]>())).Returns(mockCommand.Object);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.Inject", (object[] args) => mockStrategyInject.Object.RunStrategy(args)).Execute();

        var mockStrategyEmpty = new Mock<IStrategy>();
        mockStrategyEmpty.Setup(x => x.RunStrategy()).Returns(mockCommand.Object);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.Empty", (object[] args) => mockStrategyEmpty.Object.RunStrategy(args)).Execute();

    }
    [Fact]
    public void EndMoveCommandPos()
    {
        var mockEndable = new Mock<IMoveCommandEndable>();
        var mockUObject = new Mock<IUObject>();
        mockEndable.SetupGet(x => x.obj).Returns(mockUObject.Object);

        var mockCommand = new Mock<ICommand>();
        mockEndable.SetupGet(x => x.command).Returns(mockCommand.Object);

        ICommand EndMoveCommand = new EndMoveCommand(mockEndable.Object);
        EndMoveCommand.Execute();
        mockEndable.VerifyAll();
    }

    [Fact]
    public void EndMoveCommandNeg()
    {
        var mockEndable = new Mock<IMoveCommandEndable>();
        var mockUObject = new Mock<IUObject>();
        var mockCommand = new Mock<ICommand>();

        mockEndable.SetupGet(x => x.obj).Throws<Exception>();
        ICommand EndMoveCommandUobjException = new EndMoveCommand(mockEndable.Object);
        Assert.Throws<Exception>(() => EndMoveCommandUobjException.Execute());

        mockEndable.SetupGet(x => x.obj).Returns(mockUObject.Object);
        mockEndable.SetupGet(x => x.command).Throws<Exception>();
        ICommand EndMoveCommandComException = new EndMoveCommand(mockEndable.Object);
        Assert.Throws<Exception>(() => EndMoveCommandComException.Execute());
    }
}
