using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheShacklingOfSimon.Sprites.Products;

public class AnimatedSprite : ISprite
{
    private readonly Texture2D _texture;
    private readonly Vector2 _pos;
    // The part of the sprite sheet to draw
    private readonly Rectangle[] _sourceRectangle;
    private int _currentFrame;

    // Animation variables
    private double _timer;
    private readonly double _speed;
    
    public AnimatedSprite(Texture2D texture, Rectangle[] sourceRectangle)
    {
        this._texture = texture;
        this._sourceRectangle = sourceRectangle;
        this._currentFrame = 0;
        this._timer = 0;
        this._speed = 0.25;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 pos)
    {
        spriteBatch.Draw(_texture, pos, _sourceRectangle[_currentFrame], Color.White);
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