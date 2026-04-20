#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Sprites.Decorators;

public class FadingSprite : ISprite
{
    private readonly ISprite _baseSprite;
    private float _currentAlpha;
    private float _endAlpha;
    private float _fadeSpeed;

    public FadingSprite(ISprite baseSprite, float startAlpha, float endAlpha, float fadeSpeed)
    {
        _baseSprite = baseSprite;
        _endAlpha = MathHelper.Clamp(endAlpha, 0f, 1f);
        _currentAlpha = MathHelper.Clamp(startAlpha, 0f, 1f);
        _fadeSpeed = fadeSpeed;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color)
    {
        _baseSprite.Draw(spriteBatch, pos, color * _currentAlpha);
    }
    
    public void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color)
    {
        _baseSprite.Draw(spriteBatch, destination, color * _currentAlpha);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        _baseSprite.Draw(
            spriteBatch, 
            pos, 
            color * _currentAlpha, 
            rotation, 
            origin, 
            scale, 
            effects, 
            layerDepth);
    }

    public void Update(GameTime delta)
    {
        _currentAlpha += _fadeSpeed * (float)delta.ElapsedGameTime.TotalSeconds;

        if (_fadeSpeed > 0 && _currentAlpha > _endAlpha ||
            _fadeSpeed < 0 && _currentAlpha < _endAlpha)
        {
            _currentAlpha = _endAlpha;
        }
        
        _currentAlpha = MathHelper.Clamp(_currentAlpha, 0f, 1f);
        _baseSprite.Update(delta);
    }
    
    public Vector2 GetDimensions()
    {
        return _baseSprite.GetDimensions();
    }
    
    public ISprite RemoveDecorator()
    {
        return _baseSprite;
    }
}