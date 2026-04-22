#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Sprites.Decorators;

public class UpdateDelaySprite : BaseDecoratedSprite
{
    private readonly float _delay;
    private float _timer;
    
    public UpdateDelaySprite(ISprite baseSprite, float delay) 
        : base(baseSprite)
    {
        _timer = 0.0f;
        _delay = delay;
    }
    
    public override void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color)
    {
        BaseSprite.Draw(spriteBatch, pos, color);
    }
    
    public override void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color)
    {
        BaseSprite.Draw(spriteBatch, destination, color);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
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
        _timer += (float) delta.ElapsedGameTime.TotalSeconds;
        if (_timer < _delay) return;
        BaseSprite.Update(delta);
    }
}