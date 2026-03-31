#region

using System;
using TheShacklingOfSimon.Entities.Pickup;

#endregion

namespace TheShacklingOfSimon.Commands
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