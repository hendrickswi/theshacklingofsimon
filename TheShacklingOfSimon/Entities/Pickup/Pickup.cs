using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.LevelHandler.Tiles;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Entities.Pickup;
public class Pickup : IPickup
{
    IItem Item { get; set; }
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

    public void OnCollision(IEntity other)
    {
        // overload
    }

    public void OnCollision(IPlayer player)
    {
        player.AddItemToInventory(Item);
        Discontinue();
    }
    public void OnCollision(IEnemy enemy)
    {
        throw new System.NotImplementedException();
    }
    public void OnCollision(IProjectile projectile)
    {
        throw new System.NotImplementedException();
    }

    public void OnCollision(ITile tile)
    {
        throw new System.NotImplementedException();
    }

    public void OnCollision(IPickup pickup)
    {
        // no-op
    }
    IItem IPickup.Item { get => Item; set => Item = value; }

    Vector2 IEntity.Position => Position;

    Vector2 IEntity.Velocity { get; set; }

    bool IEntity.IsActive => IsActive;

    Rectangle IEntity.Hitbox => Hitbox;

    ISprite IEntity.Sprite { get; set; }
}