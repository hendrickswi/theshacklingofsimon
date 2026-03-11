using TheShacklingOfSimon.Entities.Pickup;

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
			pickupManager.DropItem(itemManager.DropItem());
			
		}
	}
}