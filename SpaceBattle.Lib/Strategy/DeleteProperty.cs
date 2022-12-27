namespace SpaceBattle.Lib;

public interface IDeleteProperty
{
    public void DeleteProperty(IUObject obj, string i);
}

public class DeleteProperty : ICommand
{
    IDeleteProperty propertyDelete;
    IUObject obj;
    string property;
    public DeleteProperty(IDeleteProperty propertyDelete, IUObject obj, string property)
    {
        this.propertyDelete = propertyDelete;
        this.obj = obj;
        this.property = property;
    }
    public void Execute()
    {
        propertyDelete.DeleteProperty(obj, property);
    }
}
