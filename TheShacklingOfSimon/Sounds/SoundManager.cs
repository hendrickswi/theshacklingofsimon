using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace TheShacklingOfSimon.Sounds;

public class SoundManager // should we use this for music too? we could easily make a MusicManager but let me know
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
}