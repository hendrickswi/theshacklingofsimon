using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players.Config;

namespace TheShacklingOfSimon.Entities.Players;

public class PlayerDrawManager
{
    private Dictionary<string, string> _skins;
    public Vector2 HeadOffset { get; private set; }
    public Vector2 DamagedStateOffset { get; private set; }
    public float DeathFrameDuration { get; private set; }
    public float HurtFrameDuration { get; private set; }
    public float MovementFrameDuration { get; private set; }

    public PlayerDrawManager()
    {
        _skins = ConfigDBPlayer.Configs["PlayerWithTwoSprites"].SkinsDictionary;
        HeadOffset = ConfigDBPlayer.Configs["PlayerWithTwoSprites"].HeadOffset;
        DamagedStateOffset = ConfigDBPlayer.Configs["PlayerWithTwoSprites"].DamagedStateOffset;
        DeathFrameDuration = ConfigDBPlayer.Configs["PlayerWithTwoSprites"].DeathFrameDuration;
        HurtFrameDuration = ConfigDBPlayer.Configs["PlayerWithTwoSprites"].HurtFrameDuration;
        MovementFrameDuration = ConfigDBPlayer.Configs["PlayerWithTwoSprites"].MovementFrameDuration;
    }

    /// <summary>
    /// Retrieves the skin prefix associated with the specified category.
    /// </summary>
    /// <param name="category">The category of the skin to retrieve.</param>
    /// <returns>The skin prefix if the category exists; otherwise, an empty string.</returns>
    public string GetSkin(string category)
    {
        return _skins.ContainsKey(category) ? _skins[category] : "";
    }

    /// <summary>
    /// Sets the skin prefix for the specified category.
    /// </summary>
    /// <param name="category">The category of the skin to set.</param>
    /// <param name="skinPrefix">The prefix of the skin to associate with the category.</param>
    public void SetSkin(string category, string skinPrefix)
    {
        if (_skins.ContainsKey(category))
        {
            _skins[category] = skinPrefix;
        }
    }
}