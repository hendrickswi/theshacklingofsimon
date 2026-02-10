using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Sprites.Factory;

public class AnimatedSpriteFactory : ISpriteFactory
{
    /*
     * Singleton pattern to prevent all Entity classes from instantiating
     * an instance of this.
     */
    private static AnimatedSpriteFactory _instance = new AnimatedSpriteFactory();
    public static AnimatedSpriteFactory Instance => _instance;

    private Dictionary<string, Texture2D> _textureStorage;
    private Dictionary<string, Rectangle[]> _map;

    private AnimatedSpriteFactory()
    {
        _map = new Dictionary<string, Rectangle[]>();
        _textureStorage = new Dictionary<string, Texture2D>();
    }
    
    /*
     * Loads all textures provided in content
     */
    public void LoadAllTextures(ContentManager content)
    {
        _spriteTexture = content.Load<Texture2D>("player");
        string jsonPath = Path.Combine(content.RootDirectory, "PlayerDefaultSprite.json");
        if (!File.Exists(jsonPath))
        {
            throw new FileNotFoundException("Could not find PlayerDefaultSprite.json at " + jsonPath + ".");
        }

        SpriteDataRoot data = JsonSerializer.Deserialize<SpriteDataRoot>(jsonPath);
        
        // Turn all the sprite data from the JSON file into Rectangle data
        foreach ( SpriteData sprite in data.Sprites )
        {
            Rectangle[] frames = new Rectangle[sprite.Frames.Count];
            for (int i = 0; i < sprite.Frames.Count; i++)
            {
                frames[i] = new Rectangle(
                    sprite.Frames[i].X,
                    sprite.Frames[i].Y,
                    sprite.Frames[i].W,
                    sprite.Frames[i].H
                );
            }
            _map.TryAdd(sprite.Name, frames);
        }
    }

    public ISprite CreateSprite(string spriteName)
    {
        ISprite sprite = null;
        if (_map.ContainsKey(spriteName))
        {
            sprite = new AnimatedSprite(_spriteTexture, _map[spriteName]);
        }
        return sprite;
    }
}