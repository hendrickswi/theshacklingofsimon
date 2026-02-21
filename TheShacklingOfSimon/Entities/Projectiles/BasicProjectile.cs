using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Entities.Projectiles;

public class BasicProjectile : IProjectile
{
	public Vector2 Position { get; private set; }
	public Vector2 Velocity { get; set; }
	public bool IsActive { get; private set; }
	public Rectangle Hitbox { get; private set; }
	public ISprite Sprite { get; set; }

	public ProjectileStats Stats { get; private set; }

	private float timeActive;
	private Texture2D debugTexture;
	public BasicProjectile(Vector2 startPos, Vector2 direction, ProjectileStats stats)
	{
		Position = startPos;
		Stats = stats;
		IsActive = true;

		direction.Normalize();
		Velocity = direction * stats.Speed;
		
		Sprite = SpriteFactory.Instance.CreateAnimatedSprite("BasicProjectile", 0.2f);

		Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 8, 8);
       
    }

	public void Update(GameTime gameTime)
	{
		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
		timeActive +=dt;
		Position += Velocity * dt;

		Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 8, 8);
		ShouldDestroy();

		Sprite?.Update(gameTime);
	}

	public void Draw(SpriteBatch spriteBatch)
	{


        debugTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
		debugTexture.SetData(new[] { Color.White });
        
        spriteBatch.Draw(debugTexture, Hitbox, Color.Red);

        
	}

	private void ShouldDestroy()
	{
		if (timeActive>1.5f) { 
			Discontinue();
		}
		if (Position.X < 0||Position.X>1920||Position.Y<0||Position.Y>1080) {
			Discontinue();
		
		}
	}

	public void Discontinue()
	{
		IsActive = false;
	}
}