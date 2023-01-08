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

        var mockCommand = new Mock<SpaceBattle.Lib.ICommand>();
        mockCommand.Setup(x => x.Execute());

        var mockInjectable = new Mock<IInjectable>();
        mockInjectable.Setup(x => x.Inject(It.IsAny<SpaceBattle.Lib.ICommand>()));

        var mockStrategyCommand = new Mock<IStrategy>();
        mockStrategyCommand.Setup(x => x.RunStrategy(It.IsAny<object[]>())).Returns(mockCommand.Object);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.DeleteProperty", (object[] args) => mockStrategyCommand.Object.RunStrategy(args)).Execute();

        var mockStrategyInjectable = new Mock<IStrategy>();
        mockStrategyInjectable.Setup(x => x.RunStrategy(It.IsAny<object[]>())).Returns(mockInjectable.Object);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.SetupProperty", (object[] args) => mockStrategyInjectable.Object.RunStrategy(args)).Execute();

        var mockStrategyEmpty = new Mock<IStrategy>();
        mockStrategyEmpty.Setup(x => x.RunStrategy()).Returns(mockCommand.Object);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.Empty", (object[] args) => mockStrategyEmpty.Object.RunStrategy(args)).Execute();
    }
    [Fact]
    public void EndMoveCommandPos()
    {
        var mockEndable = new Mock<IMoveCommandEndable>();
        var mockUObject = new Mock<IUObject>();
        mockEndable.SetupGet(x => x.obj).Returns(mockUObject.Object).Verifiable();
        ICommand EndMoveCommand = new EndMoveCommand(mockEndable.Object);

        EndMoveCommand.Execute();
        mockEndable.Verify();
    }

}