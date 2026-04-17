#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Items.Active_Items;
using TheShacklingOfSimon.Weapons;

#endregion

namespace TheShacklingOfSimon.Entities;

public class Inventory
{
    protected readonly ISet<IWeapon> _weapons;
    protected readonly ISet<IItem> _items;

    // Read-only public accessors (for GUI, etc.)
    public IEnumerable<IWeapon> Weapons => _weapons;
    public IEnumerable<IItem> Items => _items;
    
    public Inventory()
    {
        _weapons = new HashSet<IWeapon>();
        _items = new HashSet<IItem>();
    }
    
    public bool Add(IWeapon w)
    {
        if (w == null) return false;
        bool result = _weapons.Add(w);

        if (result)
        {
            NotifyInventoryChanged();
        }

        return result;
    }
    
    public bool Remove(IWeapon w)
    {
        if (w == null) return false;
        bool result = _weapons.Remove(w);

        if (result)
        {
            NotifyInventoryChanged();
        }

        return result;
    }
    
    public bool Add(IItem item)
    {
        if (item == null) return false;
        bool result = _items.Add(item);

        if (result)
        {
            NotifyInventoryChanged();
        }

        return result;
    }

    public bool Remove(IItem item)
    {
        if (item == null) return false;
        bool result = _items.Remove(item);

        if (result)
        {
            NotifyInventoryChanged();
        }
        
        return result;
    }

    public bool Contains(IWeapon weapon)
    {
        return _weapons.Contains(weapon);
    }
    
    public bool Contains(IItem item)
    {
        return _items.Contains(item);
    }

    public void Update(GameTime delta)
    {
        foreach (IItem item in _items)
        {
            if (item is not IActiveItem castedItem) continue;
            castedItem.Update(delta);
        }
    }

    public void Clear()
    {
        _weapons.Clear();
        _items.Clear();
        NotifyInventoryChanged();
    }

    protected void NotifyInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }

    // Allow observer pattern to prevent 60 polls per second
    public event Action OnInventoryChanged;
}
