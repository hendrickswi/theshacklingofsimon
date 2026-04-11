#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Net.Http.Headers;

#endregion

namespace TheShacklingOfSimon.Sounds;

public class SoundFactory
{
    /*
     * Singleton factory to prevent client classes from instantiating
     * an instance of this.
     * Without this pattern it will be a huge performance penalty.
     *      i.e., every Entity instantiates an instance.
     */
    private static SoundFactory _instance = new SoundFactory();
    public static SoundFactory Instance => _instance;

    private Dictionary<string, SoundEffect> _sfxStorage;
    private Dictionary<string, Song> _songStorage;

    private SoundFactory()
    {
        _sfxStorage = new Dictionary<string, SoundEffect>();
        _songStorage = new Dictionary<string, Song>();
    }

    /// Loads a sound effect into the sound factory and associates it with
    /// the specified sfx name, alongside JSON frame data.
    /// <param name="content">
    /// The object of type <c>ContentManager</c> used to load the
    /// sound effect.
    /// </param>
    /// <param name="jsonPathName">
    /// The relative path, starting at the Content folder root, to the
    /// JSON file containing the sfx JSON metadata. 
    /// </param>
    /// <param name="soundName">
    /// The unique identifier or name to be associated with the sound effect.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if a sound with the given sound name already exists
    /// in the sound storage.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// Thrown if the specified JSON file does not exist at the resolved path.
    /// </exception>
    public SoundEffect LoadSFX(ContentManager content, string soundName)
    {
        SoundEffect sfx = content.Load<SoundEffect>(soundName);
        if (_sfxStorage.ContainsKey(soundName))
        {
            throw new ArgumentException("Key-value pair already exists for key " + soundName + ".");
        }
        _sfxStorage.Add(soundName, sfx);
        return sfx;
    }

    /// Loads a song into the sound factory and associates it with
    /// the specified song name.
    /// <param name="content">
    /// The object of type <c>ContentManager</c> used to load the
    /// song resource.
    /// </param>
    /// <param name="songFileName">
    /// The relative path, starting at the Content folder root, to the
    /// song file to be loaded.
    /// </param>
    /// <param name="songName">
    /// The unique identifier or name to be associated with the
    /// loaded song.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if a song with the given song name already exists
    /// in the song storage.
    /// </exception>
    public Song LoadSong(ContentManager content, string songName)
    {
        Song song = content.Load<Song>(SanitizeFilePath(songName));
        _songStorage.Add(songName, song);
        return song;
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
    public SoundEffect GetSFX(string soundName)
    {
        SoundEffect sfx = null;
        _sfxStorage.TryGetValue(soundName, out sfx);
        if (sfx == null)
        {
            Console.WriteLine("WARNING: SoundFactory could not find font " + soundName + "in SoundFactory.Instance.GetSFX(string soundName)");
        }
        return sfx;
    }
    
    private string SanitizeFilePath(string filePath)
    {
        // Replace all slashes from input with the correct OS-specific separators
        return filePath
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);
    }
}
