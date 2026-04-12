#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace TheShacklingOfSimon.Sprites.Products;

public class AnimatedSprite : ISprite
{
    private readonly Texture2D _texture;
    // The part of the sprite sheet to draw
    private readonly Rectangle[] _sourceRectangle;
    private int _currentFrame;

    // Animation variables
    private float _timer;
    private readonly float _speed;
    
    public AnimatedSprite(Texture2D texture, Rectangle[] sourceRectangle, float animationSpeed)
    {
        this._texture = texture;
        this._sourceRectangle = sourceRectangle;
        this._currentFrame = 0;
        this._timer = 0;
        this._speed = animationSpeed;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color)
    {
        spriteBatch.Draw(_texture, pos, _sourceRectangle[_currentFrame], color);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color)
    {
        spriteBatch.Draw(_texture, destination, _sourceRectangle[_currentFrame], color);
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        spriteBatch.Draw(_texture, pos, _sourceRectangle[_currentFrame], color, rotation, origin, scale, effects, layerDepth);
    }

    public void Update(GameTime delta)
    {
        _timer += (float)delta.ElapsedGameTime.TotalSeconds;
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
    
    public Vector2 GetDimensions()
    {
        return new Vector2(_sourceRectangle[_currentFrame].Width, _sourceRectangle[_currentFrame].Height);
    }
}