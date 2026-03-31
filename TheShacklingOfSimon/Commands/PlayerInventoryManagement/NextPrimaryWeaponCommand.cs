using System.Collections.Generic;
using System.Linq;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Weapons;

namespace TheShacklingOfSimon.Commands.PlayerItem;

public class NextPrimaryWeaponCommand : ICommand
{
    private readonly IPlayer _player;

    public NextPrimaryWeaponCommand(IPlayer player)
    {
        this._player = player;
    }

    public void Execute()
    {
        List<IWeapon> weapons = _player.Inventory.Weapons.ToList();
        if (weapons.Count <= 1) return;

        IWeapon current = _player.Inventory.CurrentPrimaryWeapon;
        int currentIndex = weapons.IndexOf(current);
        int nextIndex = (currentIndex + 1 + weapons.Count ) % weapons.Count;

        _player.Inventory.CurrentPrimaryWeapon = weapons[nextIndex];
    }
}