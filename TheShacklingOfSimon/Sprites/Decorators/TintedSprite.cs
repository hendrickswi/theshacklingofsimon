using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Sprites.Decorators;

public class TintedSprite : ISprite
{
    private readonly ISprite _baseSprite;
    private readonly Color _tintColor;
    
    public TintedSprite(ISprite baseSprite, Color tintColor)
    {
        _baseSprite = baseSprite;
        _tintColor = tintColor;
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color)
    {
        _baseSprite.Draw(spriteBatch, pos, CalculateTintColor(color));
    }
    
    public void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color)
    {
        _baseSprite.Draw(spriteBatch, destination, CalculateTintColor(color));
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        _baseSprite.Draw(
            spriteBatch, 
            pos, 
            CalculateTintColor(color),
            rotation, 
            origin, 
            scale, 
            effects, 
            layerDepth);
    }

    public void Update(GameTime delta)
    {
        _baseSprite.Update(delta);
    }

    public Vector2 GetDimensions()
    {
        return _baseSprite.GetDimensions();
    }

    private Color CalculateTintColor(Color incomingColor)
    {
        if (incomingColor == Color.White) return _tintColor;
        
        // If incoming color, is not white, find the "average" of the two colors
        return Color.Lerp(_tintColor, incomingColor, 0.5f);
    }
}