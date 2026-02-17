using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheShacklingOfSimon.Entities.Projectiles;

public interface IProjectile : IEntity
{
	/*
	 * Inherits
	 * Vector2 Position { get; }
	 * Vector2 Velocity { get; }
	 * bool IsActive { get; }
	 * Rectangle Hitbox { get; }
	 * ISprite Sprite { get; }
	 *
	 * void Update(GameTime delta),
	 * void Draw(SpriteBatch spriteBatch),
	 * void Discontinue();
	 *
	 * To be implemented after Sprint 2:
	 * void Interact(IEntity other)
	 *
	 * from IEntity
	 */
	ProjectileStats Stats { get; }
}