using System;

namespace SpaceBattle.Lib;
 
public interface IMovable 
{
    Vector position { get; set; }
    Vector velocity { get; }
}

public class MoveCommand : ICommand
{   
    private IMovable movableobj;

    public MoveCommand(IMovable obj)
    {
        movableobj = obj;
    }
    public void Execute() 
    {
        movableobj.position += movableobj.velocity;
    }

}
