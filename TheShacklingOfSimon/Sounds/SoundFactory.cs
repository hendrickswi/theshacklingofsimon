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
using System.ComponentModel;

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
    /// <param name="soundName">
    /// The unique identifier or name to be associated with the sound effect.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if a sound with the given sound name already exists
    /// in the sound storage.
    /// </exception>
    public void LoadSFX(ContentManager content, string soundName)
    {
        SoundEffect sfx = content.Load<SoundEffect>(soundName);
        if (_sfxStorage.ContainsKey(soundName))
        {
            throw new ArgumentException("Key-value pair already exists for key " + soundName + ".");
        }
        _sfxStorage.Add(soundName, sfx);
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
    public void LoadSong(ContentManager content, string songName)
    {
        Song song = content.Load<Song>(SanitizeFilePath(songName));
        _songStorage.Add(songName, song);
    }

    /// Retrieves all sounds from loaded sound storage.
    /// <returns>
    /// A <c>List<SoundEffect></c> object containing all
    /// <c>SoundEffect</c> objects in sound storage
    /// </returns>
    public List<SoundEffect> GetAllSFX()
    {
        List<SoundEffect> sounds = null;
        foreach(KeyValuePair<string, SoundEffect> x in _sfxStorage)
        {
            sounds.Add(x.Value);
        }
        return sounds;
    }
    
    private string SanitizeFilePath(string filePath)
    {
        // Replace all slashes from input with the correct OS-specific separators
        return filePath
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);
    }
}
