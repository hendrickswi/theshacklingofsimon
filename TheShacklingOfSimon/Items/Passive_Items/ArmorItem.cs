using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Items;

public class ArmorItem : IItem
{
    public string Name { get; }
    public string Description { get; }
    public IPlayer Player { get; }
    public ItemEffects Effects { get; set; }


    public ArmorItem(IPlayer player)
    {
        Player = player;
        Name = "Trusty Armor";
        Description = "Allows you to take more hits.";
        Effects = new ItemEffects(0, 0, 2, 0, false);
    }
    public void Effect()
    {
        
    }
}