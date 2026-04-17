#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Sprites.Decorators;

public class UpdateDelaySprite : ISprite
{
    private readonly ISprite _baseSprite;
    private readonly float _delay;
    private float _timer;
    
    public UpdateDelaySprite(ISprite baseSprite, float delay)
    {
        _baseSprite = baseSprite;
        _delay = delay;
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color)
    {
        _baseSprite.Draw(spriteBatch, pos, color);
    }
    
    public void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color)
    {
        _baseSprite.Draw(spriteBatch, destination, color);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        _baseSprite.Draw(
            spriteBatch, 
            pos, 
            color,
            rotation, 
            origin, 
            scale, 
            effects, 
            layerDepth);
    }

    public void Update(GameTime delta)
    {
        _timer += (float) delta.ElapsedGameTime.TotalSeconds;
        if (_timer < _delay) return;
        _baseSprite.Update(delta);
    }

    public Vector2 GetDimensions()
    {
        return _baseSprite.GetDimensions();
    }
}