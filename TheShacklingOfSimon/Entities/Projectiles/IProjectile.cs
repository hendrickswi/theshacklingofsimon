using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Entities.Projectiles;

public interface IProjectile : IEntity
{
	ProjectileStats Stats { get; }
	
	IProjectile Clone(Vector2 startPos, Vector2 direction, ISprite sprite, ProjectileStats stats);
}