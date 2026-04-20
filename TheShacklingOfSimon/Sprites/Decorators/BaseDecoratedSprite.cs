using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Sprites.Decorators;

public abstract class BaseDecoratedSprite : ISprite
{
    protected readonly ISprite BaseSprite;
    
    protected BaseDecoratedSprite(ISprite baseSprite)
    {
        BaseSprite = baseSprite;
    }

    public abstract void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color);
    public abstract void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color);
    public abstract void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, float rotation, Vector2 origin,
        float scale, SpriteEffects effects, float layerDepth);
    public abstract void Update(GameTime delta);

    public Vector2 GetDimensions()
    {
        return BaseSprite.GetDimensions();
    }

    public ISprite RemoveDecorator()
    {
        return BaseSprite;
    }
}