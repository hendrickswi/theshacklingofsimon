#region
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

#endregion

namespace TheShacklingOfSimon.Sounds;

public class SoundManager
{
    private readonly Dictionary<string, SoundEffect> _soundEffects;
    public SoundManager()
    {
        _soundEffects = new Dictionary<string, SoundEffect>();
    }

    public void AddSFX(string sfxName, SoundEffect sfx)
    {
        if (sfx != null && !_soundEffects.ContainsKey(sfxName))
        {
            _soundEffects.Add(sfxName, sfx);
        }
    }

    public void RemoveSFX(string sfx)
    {
        if (sfx != null && _soundEffects.ContainsKey(sfx))
        {
            _soundEffects.Remove(sfx);
        }
    }

    public void PlaySFX(SoundEffect sfx)
    {
        sfx.Play();
    }
}