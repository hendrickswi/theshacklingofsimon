#region

using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Players;

#endregion

namespace TheShacklingOfSimon.Items;

public class NoneItem : IItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IDamageableEntity Entity { get; }
    public ItemEffects Effects { get; }

    public NoneItem(IPlayer player)
    {
        Entity = player;
    }

    public void ApplyEffect()
    {
        // No-op
    }
}