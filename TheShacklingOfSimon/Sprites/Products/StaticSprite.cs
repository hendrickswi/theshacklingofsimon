#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace TheShacklingOfSimon.Sprites.Products;

public class StaticSprite : ISprite
{
    private readonly Texture2D _texture;
    // The part of the sprite sheet to draw
    private readonly Rectangle _sourceRectangle;

    public StaticSprite(Texture2D texture, Rectangle sourceRectangle)
    {
        _texture = texture;
        _sourceRectangle = sourceRectangle;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color)
    {
        spriteBatch.Draw(_texture, pos, _sourceRectangle, color);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color)
    {
        spriteBatch.Draw(_texture, destination, _sourceRectangle, color);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        spriteBatch.Draw(_texture, pos, _sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
    }

    public void Update(GameTime delta)
    {
        // No-op
    }

    public Vector2 GetDimensions()
    {
        return new Vector2(_sourceRectangle.Width, _sourceRectangle.Height);
    }

    public ISprite RemoveDecorator()
    {
        return this;
    }
}