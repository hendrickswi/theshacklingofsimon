#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace TheShacklingOfSimon.Sprites.Products;

public class TextSprite : ISprite
{
    private readonly SpriteFont _font;
    private readonly string _text;

    public TextSprite(SpriteFont font, string text)
    {
        this._font = font;
        this._text = text;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color)
    {
        spriteBatch.DrawString(_font, _text, pos, color);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color)
    {
        Vector2 size = _font.MeasureString(_text);
        if (size.X <= 0f || size.Y <= 0f)
        {
            spriteBatch.DrawString(_font, _text, destination.Location.ToVector2(), color);
            return;
        }

        float scaleX = destination.Width / size.X;
        float scaleY = destination.Height / size.Y;
        float scale = MathF.Min(scaleX, scaleY);

        Vector2 scaledSize = size * scale;
        Vector2 topLeft = new Vector2(
            destination.X + (destination.Width - scaledSize.X) * 0.5f,
            destination.Y + (destination.Height - scaledSize.Y) * 0.5f
        );

        spriteBatch.DrawString(_font, _text, topLeft, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        spriteBatch.DrawString(_font, _text, pos, color, rotation, origin, scale, effects, layerDepth);
    }

    public void Update(GameTime delta)
    {
        // No-op
    }
    
    public Vector2 GetDimensions()
    {
        return _font.MeasureString(_text);
    }
    
    public ISprite RemoveDecorator()
    {
        return this;
    }
}