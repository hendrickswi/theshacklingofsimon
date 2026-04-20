#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace TheShacklingOfSimon.Sprites.Products;

public interface ISprite
{
    /// <summary>
    /// Draws the sprite or object to the screen at a specified position with a specified color.
    /// The most basic <c>Draw()</c> implementation.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used for rendering the sprite.</param>
    /// <param name="pos">The position at which to draw the sprite.</param>
    /// <param name="color">The color mask applied to the sprite during rendering.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color);

    /// <summary>
    /// Draws the sprite or object to the screen within a specified rectangular area with a specified color.
    /// This implementation of <c>Draw()</c> ensures the sprite is scaled to fit within the specified rectangle.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used for rendering the sprite.</param>
    /// <param name="destination">The rectangular area on the screen where the sprite will be drawn.</param>
    /// <param name="color">The color mask applied to the sprite during rendering.</param>
    public void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color);

    /// <summary>
    /// Renders the sprite to the specified position on the screen with the given parameters.
    /// The "full control" implementation of <c>Draw()</c>.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used for rendering the sprite.</param>
    /// <param name="pos">The position where the sprite will be drawn.</param>
    /// <param name="color">The color mask applied to the sprite during rendering.</param>
    /// <param name="rotation">The rotation angle, in radians, applied to the sprite.</param>
    /// <param name="origin">The origin point for the sprite's rotation and scaling.</param>
    /// <param name="scale">The scaling factor applied to the sprite.</param>
    /// <param name="effects">The sprite effects, such as flipping, applied during rendering.</param>
    /// <param name="layerDepth">The layer depth at which to render the sprite, where 0 is front and 1 is back.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 pos,
        Color color, float rotation, Vector2 origin,
        float scale, SpriteEffects effects, float layerDepth);

    /// <summary>
    /// Updates the state or behavior of the implementing object based on the game time.
    /// This method may handle logic such as animations, state transitions, or positional updates.
    /// </summary>
    /// <param name="delta">The elapsed game time since the last update</param>
    public void Update(GameTime delta);

    /// <summary>
    /// Retrieves the dimensions of the sprite as it would be displayed on screen.
    /// </summary>
    /// <returns>A <c>Vector2</c> object containing the width and height of the sprite.</returns>
    public Vector2 GetDimensions();

    /// <summary>
    /// Removes the "top" decorator wrapping the current sprite, returning the wrapped sprite.
    /// If the sprite is not wrapped in any decorators, the current sprite instance is returned.
    /// </summary>
    /// <returns>
    /// The inner, potentially still wrapped sprite, or the current sprite if no
    /// decorators are applied.
    /// </returns>
    public ISprite RemoveDecorator();
}