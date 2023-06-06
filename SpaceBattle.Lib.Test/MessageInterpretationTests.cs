namespace SpaceBattle.Lib.Test;
using Xunit;
using Moq;
using Hwdtech;
using Hwdtech.Ioc;
using System.Collections.Generic;

public class MessageInterpretationTests
{
    bool startMoveSent = false;
    bool propertiesWereSet = false;
    bool objectWasGet = false;
    bool commandsWereSent = false;
    public MessageInterpretationTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root")));
        scope.Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SendCommand", (object[] args) =>
        {
            return new ActionCommand(() => { commandsWereSent = true; });
        }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Command.StartMovement", (object[] args) =>
        {
            startMoveSent = true;

            var m = new Mock<IMoveCommandStartable>();
            m.SetupGet(a => a.UObject).Returns(new Mock<IUObject>().Object).Verifiable();
            m.SetupGet(a => a.dict).Returns(new Dictionary<string, object>() { { "speed", new Vector(It.IsAny<int>(), It.IsAny<int>()) } }).Verifiable();
            return new StartMoveCommand(m.Object);
        }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "GetObjectById", (object[] args) =>
        {
            objectWasGet = true;
            var obj = new Mock<IUObject>();
            return obj.Object;
        }).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "SetPropertiesCommand", (object[] args) =>
        {
            return new ActionCommand(() => { propertiesWereSet = true; });
        }).Execute();


    }
    [Fact]
    public void InterpretationCommandTests()
    {
        var mockMessage = new Mock<IMessage>();


        mockMessage.SetupGet(m => m.type).Returns("StartMovement");
        mockMessage.SetupGet(m => m.gameId).Returns("asdfg");
        mockMessage.SetupGet(m => m.gameItemId).Returns("548");
        var properties = new Dictionary<string, object> { { "InititalVelocity", 2 }, { "InititialSmth", 10 } };
        mockMessage.SetupGet(m => m.properties).Returns(properties);

        var interpretationCommand = new InterpretationCommand(mockMessage.Object);

        interpretationCommand.Execute();

        Assert.True(propertiesWereSet);
        Assert.True(startMoveSent);
        Assert.True(commandsWereSent);
        Assert.True(objectWasGet);
    }
}
