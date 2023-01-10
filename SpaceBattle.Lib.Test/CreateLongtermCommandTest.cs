using System;
using Xunit;
using Moq;
using Hwdtech;
using Hwdtech.Ioc;
namespace SpaceBattle.Lib.Test;

public class CreateLongtermCommandTest
{
    public CreateLongtermCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }
    [Fact]
    public void CreateLongtermCommandStrategyTest()
    {

        var mockCommand = new Mock<ICommand>();
        mockCommand.Setup(x => x.Execute());

        var mockCreateMacroCommandStrategy = new Mock<IStrategy>();
        mockCreateMacroCommandStrategy.Setup(x => x.RunStrategy(It.IsAny<object[]>())).Returns(mockCommand.Object).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.CreateMacroCommand", (object[] args) => mockCreateMacroCommandStrategy.Object.RunStrategy(args)).Execute();
    
        var mockInjectStrategy = new Mock<IStrategy>();
        mockInjectStrategy.Setup(x => x.RunStrategy(It.IsAny<object[]>())).Returns(mockCommand.Object).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.Inject", (object[] args) => mockInjectStrategy.Object.RunStrategy(args)).Execute();
        
        var mockQueuePushStrategy = new Mock<IStrategy>();
        mockQueuePushStrategy.Setup(x => x.RunStrategy(It.IsAny<object[]>())).Returns(mockCommand.Object).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue.PushBack", (object[] args) => mockQueuePushStrategy.Object.RunStrategy(args)).Execute();

        var createLongtermCommand = new CreateLongtermCommandStrategy();

        var mockUObj = new Mock<IUObject>();
        createLongtermCommand.RunStrategy(It.IsAny<string>(), mockUObj.Object);

        mockCreateMacroCommandStrategy.Verify();
        mockInjectStrategy.Verify();
        mockQueuePushStrategy.Verify();

    }
}