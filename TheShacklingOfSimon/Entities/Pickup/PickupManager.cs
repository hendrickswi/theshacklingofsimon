#region

using System;
using System.Collections.Generic;

#endregion

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

    public void Clear()
    {
        _pickups.Clear();
    }

    public event Action<IPickup> OnPickupAdded;
}