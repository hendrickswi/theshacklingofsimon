using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheShacklingOfSimon.Sprites.Products;

public class SpriteText : ISprite
{
    private readonly SpriteFont _font;
    private readonly string _text;

    
    public SpriteText(SpriteFont font, string text, Vector2 pos)
    {
        this._font = font;
        this._text = text;
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 pos)
    {
        spriteBatch.DrawString(_font, _text, pos, Color.Black);
    }

    public void Update(GameTime delta)
    {
        // No-op
    }
}