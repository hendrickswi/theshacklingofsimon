using TheShacklingOfSimon.Item_Manager;

namespace TheShacklingOfSimon.Commands
{
	public class PreviousItemCommand : ICommand
	{
		private ItemManager itemManager;

		public PreviousItemCommand(ItemManager itemManager)
		{
			this.itemManager = itemManager;
		}

		public void Execute()
		{
			itemManager.PreviousItem();
		}
	}
}