#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

#endregion

namespace TheShacklingOfSimon.Sounds;

public class SoundOptions
{
    private static SoundOptions _instance = new SoundOptions();
    public static SoundOptions Instance => _instance;

    public float SFXVol { get; protected set; }
    public float MusicVol { get; protected set; }
    private bool IsMuted = false;
    private float Increment = 0.1f;
    private string SFX = SoundManager.Instance.AddSFX("items","plop");

    public SoundOptions()
    {
        SFXVol = 1;
        MusicVol = 1;
    }

    public void ChangeSFXVol()
    {
        SoundManager.Instance.VolumeSFX(SFXVol);
    }

    public void ChangeMusicVol()
    {
        MediaPlayer.Volume = MusicVol;
    }

    private void Mute()
    {
        SoundManager.Instance.VolumeSFX(0);
        MediaPlayer.IsMuted = true;
        IsMuted = true;
    }
    private void Unmute()
    {
        SoundManager.Instance.VolumeSFX(SFXVol);
        MediaPlayer.IsMuted = false;
        IsMuted = false;
    }
    public void ToggleMute()
    {
        if (IsMuted)
            Unmute();
        else
            Mute();
    }

    public void IncSFX()
    {
        if (SFXVol < 1 - Increment) SFXVol += Increment;
        else SFXVol = 1;
        ChangeSFXVol();
        if (SFXVol != 1) SoundManager.Instance.PlaySFX(SFX);
    }
    public void DecSFX()
    {
        if (SFXVol > Increment) SFXVol -= Increment;
        else SFXVol = 0;
        ChangeSFXVol();
        if (SFXVol != 0) SoundManager.Instance.PlaySFX(SFX);
    }
    public void IncMusic()
    {
        if (MusicVol < 1 - Increment) MusicVol += Increment;
        else MusicVol = 1;
        ChangeMusicVol();
    }
    public void DecMusic()
    {
        if (MusicVol > Increment) MusicVol -= Increment;
        else MusicVol = 0;
        ChangeMusicVol();
    }
}