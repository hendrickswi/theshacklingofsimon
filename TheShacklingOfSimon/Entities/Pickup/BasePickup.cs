#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Entities.Pickup;

public abstract class BasePickup : IPickup
{
    public abstract IItem Item { get; protected set; }
    public Vector2 Position { get; protected set; }
    public Vector2 Velocity { get; set; }
    public bool IsActive { get; protected set; }
    public Rectangle Hitbox { get; protected set; }
    public ISprite Sprite { get; set; }
    
    protected BasePickup(Vector2 position, ISprite sprite)
    {
        Position = position;
        Velocity = Vector2.Zero;
        IsActive = true;
        Hitbox = new Rectangle((int)position.X, (int)position.Y, 16, 16);
        Sprite = sprite;
    }

    public void Update(GameTime delta)
    {
        Sprite?.Update(delta);
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        Sprite?.Draw(spriteBatch, Position, Color.White);
    }
    
    public void Discontinue()
    {
        IsActive = false;
    }

    public void SetPosition(Vector2 position)
    {
        Position = position;
        Hitbox = new Rectangle((int)position.X, (int)position.Y, Hitbox.Width, Hitbox.Height);
        Velocity = Vector2.Zero;
    }

    public void OnCollision(IEntity other)
    {
        other.OnCollision(this);
    }

    public abstract void OnCollision(IPlayer player);
    
    public virtual void OnCollision(IEnemy enemy)
    {
    }
    
    public virtual void OnCollision(IProjectile projectile)
    {
    }

    public virtual void OnCollision(ITile tile)
    {
    }

    public virtual void OnCollision(IPickup pickup)
    {
    }
}