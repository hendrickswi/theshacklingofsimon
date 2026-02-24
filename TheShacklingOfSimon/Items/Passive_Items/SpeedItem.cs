using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Items.Passive_Items;

public class SpeedItem : IItem
{
    public string Name { get; }
    public string Description { get; }
    public IPlayer Player { get; }
    public ItemEffects Effects { get; set; }


    public SpeedItem(IPlayer player)
    {
        Player = player;
        Name = "Running Boots";
        Description = "";
        Effects = new ItemEffects(0, 0, 0, 2, false);
    }
    public void Effect()
    {
        
    }
}