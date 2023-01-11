namespace SpaceBattle.Lib;
using Hwdtech;

public class CheckCollision : ICommand
{
    private IUObject obj1, obj2;
    public CheckCollision(IUObject obj1, IUObject obj2)
    {
        this.obj1 = obj1;
        this.obj2 = obj2;
    }

    public void Execute()
    {
        var diffs = IoC.Resolve<List<Vector>>("Game.GetDifference", obj1, obj2);
        bool isCollision = IoC.Resolve<bool>("Game.CheckCollision", diffs);

        if (isCollision) throw new Exception();
    }
}
