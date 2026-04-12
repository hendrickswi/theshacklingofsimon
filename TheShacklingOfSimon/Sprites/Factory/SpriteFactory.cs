#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Factory.Data;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Sprites.Factory;

public class SpriteFactory
{
    /*
     * Singleton factory to prevent client classes from instantiating
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

    /// Loads a texture into the sprite factory and associates it with
    /// the specified sprite name, alongside JSON frame data.
    /// <param name="content">
    /// The object of type <c>ContentManager</c> used to load the
    /// sprite texture.
    /// </param>
    /// <param name="jsonPathName">
    /// The relative path, starting at the Content folder root, to the
    /// JSON file containing the sprite JSON metadata. 
    /// </param>
    /// <param name="spriteName">
    /// The unique identifier or name to be associated with the sprite and
    /// its texture.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if a texture with the given sprite name already exists
    /// in the texture storage.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// Thrown if the specified JSON file does not exist at the resolved path.
    /// </exception>
    public void LoadTexture(ContentManager content, string jsonPathName, string spriteName)
    {
        Texture2D texture = content.Load<Texture2D>(spriteName);
        if (_textureStorage.ContainsKey(spriteName))
        {
            throw new ArgumentException("Key-value pair already exists for key " + spriteName + ".");
        }
        _textureStorage.Add(spriteName, texture);
        
        string jsonPath = Path.Combine(content.RootDirectory, SanitizeFilePath(jsonPathName));
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

    /// Loads a font into the sprite factory and associates it with
    /// the specified font name.
    /// <param name="content">
    /// The object of type <c>ContentManager</c> used to load the
    /// font resource.
    /// </param>
    /// <param name="fontFileName">
    /// The relative path, starting at the Content folder root, to the
    /// font file to be loaded.
    /// </param>
    /// <param name="fontName">
    /// The unique identifier or name to be associated with the
    /// loaded font.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if a font with the given font name already exists
    /// in the font storage.
    /// </exception>
    public void LoadFont(ContentManager content, string fontFileName, string fontName)
    {
        if (_fontStorage.ContainsKey(fontName))
        {
            throw new ArgumentException("Key-value pair already exists for " + fontFileName + ".");
        }
        
        SpriteFont font = content.Load<SpriteFont>(SanitizeFilePath(fontFileName));
        _fontStorage.Add(fontName, font);
    }

    /// Creates an animated sprite based on the provided sprite name and
    /// animation speed.
    /// <param name="spriteName">
    /// The unique identifier or name used to locate the texture and
    /// animation frame data stored within the factory.
    /// </param>
    /// <param name="animationSpeed">
    /// The speed of the animation specified as frames per second.
    /// Higher values will make the animation play faster, while
    /// lower values will slow it down.
    /// </param>
    /// <remarks>
    /// The requested sprite must have previously been loaded into <c>this</c>
    /// using <c>LoadTexture(...)</c>.
    /// </remarks>
    /// <returns>
    /// An object implementing the <c>ISprite</c> interface, representing
    /// the animated sprite, or <c>null</c> if the sprite could not be created.
    /// </returns>
    public ISprite CreateAnimatedSprite(string spriteName, float animationSpeed)
    {
        ISprite sprite = null;
        bool textureExists = _textureStorage.TryGetValue(spriteName, out var texture);
        bool sourceRectanglesExist = _rectangleData.TryGetValue(spriteName, out var frames);

        if (textureExists && sourceRectanglesExist)
        {
            sprite = new AnimatedSprite(texture, frames, animationSpeed);
        }
        else
        {
            Console.WriteLine("WARNING: SpriteFactory could not find texture " + spriteName);
        }
        return sprite;
    }

    /// Creates a static sprite, using the provided sprite name to locate the previously
    /// loaded texture and JSON frame data.
    /// <param name="spriteName">
    /// The unique name of the sprite to create, which must be previously loaded into <c>this</c>.
    /// </param>
    /// <remarks>
    /// The requested sprite must have previously been loaded into <c>this</c>
    /// using <c>LoadTexture(...)</c>.
    /// </remarks>
    /// <returns>
    /// An instance of <c>ISprite</c> representing the static sprite, or <c>null</c> if
    /// no matching texture or rectangle data is found for the provided name.
    /// </returns>
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
        else
        {
            Console.WriteLine("WARNING: SpriteFactory could not find texture " + spriteName);
        }
        return sprite;
    }

    /// Creates a text sprite using the specified font and text content.
    /// <param name="fontName">
    /// The name of the font to be used for rendering the text.
    /// </param>
    /// <param name="text">
    /// The text content to be displayed by the sprite.
    /// </param>
    /// <remarks>
    /// The requested sprite must have previously been loaded into <c>this</c>
    /// using <c>LoadFont(...)</c>.
    /// </remarks>
    /// <returns>
    /// A text sprite implementing <c>ISprite</c> that renders the specified text
    /// using the provided font. Returns <c>null</c> if the font is not found
    /// in the factory's font storage.
    /// </returns>
    public ISprite CreateTextSprite(string fontName, string text)
    {
        ISprite sprite = null;
        bool fontExists = _fontStorage.TryGetValue(fontName, out var font);

        if (fontExists)
        {
            sprite = new TextSprite(font, text);
        }
        else
        {
            Console.WriteLine("WARNING: SpriteFactory could not find font " + fontName + " in SpriteFactory.Instance.CreateTextSprite(string fontFileName, string text)");
        }

        return sprite;
    }

    /// Retrieves a font from the font storage by the specified font name.
    /// <param name="fontName">
    /// The unique identifier or name of the font to retrieve from the font storage.
    /// </param>
    /// <remarks>
    /// The requested font must have previously been loaded into <c>this</c>
    /// using <c>LoadFont(...)</c>.
    /// </remarks>
    /// <returns>
    /// The <c>SpriteFont</c> object associated with the specified font name,
    /// or <c>null</c> if no font with the given name exists in the font storage.
    /// </returns>
    public SpriteFont GetFont(string fontName)
    {
        SpriteFont font = null;
        _fontStorage.TryGetValue(fontName, out font);
        if (font == null)
        {
            Console.WriteLine("WARNING: SpriteFactory could not find font " + fontName + "in SpriteFactory.Instance.GetFont(string fontName)");
        }
        return font;
    }
    
    private string SanitizeFilePath(string filePath)
    {
        // Replace all slashes from input with the correct OS-specific separators
        return filePath
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);
    }
}
