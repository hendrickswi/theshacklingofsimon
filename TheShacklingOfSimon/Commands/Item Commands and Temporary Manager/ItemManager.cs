using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Level_Handler.Tiles;
using TheShacklingOfSimon.Sprites.Factory;

namespace TheShacklingOfSimon.Commands.Item_Commands_and_Temporary_Manager;

public class ItemManager
{
    private readonly IPlayer _player;
    private readonly List<ITile> _icons; // icon for each inventory item slot
    private int _currentIndex;

    public ItemManager(IPlayer player, SpriteFactory spriteFactory)
    {
        _player = player;
        _icons = new List<ITile>();
        _currentIndex = 0;

        BuildIcons(spriteFactory);

        if (_player.Inventory.Items.Count > 0)
            _player.EquipItem(_currentIndex);
    }

    // Call this after you add/remove items at runtime (optional)
    public void Rebuild(SpriteFactory spriteFactory)
    {
        _icons.Clear();
        _currentIndex = 0;
        BuildIcons(spriteFactory);

        if (_player.Inventory.Items.Count > 0)
            _player.EquipItem(_currentIndex);
    }

    private void BuildIcons(SpriteFactory spriteFactory)
    {
        Vector2 center = new Vector2(500, 400);

        // Build one icon per inventory slot, same ordering as Inventory.Items
        for (int i = 0; i < _player.Inventory.Items.Count; i++)
        {
            IItem item = _player.Inventory.Items[i];

            // Map item type corresponds to icon sprite
            // TeleportItem corresponds to 8Ball, AdrenalineItem corresponds to Red_Heart
            var sprite = item switch
            {
                TeleportItem => spriteFactory.CreateStaticSprite("images/8Ball"),
                AdrenalineItem => spriteFactory.CreateStaticSprite("images/Red_Heart"),
                _ => spriteFactory.CreateStaticSprite("images/Red_Heart") // fallback icon
            };

            // Use whatever tile type to draw an icon
            // RockTile is fine since it's just a drawable wrapper for now
            _icons.Add(new RockTile(sprite, center));
        }
    }

    public void NextItem()
    {
        int count = _player.Inventory.Items.Count;
        if (count == 0) return;

        _currentIndex = (_currentIndex + 1) % count;
        _player.EquipItem(_currentIndex);
    }

    public void PreviousItem()
    {
        int count = _player.Inventory.Items.Count;
        if (count == 0) return;

        _currentIndex = (_currentIndex - 1 + count) % count;
        _player.EquipItem(_currentIndex);
    }

    public void UseCurrent()
    {
        _player.CurrentItem?.Effect();
    }

    public void Update(GameTime gameTime)
    {
        int count = _icons.Count;
        if (count > 0)
            _icons[_currentIndex].Update(gameTime);

        // ticks cooldown/duration on the equipped active item
        _player.CurrentItem?.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_icons.Count > 0)
            _icons[_currentIndex].Draw(spriteBatch);
    }
}