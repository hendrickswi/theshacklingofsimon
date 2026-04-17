#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles.TileConstructor;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Entities.Projectiles.Implementations;

public class BasicProjectile : ProjectileBase
{
	private readonly string _sfx;
	private float _timer;
	
    public BasicProjectile(Vector2 startPos, Vector2 direction, ISprite sprite, ProjectileStats stats)
	{
        Position = startPos;
		Stats = stats;
		IsActive = true;

		if (direction.LengthSquared() > 0.0001f)
		{
			direction.Normalize();
		}
		else
		{
			direction = new Vector2(0, 1);
		}
		
		Velocity = direction * stats.Speed;
		Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 8, 8);
        Sprite = SpriteFactory.Instance.CreateStaticSprite("BasicProjectile");
        
		_sfx = SoundManager.Instance.AddSFX("projectiles", "splatter00");
    }

    public override void Update(GameTime gameTime)
	{
		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
		Position += Velocity * dt;
		Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 8, 8);
		Sprite.Update(gameTime);

		// Safety in case collision doesn't work
		_timer += dt;
		if (_timer > 10f)
		{
			Discontinue();
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		Sprite.Draw(spriteBatch, Position, Color.White);
	}

	public override IProjectile Clone(Vector2 startPos, Vector2 direction, ISprite sprite, ProjectileStats stats)
	{
		return new BasicProjectile(startPos, direction, sprite, stats);
	}

    public override void OnCollision(ITile tile)
    {
        if (!IsActive || tile == null) return;
        if (tile.BlocksProjectiles)
        {
			SoundManager.Instance.PlaySFX(_sfx);
        }
    }
}
