#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

#endregion

namespace TheShacklingOfSimon.Sounds;

public sealed class SoundManager
{
    private static SoundManager _instance = new SoundManager();
    public static SoundManager Instance => _instance;

    private readonly Dictionary<string, SoundEffectInstance> _soundEffects;
    public SoundManager()
    {
        _soundEffects = new Dictionary<string, SoundEffectInstance>();
    }

    public string AddSFX(string callType, string sfxName)
    {
        string fullName = "sounds/" + callType + "/" + sfxName;
        SoundEffect toAdd = SoundFactory.Instance.GetSFX(fullName);
        if (toAdd != null && !_soundEffects.ContainsKey(fullName))
        {
            _soundEffects.Add(fullName, toAdd.CreateInstance());
        }
        return fullName;
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
        if (string.IsNullOrEmpty(sfx))
        {
            Console.WriteLine("WARNING: PlaySFX called with null or empty string.");
            return;
        }

        SoundEffectInstance effect = _soundEffects.GetValueOrDefault(sfx);
        if (effect == null)
        {
            Console.WriteLine($"WARNING: No sound effect found for key: {sfx}");
            return;
        }

        effect.Play();
    }

    public void StopSFX(string sfx)
    {
        if (string.IsNullOrEmpty(sfx))
        {
            Console.WriteLine("WARNING: PlaySFX called with null or empty string.");
            return;
        }

        SoundEffectInstance effect = _soundEffects.GetValueOrDefault(sfx);
        if (effect == null)
        {
            Console.WriteLine($"WARNING: No sound effect found for key: {sfx}");
            return;
        }

        effect.Stop();
    }

    public void StopAllSFX()
    {
        foreach(KeyValuePair<string, SoundEffectInstance> sfx in _soundEffects)
        {
            sfx.Value.Stop(true);
        }
    }
}