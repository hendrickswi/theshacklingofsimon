using System.Numerics;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Items;

namespace TheShacklingOfSimon.Commands.Item_Commands_and_Temporary_Manager
{
	public class DropItemCommand : ICommand
	{
		private ItemManager itemManager;
		private PickupManager pickupManager;

		public DropItemCommand(ItemManager itemManager, PickupManager pickupManager)
		{
			this.itemManager = itemManager;
			this.pickupManager = pickupManager;
		}

		public void Execute()
		{
			Vector2 pos = Vector2.Zero;
			IPickup toDrop = itemManager.DropItem();
			if (toDrop != null)
			pickupManager.DropItem(toDrop);
			
		}
	}
}