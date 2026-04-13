#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;

#endregion

namespace TheShacklingOfSimon.Items.Active_Items;
public abstract class ActiveItem : IActiveItem
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    
    protected IDamageableEntity Entity;
    
    public ActiveItem(IDamageableEntity entity)
    {
        Entity = entity;
    }
    
    public abstract void ApplyEffect();
    public abstract void ClearEffect();
    public abstract void Update(GameTime delta);

}