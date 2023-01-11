namespace SpaceBattle.Lib.Test;
using Hwdtech;
using Moq;
using Xunit;
using System.Collections.Generic;

public class TestExceptionFindHandler
{

    public TestExceptionFindHandler()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        Mock<IStrategy> mockStrategy = new Mock<IStrategy>();
        mockStrategy.Setup(x => x.RunStrategy()).Returns("strategy");

        Mock<IStrategy> mockNotCommandStrategy = new Mock<IStrategy>();
        mockNotCommandStrategy.Setup(x => x.RunStrategy()).Returns("not found command strategy");

        Mock<IStrategy> mockNotExceptionStrategy = new Mock<IStrategy>();
        mockNotExceptionStrategy.Setup(x => x.RunStrategy()).Returns("not found exception strategy");

        Dictionary<int, Dictionary<int, IStrategy>> dict = new() { { new Mock<SpaceBattle.Lib.ICommand>().Object.GetType().GetHashCode(), new Dictionary<int, IStrategy>() { { new Mock<System.Exception>().Object.GetType().GetHashCode(), mockStrategy.Object } } } };
        IoC.Resolve<ICommand>("IoC.Register", "Handler.Exception", (object[] args) =>
        {
            return dict;
        }).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "Exception.Get.NotFoundCommand", (object[] args) =>
        {
            return new Dictionary<int, IStrategy>() { { new Mock<System.Exception>().Object.GetType().GetHashCode(), mockNotCommandStrategy.Object } };
        }).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "Exception.Get.NotFoundExcepetionHandler", (object[] args) =>
        {
            return mockNotExceptionStrategy.Object;
        }).Execute();
    }

    [Fact]
    public void ExceptionFindHandlerFoundAllTestPositive()
    {
        Mock<SpaceBattle.Lib.ICommand> mockCommand = new Mock<SpaceBattle.Lib.ICommand>();
        Mock<System.Exception> mockException = new Mock<System.Exception>();

        IStrategy strategy = (IStrategy)new ExceptionHandlerFindStrategy().RunStrategy(mockCommand.Object, mockException.Object);
        Assert.Equal(strategy.RunStrategy(), "strategy");
    }

    [Fact]
    public void ExceptionFindHandlerTestNegative()
    {
        Mock<SpaceBattle.Lib.ICommand> mockCommand = new Mock<SpaceBattle.Lib.ICommand>();
        System.Exception exception = new System.Exception();

        IStrategy strategy = (IStrategy)new ExceptionHandlerFindStrategy().RunStrategy(mockCommand.Object, exception);


        Assert.Equal(strategy.RunStrategy(), "not found exception strategy");
    }

}