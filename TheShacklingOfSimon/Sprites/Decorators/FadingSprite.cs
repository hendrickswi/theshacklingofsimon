#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Sprites.Decorators;

public class FadingSprite : BaseDecoratedSprite
{
    private float _currentAlpha;
    private float _endAlpha;
    private float _fadeSpeed;

    public FadingSprite(ISprite baseSprite, float startAlpha, float endAlpha, float fadeSpeed) 
        : base(baseSprite)
    {
        _endAlpha = MathHelper.Clamp(endAlpha, 0f, 1f);
        _currentAlpha = MathHelper.Clamp(startAlpha, 0f, 1f);
        _fadeSpeed = fadeSpeed;
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color)
    {
        BaseSprite.Draw(spriteBatch, pos, color * _currentAlpha);
    }
    
    public override void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color)
    {
        BaseSprite.Draw(spriteBatch, destination, color * _currentAlpha);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        BaseSprite.Draw(
            spriteBatch, 
            pos, 
            color * _currentAlpha, 
            rotation, 
            origin, 
            scale, 
            effects, 
            layerDepth);
    }

    public override void Update(GameTime delta)
    {
        _currentAlpha += _fadeSpeed * (float)delta.ElapsedGameTime.TotalSeconds;

        if (_fadeSpeed > 0 && _currentAlpha > _endAlpha ||
            _fadeSpeed < 0 && _currentAlpha < _endAlpha)
        {
            _currentAlpha = _endAlpha;
        }
        
        _currentAlpha = MathHelper.Clamp(_currentAlpha, 0f, 1f);
        BaseSprite.Update(delta);
    }
}