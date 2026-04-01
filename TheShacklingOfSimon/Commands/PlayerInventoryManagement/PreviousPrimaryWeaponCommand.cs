using System.Collections.Generic;
using System.Linq;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Weapons;

namespace TheShacklingOfSimon.Commands.PlayerInventoryManagement;

public class PreviousPrimaryWeaponCommand : ICommand
{
    private readonly IPlayer _player;

    public PreviousPrimaryWeaponCommand(IPlayer player)
    {
        this._player = player;
    }

    public void Execute()
    {
        List<IPrimaryWeapon> weapons = _player.Inventory.Weapons.OfType<IPrimaryWeapon>().ToList();
        if (weapons.Count <= 1) return;

        IPrimaryWeapon current = _player.Inventory.CurrentPrimaryWeapon;
        int currentIndex = weapons.IndexOf(current);
        int nextIndex = (currentIndex - 1 + weapons.Count ) % weapons.Count;

        _player.Inventory.CurrentPrimaryWeapon = weapons[nextIndex];
    }
}