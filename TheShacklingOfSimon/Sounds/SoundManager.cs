#region
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

#endregion

namespace TheShacklingOfSimon.Sounds;

public sealed class SoundManager
{
    private static SoundManager _instance = new SoundManager();
    public static SoundManager Instance => _instance;

    private readonly Dictionary<string, SoundEffect> _soundEffects;
    public SoundManager()
    {
        _soundEffects = new Dictionary<string, SoundEffect>();
    }

    public string NameSFX(string callType, string sfxName)
    {
        return "sounds/" + callType + "/" + sfxName;
    }

    public void AddSFX(string fullName)
    {
        SoundEffect toAdd = SoundFactory.Instance.GetSFX(fullName);
        if (toAdd != null && !_soundEffects.ContainsKey(fullName))
        {
            _soundEffects.Add(fullName, toAdd);
        }
    }

    public void RemoveSFX(string sfx)
    {
        if (sfx != null && _soundEffects.ContainsKey(sfx))
        {
            _soundEffects.Remove(sfx);
        }
    }

    public void PlaySFX(string sfx)
    {
        _soundEffects.GetValueOrDefault(sfx).Play();
        
    }
}