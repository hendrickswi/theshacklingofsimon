using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Level_Handler.Tiles;
using TheShacklingOfSimon.Sprites.Factory;

namespace TheShacklingOfSimon.Commands.Item_Commands_and_Temporary_Manager;

public class ItemManager
{
	private readonly List<ITile> items;
	private int currentIndex;

	public ItemManager(SpriteFactory spriteFactory)
	{
		items = new List<ITile>();

		var rockSprite = spriteFactory.CreateStaticSprite("images/8Ball");
		var spikeSprite = spriteFactory.CreateStaticSprite("images/Red_Heart");

		Vector2 center = new Vector2(500, 400);

		items.Add(new RockTile(rockSprite, center));
		items.Add(new RockTile(spikeSprite, center));

		currentIndex = 0;
	}

	public void NextItem()
	{
		currentIndex = (currentIndex + 1) % items.Count;
	}

	public void PreviousItem()
	{
		currentIndex--;
		if (currentIndex < 0)
			currentIndex = items.Count - 1;
	}

	public void Update(GameTime gameTime)
	{
		items[currentIndex].Update(gameTime);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		items[currentIndex].Draw(spriteBatch);
	}
}