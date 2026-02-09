using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheShacklingOfSimon.Sprites.Products;

public class Sprite1 : ISprite
{
    private readonly Texture2D _texture;
    private readonly Vector2 _pos;
    // The part of the sprite sheet to draw
    private readonly Rectangle _sourceRectangle;

    public Sprite1(Texture2D texture, Vector2 pos, Rectangle sourceRectangle)
    {
        this._texture = texture;
        this._pos = pos;
        this._sourceRectangle = sourceRectangle;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, _pos, _sourceRectangle, Color.White);
    }

    public void Update(GameTime delta)
    {
        // This sprite does not move, nor is it animated
    }
}