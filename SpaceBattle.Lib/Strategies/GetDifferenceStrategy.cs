using Hwdtech;
namespace SpaceBattle.Lib;

public class GetDifferenceStrategy : IStrategy
{
    public object RunStrategy(params object[] args)
    {
        var obj1 = (IUObject)args[0];
        var obj2 = (IUObject)args[1];
        
        var obj1Pos = IoC.Resolve<Vector>("Game.GetProperty", "position", obj1);
        var obj2Pos = IoC.Resolve<Vector>("Game.GetProperty", "position", obj2);
        Vector difPos = obj1Pos-obj2Pos;
        var obj1Vel = IoC.Resolve<Vector>("Game.GetProperty", "velocity", obj1);
        var obj2Vel = IoC.Resolve<Vector>("Game.GetProperty", "velocity", obj2);
        Vector difVel = obj1Vel-obj2Vel;

        return new List<Vector>{difPos, difVel};
    }
}
