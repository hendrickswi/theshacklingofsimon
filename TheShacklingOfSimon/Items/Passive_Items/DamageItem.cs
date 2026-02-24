using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Items.Passive_Items;

public class DamageItem : IItem
{
    public string Name { get; }
    public string Description { get; }
    public IPlayer Player { get; }
    public ItemEffects Effects { get; set; }


    public DamageItem(IPlayer player)
    {
        Player = player;
        Name = "Brace";
        Description = "Allows you to channel more power into your shots.";
        Effects = new ItemEffects(1, 0, 0, 0, false);
    }
    public void Effect()
    {
        
    }
}