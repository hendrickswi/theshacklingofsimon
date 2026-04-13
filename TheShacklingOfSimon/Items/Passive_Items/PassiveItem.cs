#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.StatusEffects;

#endregion

namespace TheShacklingOfSimon.Items.Passive_Items;
public abstract class PassiveItem : IItem
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    
    protected IDamageableEntity Entity;
    
    protected PassiveItem(IDamageableEntity entity)
    {
        Entity = entity;
    }

    public abstract void ApplyEffect();
}