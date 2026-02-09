using Microsoft.Xna.Framework.Content;

namespace TheShacklingOfSimon.Sprites.Factory;

public interface ISpriteFactory
{
    void LoadAllTextures(ContentManager content);
    void CreateSprite();
}