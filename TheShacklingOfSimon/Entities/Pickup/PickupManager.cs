using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomClass;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Entities.Pickup;

public class PickupManager
{
    private List<IPickup> _pickups = new List<IPickup>();
    private Room _room;
    ISprite temp;

    public PickupManager(Room room, SpriteFactory spriteFactory)
    {
        temp = spriteFactory.CreateStaticSprite("images/8Ball");
        _room = room;
    }
    public void AddPickup(IPickup pickup)
        {
            if (pickup != null && !_pickups.Contains(pickup))
            {
                _pickups.Add(pickup);
            }
        }

        public void RemovePickup(IPickup pickup)
        {
            if (pickup != null)
            {
                _pickups.Remove(pickup);
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = _pickups.Count - 1; i >= 0; i--)
            {
                IPickup pickup = _pickups[i];
                pickup.Update(gameTime);

                if (pickup.IsActive == false)
                {
                    _pickups.RemoveAt(i);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (IPickup pickup in _pickups)
            {
                pickup.Draw(spriteBatch);
            }
        }

        public void DropItem(IPickup toDrop)
        {
            if(toDrop is NoneItem) return;
            AddPickup(toDrop);
        }

        public void ClearAllPickups()
        {
            _pickups.Clear();
        }
}