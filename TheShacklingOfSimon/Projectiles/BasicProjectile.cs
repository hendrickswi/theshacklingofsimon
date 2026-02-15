using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Projectiles;

public class BasicProjectile : IProjectile
{
	public Vector2 Position { get; private set; }
	public Vector2 Velocity { get; set; }
	public bool IsActive { get; private set; }
	public Rectangle Hitbox { get; private set; }
	public ISprite Sprite { get; set; }

	public ProjectileStats Stats { get; private set; }

	public BasicProjectile(Vector2 startPos, Vector2 direction, ProjectileStats stats)
	{
		Position = startPos;
		Stats = stats;
		IsActive = true;

		direction.Normalize();
		Velocity = direction * stats.Speed;

		Sprite = SpriteFactory.Instance.CreateAnimatedSprite("BasicProjectile");

		Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 8, 8);
	}

	public void Update(GameTime gameTime)
	{
		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
		Position += Velocity * dt;

		Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 8, 8);

		Sprite?.Update(gameTime);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		Texture2D debugTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
		debugTexture.SetData(new[] { Color.White });

		spriteBatch.Draw(debugTexture, Hitbox, Color.Red);
	}

	public void Discontinue()
	{
		IsActive = false;
	}
}