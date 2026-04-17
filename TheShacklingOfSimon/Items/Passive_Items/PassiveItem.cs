#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.StatusEffects;

#endregion

namespace TheShacklingOfSimon.Items.Passive_Items;
public abstract class PassiveItem : IPassiveItem
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public string SFX { get; protected set; }
    public IDamageableEntity Entity { get; set; }
    
    protected PassiveItem(IDamageableEntity entity)
    {
        Entity = entity;
    }

    public abstract bool ApplyEffect();
}