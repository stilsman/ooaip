namespace SpaceBattle.Lib.Test;
using Moq;
using System.Collections.Generic;
using Hwdtech;
using Xunit;

public class WorkWithGameTest
{
    public WorkWithGameTest()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<ICommand>("Scopes.Current.Set", scope).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Game.GetTick", (object[] args) =>
        {
            return (object)100;
        }).Execute();
    }
    [Fact]
    public void Test_GameObjectsDeleteGet()
    {
        string gameItemId = "item1";
        var obj = new object();
        var objects = new Dictionary<string, object>()
        {
            { gameItemId, obj }
        };

        var result = new GetGameObjectStrategy().RunStrategy(objects, gameItemId);
        Assert.Equal(obj, result);

        new DeleteGameObjectCommand(objects, gameItemId).Execute();
        Assert.DoesNotContain(gameItemId, objects.Keys);
    }
    [Fact]
    public void Test_GameQueuePushAndPop()
    {
        var commandMock = new Mock<Lib.ICommand>();
        var commandQueueMock = new Mock<Queue<Lib.ICommand>>();

        new GameQueuePushCommand(commandQueueMock.Object, commandMock.Object).Execute();
        Assert.True(commandQueueMock.Object.Contains(commandMock.Object));

        var command = new GameQueuePopStrategy().RunStrategy(commandQueueMock.Object);
        Assert.Equal(command, commandMock.Object);
    }
}
