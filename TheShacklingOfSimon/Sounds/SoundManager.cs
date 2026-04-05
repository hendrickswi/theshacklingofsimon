#region

using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

#endregion

namespace TheShacklingOfSimon.Sounds;

public class SoundManager
{
    private readonly List<SoundEffect> _soundEffects;
    public SoundManager()
    {
        _soundEffects = new List<SoundEffect>();
    }
    public void AddSFX(SoundEffect sfx)
    {
        if (sfx != null && !_soundEffects.Contains(sfx))
        {
            _soundEffects.Add(sfx);
        }
    }

    public void RemoveSFX(SoundEffect sfx)
    {
        if (sfx != null)
        {
            _soundEffects.Remove(sfx);
        }
    }

    public void PlaySFX(SoundEffect sfx)
    {
        sfx.Play();
    }
}