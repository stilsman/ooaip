namespace SpaceBattle.Lib;

public interface IRotatable
{
    Angle angular {get; set;}
    Angle angularVelocity {get;}
}

public class RotateCommand : ICommand
{
    private IRotatable rotateableobj;
    public RotateCommand(IRotatable obj)
    {
        rotateableobj = obj;
    }

    public void Execute()
    {
        rotateableobj.angular += rotateableobj.angularVelocity;
    }
}
