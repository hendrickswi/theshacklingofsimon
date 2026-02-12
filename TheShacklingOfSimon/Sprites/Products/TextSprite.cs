using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
    
    public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, SpriteEffects effects)
    {
        spriteBatch.DrawString(_font, _text, pos, color);
    }

    public void Update(GameTime delta)
    {
        // No-op
    }
}