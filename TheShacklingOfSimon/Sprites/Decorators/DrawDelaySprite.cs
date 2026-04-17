using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Sprites.Decorators;

public class DrawDelaySprite : ISprite
{
    private readonly ISprite _baseSprite;
    private readonly float _delay;
    private float _timer;
    
    public DrawDelaySprite(ISprite baseSprite, float delay)
    {
        _baseSprite = baseSprite;
        _delay = delay;
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color)
    {
        if (_timer < _delay) return;
        _baseSprite.Draw(spriteBatch, pos, color);
    }
    
    public void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color)
    {
        if (_timer < _delay) return;
        _baseSprite.Draw(spriteBatch, destination, color);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        if (_timer < _delay) return;
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
        _timer += (float)delta.ElapsedGameTime.TotalSeconds;
        _baseSprite.Update(delta);
    }

    public Vector2 GetDimensions()
    {
        return _baseSprite.GetDimensions();
    }
}