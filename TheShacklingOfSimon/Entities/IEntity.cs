using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Level_Handler.Tiles;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Entities;
public interface IEntity
{
    // Common properties of *every* entity
    Vector2 Position { get; }
    Vector2 Velocity { get; set; }
    bool IsActive { get; }
    Rectangle Hitbox { get; }
    ISprite Sprite { get; set; }
    
    void Update(GameTime delta);
    void Draw(SpriteBatch spriteBatch);
    void Discontinue();
    void OnCollision(IEntity other);
    void OnCollision(IPlayer player);
    void OnCollision(IEnemy enemy);
    void OnCollision(IProjectile projectile);
    void OnCollision(ITile tile);
}
