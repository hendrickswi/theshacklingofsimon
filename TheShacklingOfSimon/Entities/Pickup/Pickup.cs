using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Entities.Pickup;
public class Pickup : IPickup
{
    Vector2 Position { get; }
    Vector2 Velocity { get; }
    bool IsActive { get; set; }
    Rectangle Hitbox { get; }
    ISprite Sprite { get; }

    public void Update(GameTime delta)
    {
        //Interact(IEntity player);
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        
    }
    public void Discontinue()
    {
        IsActive = false;
    }

    /* To be implemented after Sprint 2:
    * void Interact(IEntity other)
    *
    * from IEntity
    */

    IItem Item { get; set; }
    IItem IPickup.Item { get => Item; set => Item = value; }

    Vector2 IEntity.Position => Position;

    Vector2 IEntity.Velocity { get; set; }

    bool IEntity.IsActive => IsActive;

    Rectangle IEntity.Hitbox => Hitbox;

    ISprite IEntity.Sprite { get; set; }
}