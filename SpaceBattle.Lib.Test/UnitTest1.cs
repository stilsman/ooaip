using System;
using Xunit;
using Moq;
namespace SpaceBattle.Lib.Test;

public class MoveCommandTests
{
    [Fact]
    public void TestPosMove()
    {
        var movableMock = new Mock<IMovable>();
        movableMock.SetupGet<Vector>(m=>m.position).Returns(new Vector(1, 2));

        Assert.Equal(new Vector(1,2), movableMock.Object.position);

        ICommand c = new MoveCommand(movableMock.Object);
        c.Execute();

        Assert.Equal(new Vector(2, 4), movableMock.Object.position);
    }
}

