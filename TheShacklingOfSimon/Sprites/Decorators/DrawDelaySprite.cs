using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Sprites.Decorators;

public class DrawDelaySprite : BaseDecoratedSprite
{
    private readonly float _delay;
    private float _timer;
    
    public DrawDelaySprite(ISprite baseSprite, float delay) 
        : base(baseSprite)
    {
        _delay = delay;
    }
    
    public override void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color)
    {
        if (_timer < _delay) return;
        BaseSprite.Draw(spriteBatch, pos, color);
    }
    
    public override void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color)
    {
        if (_timer < _delay) return;
        BaseSprite.Draw(spriteBatch, destination, color);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        if (_timer < _delay) return;
        BaseSprite.Draw(
            spriteBatch, 
            pos, 
            color,
            rotation, 
            origin, 
            scale, 
            effects, 
            layerDepth);
    }

    public override void Update(GameTime delta)
    {
        _timer += (float)delta.ElapsedGameTime.TotalSeconds;
        BaseSprite.Update(delta);
    }
}