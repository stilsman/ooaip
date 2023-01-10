using Moq;
using Hwdtech;
using Hwdtech.Ioc;
using Xunit;
namespace SpaceBattle.Lib.Test;

public class BuildMacroCommandStrategyTests
{
    public BuildMacroCommandStrategyTests()
    {
        var command = new Mock<ICommand>();
        command.Setup(_c => _c.Execute());
        var PropStrat = new Mock<IStrategy>();
        PropStrat.Setup(_s => _s.RunStrategy(It.IsAny<object[]>())).Returns(command.Object);
        var StratList = new Mock<IStrategy>();
        StratList.Setup(_i => _i.RunStrategy()).Returns(new string[] { "Second" });

        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Operations.First", (object[] props) => StratList.Object.RunStrategy()).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Second", (object[] props) => PropStrat.Object.RunStrategy(props)).Execute();
    }
    [Fact]
    public void PosBuildMacroCommandStrategyTests()
    {
        var p = new Mock<IUObject>();
        var cs = new MacroCmdStrategy();

        var mc = (ICommand)cs.RunStrategy("First", p.Object);

        mc.Execute();
    }
}