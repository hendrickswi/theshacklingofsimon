namespace TheShacklingOfSimon.Items;

public interface IItem
{
    string Name { get; }
    string Description { get; }
    
    void ApplyEffect();
}