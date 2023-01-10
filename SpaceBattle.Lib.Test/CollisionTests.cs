using Xunit;
using Moq;
using Hwdtech;
using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Test;

public class CheckCollisionTest
{
    Mock<IStrategy> mockCheckCollisionStrategy = new Mock<IStrategy>();
    public CheckCollisionTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        var getPropertyStrategy = new GetPropertyStrategy();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.GetProperty", (object[] args) => getPropertyStrategy.RunStrategy(args)).Execute();

        var getDifferenceStrategy = new GetDifferenceStrategy();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.GetDifference", (object[] args) => getDifferenceStrategy.RunStrategy(args)).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.CheckCollision", (object[] args) => mockCheckCollisionStrategy.Object.RunStrategy(args)).Execute();
    }


    [Fact]
    public void CollisionCheckTest()
    {
        var obj1 = new Mock<IUObject>();
        var obj2 = new Mock<IUObject>();

        obj1.Setup(x => x.GetProperty("position")).Returns(new Vector(It.IsAny<int>(), It.IsAny<int>()));
        obj2.Setup(x => x.GetProperty("position")).Returns(new Vector(It.IsAny<int>(), It.IsAny<int>()));
        obj1.Setup(x => x.GetProperty("velocity")).Returns(new Vector(It.IsAny<int>(), It.IsAny<int>()));
        obj2.Setup(x => x.GetProperty("velocity")).Returns(new Vector(It.IsAny<int>(), It.IsAny<int>()));

        mockCheckCollisionStrategy.Setup(x => x.RunStrategy(It.IsAny<object[]>())).Returns(true).Verifiable();
        var checkCollision = new CheckCollision(obj1.Object, obj2.Object);

        Assert.Throws<Exception>(() => checkCollision.Execute());
        mockCheckCollisionStrategy.Verify();


        mockCheckCollisionStrategy.Setup(x => x.RunStrategy(It.IsAny<object[]>())).Returns(false).Verifiable();
        var checkCollisionFalse = new CheckCollision(obj1.Object, obj2.Object);
        checkCollisionFalse.Execute();

        mockCheckCollisionStrategy.Verify();
    }

    [Fact]
    public void GetDiffTest()
    {
        var obj1 = new Mock<IUObject>();
        var obj2 = new Mock<IUObject>();

        obj1.Setup(x => x.GetProperty("position")).Returns(new Vector(1, 1));
        obj2.Setup(x => x.GetProperty("position")).Returns(new Vector(2, 2));
        obj1.Setup(x => x.GetProperty("velocity")).Returns(new Vector(1, 1));
        obj2.Setup(x => x.GetProperty("velocity")).Returns(new Vector(2, 2));

        var getDifferenceStrategy = new GetDifferenceStrategy();
        Assert.Equal(new List<Vector>{new Vector(-1,-1), new Vector(-1,-1)},getDifferenceStrategy.RunStrategy(obj1.Object, obj2.Object));
    }


}