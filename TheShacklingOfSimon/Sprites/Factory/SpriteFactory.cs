using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Factory.Data;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Sprites.Factory;

public class SpriteFactory
{
    /*
     * Singleton "antipattern" to prevent client classes from instantiating
     * an instance of this.
     * Without this pattern it will be a huge performance penalty.
     *      i.e., every Entity instantiates an instance.
     */
    private static SpriteFactory _instance = new SpriteFactory();
    public static SpriteFactory Instance => _instance;

    private Dictionary<string, Texture2D> _textureStorage;
    private Dictionary<string, SpriteFont> _fontStorage;
    private Dictionary<string, Rectangle[]> _rectangleData;

    private SpriteFactory()
    {
        _textureStorage = new Dictionary<string, Texture2D>();
        _fontStorage = new Dictionary<string, SpriteFont>();
        _rectangleData = new Dictionary<string, Rectangle[]>();
        
    }
    
    /*
     * Loads a sprite sheet png and corresponding JSON into memory.
     */
    public void LoadTexture(ContentManager content, string jsonPathName, string spriteName)
    {
        Texture2D texture = content.Load<Texture2D>(spriteName);
        if (_textureStorage.ContainsKey(spriteName))
        {
            throw new ArgumentException("Key-value pair already exists for key " + spriteName + ".");
        }
        _textureStorage.Add(spriteName, texture);
        
        string jsonPath = Path.Combine(content.RootDirectory, jsonPathName);
        if (!File.Exists(jsonPath))
        {
            // Prevent "ghost" instances
            _textureStorage.Remove(spriteName);
            throw new FileNotFoundException("Could not find jsonPathName at " + jsonPath + ".");
        }
        
        // Read JSON file contents
        string jsonContent = File.ReadAllText(jsonPath);

        // Deserialize file contents
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        SpriteDataRoot data = JsonSerializer.Deserialize<SpriteDataRoot>(jsonContent, options);
        
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
            _rectangleData.TryAdd(sprite.Name, frames);
            _textureStorage.TryAdd(sprite.Name, texture);
        }
    }

    public void LoadFont(ContentManager content, string fontFileName)
    {
        if (_fontStorage.ContainsKey(fontFileName))
        {
            throw new ArgumentException("Key-value pair already exists for " + fontFileName + ".");
        }
        _fontStorage.Add(fontFileName, content.Load<SpriteFont>(fontFileName));
    }

    public ISprite CreateAnimatedSprite(string spriteName)
    {
        ISprite sprite = null;
        bool textureExists = _textureStorage.TryGetValue(spriteName, out var texture);
        bool sourceRectanglesExist = _rectangleData.TryGetValue(spriteName, out var frames);

        if (textureExists && sourceRectanglesExist)
        {
            sprite = new AnimatedSprite(texture, frames);
        }
        return sprite;
    }
    
    public ISprite CreateStaticSprite(string spriteName)
    {
        ISprite sprite = null;
        bool textureExists = _textureStorage.TryGetValue(spriteName, out var texture);
        bool sourceRectangleExists = _rectangleData.TryGetValue(spriteName, out var frame);

        if (textureExists && sourceRectangleExists)
        {
            if (frame.Length > 0)
            {
                sprite = new StaticSprite(texture, frame[0]);
            }
        }
        return sprite;
    }
    
    public ISprite CreateTextSprite(string fontFileName, string text)
    {
        ISprite sprite = null;
        bool fontExists = _fontStorage.TryGetValue(fontFileName, out var font);

        if (fontExists)
        {
            sprite = new TextSprite(font, text);
        }

        return sprite;
    }
}