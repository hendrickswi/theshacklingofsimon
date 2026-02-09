using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheShacklingOfSimon.Sprites.Products;

public class Sprite2 : ISprite
{
    private readonly Texture2D _texture;
    private readonly Vector2 _pos;
    // The part of the sprite sheet to draw
    private readonly Rectangle[] _sourceRectangle;
    private int _currentFrame;

    // Animation variables
    private double _timer;
    private readonly double _speed;
    
    public Sprite2(Texture2D texture, Vector2 pos, Rectangle[] sourceRectangle)
    {
        this._texture = texture;
        this._pos = pos;
        this._sourceRectangle = sourceRectangle;
        this._currentFrame = 0;
        this._timer = 0;
        this._speed = 0.25;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, _pos, _sourceRectangle[_currentFrame], Color.White);
    }

    public void Update(GameTime delta)
    {
        _timer += delta.ElapsedGameTime.TotalSeconds;
        if (_timer >= _speed)
        {
            _timer -= _speed;
            _currentFrame++;
            if (_currentFrame >= _sourceRectangle.Length)
            {
                _currentFrame = 0;
            }
        }
    }
}