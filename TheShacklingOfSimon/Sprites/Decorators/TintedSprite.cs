using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Sprites.Decorators;

public class TintedSprite : BaseDecoratedSprite
{
    private readonly Color _tintColor;
    
    public TintedSprite(ISprite baseSprite, Color tintColor) 
        : base(baseSprite)
    {
        _tintColor = tintColor;
    }
    
    public override void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color)
    {
        BaseSprite.Draw(spriteBatch, pos, CalculateTintColor(color));
    }
    
    public override void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color)
    {
        BaseSprite.Draw(spriteBatch, destination, CalculateTintColor(color));
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        BaseSprite.Draw(
            spriteBatch, 
            pos, 
            CalculateTintColor(color),
            rotation, 
            origin, 
            scale, 
            effects, 
            layerDepth);
    }

    public override void Update(GameTime delta)
    {
        BaseSprite.Update(delta);
    }

    private Color CalculateTintColor(Color incomingColor)
    {
        if (incomingColor == Color.White) return _tintColor;
        
        // If incoming color is not white, find the "average" of the two colors
        return Color.Lerp(_tintColor, incomingColor, 0.5f);
    }
}