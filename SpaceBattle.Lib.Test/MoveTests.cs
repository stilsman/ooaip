using System;
using Xunit;
using Moq;
namespace SpaceBattle.Lib.Test;

public class MoveCommandTests
{
    [Fact]
    public void PosMove()
    {
        var movableMock = new Mock<IMovable>();
        ICommand move = new MoveCommand(movableMock.Object);
        movableMock.SetupGet<Vector>(m=>m.position).Returns(new Vector(1, 2));
        movableMock.SetupGet<Vector>(m=>m.velocity).Returns(new Vector(1, 2));

        Assert.Equal(new Vector(1,2), movableMock.Object.position);

        move.Execute();

        movableMock.VerifySet(m=>m.position = new Vector(2,4));
        //Assert.Equal(new Vector(2, 4), movableMock.Object.position);
    }
    [Fact]
    public void NegGetPos()
    {
        var movableMock = new Mock<IMovable>();
        movableMock.SetupGet<Vector>(m=>m.position).Throws<Exception>();
        movableMock.SetupGet<Vector>(m=>m.velocity).Returns(new Vector(1, 2));
        ICommand move = new MoveCommand(movableMock.Object);

        Assert.Throws<Exception>(() => move.Execute());

    }
    [Fact]
    public void NegGetVel()
    {
        var movableMock = new Mock<IMovable>();
        movableMock.SetupGet<Vector>(m=>m.velocity).Throws<Exception>();
        movableMock.SetupGet<Vector>(m=>m.position).Returns(new Vector(1, 2));
        ICommand move = new MoveCommand(movableMock.Object);

        Assert.Throws<Exception>(() => move.Execute());
    }
    [Fact]
    public void NegSetPos()
    {
        var movableMock = new Mock<IMovable>();
        movableMock.SetupGet<Vector>(m=>m.velocity).Returns(new Vector(1, 2));
        movableMock.SetupGet<Vector>(m=>m.position).Returns(new Vector(1, 2));
        movableMock.SetupSet(m => m.position = It.IsAny<Vector>()).Throws<Exception>();
        ICommand move = new MoveCommand(movableMock.Object);

        Assert.Throws<Exception>(() => move.Execute());
    }

}

