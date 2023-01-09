using Hwdtech.Ioc;
using Hwdtech;
using Xunit;
using Moq;

namespace SpaceBattle.Lib.Test;

public class SolutionTreeTests
{
    public SolutionTreeTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    
        var mockStrategyReturnsDict = new Mock<IStrategy>();
        mockStrategyReturnsDict.Setup(x => x.Execute()).Returns(new Dictionary<int, object>());

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.GetSolutionTree", (object[] args) => mockStrategyReturnsDict.Object.Execute(args)).Execute();
    }

    [Fact]
    public void SuccesfulBuildSolutionTree()
    {
        var path = @"..\..\..\file.txt";
        var buildCommand = new BuildSolutionTreeCommand(path);

        buildCommand.Execute();

        var a = IoC.Resolve<IDictionary<int, object>>("Game.GetSolutionTree");
        
        Assert.True(a.ContainsKey(1));
        Assert.True(a.ContainsKey(4));
        Assert.True(a.ContainsKey(9));
        Assert.True(a.ContainsKey(22));

        Assert.True(((IDictionary<int, object>) a[22]).ContainsKey(37));
        Assert.True(((IDictionary<int, object>) a[1]).ContainsKey(3));

        Assert.True(((IDictionary<int, object>)((IDictionary<int, object>) a[1])[2]).ContainsKey(4));
    }  
}
