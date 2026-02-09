using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheShacklingOfSimon.Sprites.Products;

public class SpriteText : ISprite
{
    private readonly SpriteFont _font;
    private readonly string _text;
    private readonly Vector2 _pos; 

    
    public SpriteText(SpriteFont font, string text, Vector2 pos)
    {
        this._font = font;
        this._text = text;
        this._pos = pos;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(_font, _text, _pos, Color.Black);
    }

    public void Update(GameTime delta)
    {
        // This sprite does not move, nor is it animated.
    }
}