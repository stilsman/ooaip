namespace SpaceBattle.Lib.Test;
using System;
using Xunit;
using Moq;

public class RotateTest
{
    [Fact]
    public void PositiveTest()
    {
        var rotatableMock = new Mock<IRotatable>();
        rotatableMock.SetupProperty<Angle>(m => m.angular, new Angle (45, 1));
        rotatableMock.SetupGet<Angle>(m => m.angularVelocity).Returns(new Angle (90, 1));
        
        ICommand rotateCommand = new RotateCommand(rotatableMock.Object);

        rotateCommand.Execute();
        Assert.Equal(new Angle (135, 1),rotatableMock.Object.angular);



    }
    [Fact]
    public void setAngularException()
    {
        var rotatableMock = new Mock<IRotatable>();
        rotatableMock.SetupProperty(m => m.angular, new Angle(45, 1));
        rotatableMock.SetupGet<Angle>(m => m.angularVelocity).Returns(new Angle (90,1));
        rotatableMock.SetupSet<Angle>(m => m.angular = It.IsAny<Angle>()).Throws<Exception>();
        
        ICommand rotateCommand = new RotateCommand(rotatableMock.Object);

        Assert.Throws<Exception>(() => rotateCommand.Execute());
        

    }
    [Fact]
    public void getAngularException()
    {
        var rotatableMock = new Mock<IRotatable>();
        rotatableMock.SetupGet<Angle>(m => m.angularVelocity).Returns(new Angle (45, 1));
        rotatableMock.SetupSet<Angle>(m => m.angular = new Angle (90, 1));
        rotatableMock.SetupGet<Angle>(m => m.angular).Throws<Exception>();
        
        ICommand rotateCommand = new RotateCommand(rotatableMock.Object);

        Assert.Throws<Exception>(() => rotateCommand.Execute());

    }
    [Fact]
    public void getAngularVelocityException()
    {
        var rotatableMock = new Mock<IRotatable>();
        rotatableMock.SetupProperty<Angle>(m => m.angular, new Angle (90, 1));
        rotatableMock.SetupGet<Angle>(m => m.angularVelocity).Throws<Exception>();
        
        ICommand rotateCommand = new RotateCommand(rotatableMock.Object);
        Assert.Throws<Exception>(() => rotateCommand.Execute());
    }

}