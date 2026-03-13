using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheShacklingOfSimon.Entities.Pickup;

public class PickupManager
{
    private readonly List<IPickup> _pickups;

    public PickupManager()
    {
        _pickups = new List<IPickup>();
    }
    
    public void AddPickup(IPickup pickup)
    {
        if (pickup != null && !_pickups.Contains(pickup))
        {
            _pickups.Add(pickup);
            OnPickupAdded?.Invoke(pickup);
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
        foreach (IPickup pickup in _pickups)
        {
            pickup.Update(gameTime);
        }
        _pickups.RemoveAll(p => !p.IsActive);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (IPickup pickup in _pickups)
        {
            pickup?.Draw(spriteBatch);
        }
    }

    public void ClearAllPickups()
    {
        _pickups.Clear();
    }

    public event Action<IPickup> OnPickupAdded;
}