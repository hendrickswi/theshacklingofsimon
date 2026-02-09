using Microsoft.Xna.Framework.Content;
using TheShacklingOfSimon.Sprites.Products;


namespace TheShacklingOfSimon.Sprites.Factory;

public interface ISpriteFactory
{
    void LoadAllTextures(ContentManager content);
    ISprite CreateSprite();
}