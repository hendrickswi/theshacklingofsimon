using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

internal interface IEntity
{
    // Common properties of *every* entity
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public bool IsActive { get; set; } = true;
    public Vector2 Hitbox { get; set; }

    // Abstract methods that every Entity-extending class will have to define separately
    void Update(GameTime delta);
    void Draw(SpriteBatch spriteBatch);

    // Methods every Entity-extending class will need, but can also be overridden.
    void Interact();
    void Discontinue()
    {
       IsActive = false;
    }
}