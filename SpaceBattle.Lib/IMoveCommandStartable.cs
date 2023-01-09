namespace SpaceBattle.Lib;

public interface IMoveCommandStartable
{
    IUObject UObject { get; }

    IDictionary<string, object> dict
    {
        get;
    }
}
