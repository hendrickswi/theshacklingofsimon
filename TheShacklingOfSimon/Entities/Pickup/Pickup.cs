using Microsoft.VisualBasic;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.LevelHandler.Tiles;
using TheShacklingOfSimon.Sprites.Products;
using TheShacklingOfSimon.Sprites.Factory;

namespace TheShacklingOfSimon.Entities.Pickup;
public class Pickup : IPickup
{
    private List<IPickup> _pickups = new();

    public IItem Item { get; set; }
    public Vector2 Position { get; private set; }
    public Vector2 Velocity { get; set; }
    public bool IsActive { get; private set; }
    public Rectangle Hitbox { get; private set; }
    public ISprite Sprite { get; set; }

    public Pickup(Vector2 position, IItem item, ISprite sprite)
    {
        Item = item;
        Position = position;
        Velocity = Vector2.Zero;
        IsActive = true;
        Hitbox = new Rectangle((int)position.X, (int)position.Y, 32, 32);
        Sprite = sprite;
    }

    public void Update(GameTime delta)
    {
        Sprite.Update(delta);
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (Sprite != null)
        {
            Sprite.Draw(spriteBatch, Position, Color.White);
        }
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

    public void OnCollision(IPlayer player)
    {
        player.AddItemToInventory(Item);
        Discontinue();
    }
    
    public void OnCollision(IEnemy enemy)
    {
        // No-op
    }
    public void OnCollision(IProjectile projectile)
    {
        // No-op
    }

    public void OnCollision(ITile tile)
    {
        // No-op
    }

    public void OnCollision(IPickup pickup)
    {
        // No-op
    }
}