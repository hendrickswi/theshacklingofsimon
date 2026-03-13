using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Commands.Item_Commands_and_Temporary_Manager
{
	public class SpawnPickupCommand : ICommand
	{
		private readonly Func<IPickup> _pickupFactory;
		private readonly PickupManager _pickupManager;

		public SpawnPickupCommand(Func<IPickup> factory, PickupManager pickupManager)
		{
			_pickupFactory = factory;
			_pickupManager = pickupManager;
		}

		public void Execute()
		{
			IPickup pickup = _pickupFactory.Invoke();
			_pickupManager.AddPickup(pickup);
		}
	}
}