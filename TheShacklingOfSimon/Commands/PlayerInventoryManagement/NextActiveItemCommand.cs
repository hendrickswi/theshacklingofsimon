using System.Collections.Generic;
using System.Linq;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Items;

namespace TheShacklingOfSimon.Commands.PlayerItem
{
	public class NextActiveItemCommand : ICommand
	{
		private readonly IPlayer _player;

		public NextActiveItemCommand(IPlayer player)
		{
			_player = player;
		}

		public void Execute()
		{
			List<IItem> items = _player.Inventory.Items.ToList();
			if (items.Count <= 1) return;

			IItem current = _player.Inventory.CurrentActiveItem;
			int currentIndex = items.IndexOf(current);
			int nextIndex = (currentIndex + 1 + items.Count) % items.Count;

			_player.Inventory.CurrentActiveItem = items[nextIndex];
		}
	}
}