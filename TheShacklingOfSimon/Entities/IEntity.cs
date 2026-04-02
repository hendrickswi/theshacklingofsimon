#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.LevelHandler.Tiles;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Entities;
public interface IEntity
{
    // Common properties of *every* entity
    Vector2 Position { get; }
    Vector2 Velocity { get; set; }
    bool IsActive { get; }
    Rectangle Hitbox { get; }
    ISprite Sprite { get; set; }

    /// <summary>
    /// Updates the state <c>this</c>, based on the provided game time.
    /// </summary>
    /// <param name="delta">The elapsed game time since the last update.</param>
    void Update(GameTime delta);

    /// <summary>
    /// Renders <c>this</c> to the given sprite batch.
    /// </summary>
    /// <param name="spriteBatch">The <c>SpriteBatch</c> instance used for drawing <c>this</c>.</param>
    void Draw(SpriteBatch spriteBatch);

    /// <summary>
    /// Deactivates <c>this</c>, marking it for removal or making it no longer interactable.
    /// </summary>
    void Discontinue();

    /// <summary>
    /// Sets the position of the entity to the specified value.
    /// Updates related properties such as velocity and hitbox, if applicable.
    /// </summary>
    /// <param name="position">The new position to set for the entity.</param>
    void SetPosition(Vector2 position);

    /// <summary>
    /// Handles the collision logic between <c>this</c> and another <c>IEntity</c>.
    /// In practice, this is just a router method.
    /// </summary>
    /// <param name="other">The other <c>IEntity</c> involved in the collision.</param>
    void OnCollision(IEntity other);

    /// <summary>
    /// Handles collision logic when <c>this</c> collides with a different object of type <c>IPlayer</c>.
    /// </summary>
    /// <param name="player">The entity that <c>this</c> has collided with.</param>
    void OnCollision(IPlayer player);

    /// <summary>
    /// Handles collision logic when <c>this</c> collides with a different object of type <c>IEnemy</c>.
    /// </summary>
    /// <param name="enemy">The entity that <c>this</c> collided with.</param>
    void OnCollision(IEnemy enemy);

    /// <summary>
    /// Handles collision logic when <c>this</c> collides with a different object of type <c>IProjectile</c>.
    /// </summary>
    /// <param name="projectile">The entity that <c>this</c> collided with.</param>
    void OnCollision(IProjectile projectile);
    
    /// <summary>
    /// Handles collision logic when <c>this</c> collides with a different object of type <c>ITile</c>.
    /// </summary>
    /// <param name="tile">The entity that <c>this</c> collided with.</param>
    void OnCollision(ITile tile);
    
    /// <summary>
    /// Handles collision logic when <c>this</c> collides with a different object of type <c>IPickup</c>.
    /// </summary>
    /// <param name="pickup">The entity that <c>this</c> collided with.</param>
    void OnCollision(IPickup pickup);
}
