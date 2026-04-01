#region

using System;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Weapons;

#endregion

namespace TheShacklingOfSimon.Entities.Players;

public class PlayerInventory : Inventory
{
    private IPrimaryWeapon _currentPrimaryWeapon;
    private ISecondaryWeapon _currentSecondaryWeapon;
    private IItem _currentActiveItem;
    private int _numKeys;
    private int _numCoins;

    public IPrimaryWeapon CurrentPrimaryWeapon
    {
        get { return _currentPrimaryWeapon; }
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            
            if (!_weapons.Contains(value))
            {
                throw new ArgumentException("Provided IWeapon argument does not exist within the inventory.",
                    nameof(value));
            }
            
            if (_currentPrimaryWeapon != null && _currentPrimaryWeapon == value)
            {
                return;
            }
            
            _currentPrimaryWeapon = value;
            NotifyInventoryChanged();
        }
    }

    public ISecondaryWeapon CurrentSecondaryWeapon
    {
        get { return _currentSecondaryWeapon;  }
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            
            if (!_weapons.Contains(value))
            {
                throw new ArgumentException("Provided IWeapon arguemnt does not exist within the inventory.",
                    nameof(value));
            }

            if (_currentSecondaryWeapon != null && _currentSecondaryWeapon == value)
            {
                return;
            }
            
            _currentSecondaryWeapon = value;
            NotifyInventoryChanged();
        }
    }

    public IItem CurrentActiveItem
    {
        get { return _currentActiveItem; }
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            
            if (!_items.Contains(value))
            {
                throw new ArgumentException("Provided IWeapon arguemnt does not exist within the inventory.",
                    nameof(value));
            }

            if (_currentActiveItem != null && _currentActiveItem == value)
            {
                return;
            }
            
            _currentActiveItem = value;
            NotifyInventoryChanged();
        }
    }

    public int NumKeys
    {
        get { return _numKeys; }
        set
        {
            _numKeys = value;
            NotifyInventoryChanged();
        }
    }

    public int NumCoins
    {
        get { return _numCoins;  }
        set
        {
            _numCoins = value;
            NotifyInventoryChanged();
        }
    }
}
