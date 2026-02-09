using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Sprites.Factory;

public class AnimatedSpriteFactory : ISpriteFactory
{
    private Texture2D _spriteTexture;
    private Dictionary<string, Rectangle[]> _map;

    public AnimatedSpriteFactory()
    {
        _map = new Dictionary<string, Rectangle[]>();
    }
    public void LoadAllTextures(ContentManager content)
    {
        // TODO
    }

    public ISprite CreateSprite()
    {
        return null;
        // TODO
    }
}