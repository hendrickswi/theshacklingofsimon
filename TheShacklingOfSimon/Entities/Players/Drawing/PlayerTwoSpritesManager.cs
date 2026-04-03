#region

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Players.Config;
using TheShacklingOfSimon.Entities.Players.States.Body;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Entities.Players.Drawing;

public class PlayerTwoSpritesManager
{
    public Vector2 HeadOffset { get; private set; }
    public Vector2 DamagedStateOffset { get; private set; }
    public float DeathFrameDuration { get; private set; }
    public float HurtFrameDuration { get; private set; }
    public float MovementFrameDuration { get; private set; }
    
    public ISprite Head { get; set; }
    public ISprite Body { get; set; }

    private PlayerWithTwoSprites _player;
    private Dictionary<string, string> _skins;

    public PlayerTwoSpritesManager(PlayerWithTwoSprites player)
    {
        _player = player;
        
        _skins = ConfigDBPlayer.Configs["PlayerWithTwoSprites"].SkinsDictionary;
        HeadOffset = ConfigDBPlayer.Configs["PlayerWithTwoSprites"].HeadOffset;
        DamagedStateOffset = ConfigDBPlayer.Configs["PlayerWithTwoSprites"].DamagedStateOffset;
        DeathFrameDuration = ConfigDBPlayer.Configs["PlayerWithTwoSprites"].DeathFrameDuration;
        HurtFrameDuration = ConfigDBPlayer.Configs["PlayerWithTwoSprites"].HurtFrameDuration;
        MovementFrameDuration = ConfigDBPlayer.Configs["PlayerWithTwoSprites"].MovementFrameDuration;
        
        Head = null;
        Body = null;
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
    
    public void Update(GameTime delta)
    {
        Head?.Update(delta);
        Body?.Update(delta);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        SpriteEffects flip = SpriteEffects.None;
        if (_player.Velocity.X < -0.0001f)
        {
            flip = SpriteEffects.FlipHorizontally;
        }
        Vector2 drawPos = (_player.StatesManager.Body is PlayerBodyDamagedState) ? _player.Position + DamagedStateOffset : _player.Position;
        Body?.Draw(spriteBatch, drawPos, Color.White, 0.0f,
            new Vector2(0, 0), 1.0f, flip, 0.0f);
        
        Head?.Draw(spriteBatch, _player.Position + HeadOffset, Color.White);
    }
}