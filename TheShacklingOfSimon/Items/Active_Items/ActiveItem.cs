#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;

#endregion

namespace TheShacklingOfSimon.Items.Active_Items;
public abstract class ActiveItem : IActiveItem
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public IDamageableEntity Entity { get; set; }
    
    public ActiveItem(IDamageableEntity entity)
    {
        Entity = entity;
    }
    
    public abstract bool ApplyEffect();

    public virtual void ClearEffect()
    {
    }
    
    public abstract void Update(GameTime delta);

}