#region

using System;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace TheShacklingOfSimon.LevelHandler.Tiles.Border.Doors
{
    // I keep both upright door textures together and let DoorTile rotate them by side.
    public sealed class DoorTextureSet
    {
        public Texture2D LockedUpTexture { get; }
        public Texture2D UnlockedUpTexture { get; }

        public DoorTextureSet(Texture2D lockedUpTexture, Texture2D unlockedUpTexture)
        {
            LockedUpTexture = lockedUpTexture ?? throw new ArgumentNullException(nameof(lockedUpTexture));
            UnlockedUpTexture = unlockedUpTexture ?? throw new ArgumentNullException(nameof(unlockedUpTexture));
        }
    }
}