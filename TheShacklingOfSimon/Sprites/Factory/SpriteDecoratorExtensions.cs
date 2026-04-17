#region

using TheShacklingOfSimon.Sprites.Decorators;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Sprites.Factory;

/// <summary>
/// Provides extension methods for decorating sprites with additional behaviors.
/// </summary>
public static class SpriteDecoratorExtensions
{
    /// <summary>
    /// Adds fading behavior to an existing sprite, allowing its transparency to change over time.
    /// </summary>
    /// <param name="baseSprite">The base sprite to which fading behavior will be added.</param>
    /// <param name="startAlpha">The initial transparency level for the sprite. Must be between 0 (fully transparent) and 1 (fully opaque).</param>
    /// <param name="endAlpha">The final transparency level for the sprite. Must be between 0 (fully transparent) and 1 (fully opaque).</param>
    /// <param name="fadeSpeed">The rate at which the sprite's transparency changes over time. A positive value increases transparency, while a negative value decreases it.</param>
    /// <returns>A new sprite instance with fading behavior applied.</returns>
    public static ISprite WithFade(this ISprite baseSprite, float startAlpha, float endAlpha, float fadeSpeed)
    {
        return new FadingSprite(baseSprite, startAlpha, endAlpha, fadeSpeed);
    }

    /// <summary>
    /// Adds a delay to the update behavior of an existing sprite.
    /// </summary>
    /// <param name="baseSprite">The base sprite to which the delay behavior will be added.</param>
    /// <param name="delay">The amount of time, in seconds, to delay update operations.</param>
    /// <returns>A new sprite instance with the update delay applied.</returns>
    public static ISprite WithUpdateDelay(this ISprite baseSprite, float delay)
    {
        return new UpdateDelaySprite(baseSprite, delay);
    }

    /// <summary>
    /// Adds a delay to the rendering of an existing sprite.
    /// </summary>
    /// <param name="baseSprite">The base sprite to which the delay behavior will be added.</param>
    /// <param name="delay">The amount of time, in seconds, to delay rendering operations.</param>
    /// <returns>A new sprite instance with the render delay behavior applied.</returns>
    public static ISprite WithDrawDelay(this ISprite baseSprite, float delay)
    {
        return new DrawDelaySprite(baseSprite, delay);
    }

    // Can add more decorator instantiation methods here as more decorators are implemented
}