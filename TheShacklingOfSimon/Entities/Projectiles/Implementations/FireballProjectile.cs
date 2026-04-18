#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles.TileConstructor;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Entities.Projectiles.Implementations;

public class FireballProjectile : ProjectileBase
{
	private readonly string _sfx;
	private float _timer;
	private Vector2 Direction;

    public FireballProjectile(Vector2 startPos, Vector2 direction, ISprite sprite, ProjectileStats stats)
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
		Direction = direction;
		Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 10, 10);
		Sprite = SpriteFactory.Instance.CreateStaticSprite("FireballProjectile");
		
		
	}

	public override void Update(GameTime gameTime)
	{
		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
		Position += Velocity * dt;
		Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 10, 10);
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
		//Sprite.Draw(spriteBatch,Position , Color.White);
		if (Direction==new Vector2(0,1))
		{
            Sprite = SpriteFactory.Instance.CreateStaticSprite("FireballProjectileVertical");
            Sprite.Draw(spriteBatch, Position, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

        }
		else if (Direction == new Vector2(1, 0)) {

            Sprite.Draw(spriteBatch, Position, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }
        else if (Direction == new Vector2(-1,0))
        {

            Sprite.Draw(spriteBatch, Position, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 1f);
        }
        else if (Direction == new Vector2(0, -1))
        {
            Sprite = SpriteFactory.Instance.CreateStaticSprite("FireballProjectileVertical");
            Sprite.Draw(spriteBatch, Position, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 1f);

        }
        
    }

	public override IProjectile Clone(Vector2 startPos, Vector2 direction, ISprite sprite, ProjectileStats stats)
	{
		return new FireballProjectile(startPos, direction, sprite, stats);
	}

    public override void OnCollision(ITile tile)
    {
	    if (!IsActive || tile == null) return;
	    if (tile.BlocksProjectiles)
	    {
		    // play sfx here
	    }

    }
}
