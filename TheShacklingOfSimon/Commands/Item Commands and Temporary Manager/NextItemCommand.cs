using TheShacklingOfSimon.Item_Manager;

namespace TheShacklingOfSimon.Commands
{
	public class NextItemCommand : ICommand
	{
		private ItemManager itemManager;

		public NextItemCommand(ItemManager itemManager)
		{
			this.itemManager = itemManager;
		}

		public void Execute()
		{ 
			itemManager.NextItem();
		}
	}
}