using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheShacklingOfSimon.Sprites.Products;

public interface ISprite
{
    public void Draw(SpriteBatch spriteBatch, Vector2 pos);
    public void Update(GameTime delta);
}