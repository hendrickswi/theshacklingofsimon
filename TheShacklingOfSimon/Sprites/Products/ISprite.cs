using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheShacklingOfSimon.Sprites.Products;

public interface ISprite
{
    public void Draw(SpriteBatch spriteBatch);
    public void Update(GameTime delta);
}