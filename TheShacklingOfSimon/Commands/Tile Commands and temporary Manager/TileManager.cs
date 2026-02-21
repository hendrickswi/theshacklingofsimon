using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Level_Handler.Tiles;
using TheShacklingOfSimon.Sprites.Factory;

namespace TheShacklingOfSimon.Commands.Tile_Commands_and_temporary_Manager;

public class TileManager
{
	private readonly List<ITile> tiles;
	private int currentIndex;

	public TileManager(SpriteFactory spriteFactory)
	{
		tiles = new List<ITile>();

		var rockSprite = spriteFactory.CreateStaticSprite("images/Rocks");
		var spikeSprite = spriteFactory.CreateStaticSprite("images/Spikes");

		Vector2 center = new Vector2(400, 300);

		tiles.Add(new RockTile(rockSprite, center));
		tiles.Add(new SpikeTile(spikeSprite, center));

		currentIndex = 0;
	}

	public void NextTile()
	{
		currentIndex = (currentIndex + 1) % tiles.Count;
	}

	public void PreviousTile()
	{
		currentIndex--;
		if (currentIndex < 0)
			currentIndex = tiles.Count - 1;
	}

	public void Update(GameTime gameTime)
	{
		tiles[currentIndex].Update(gameTime);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		tiles[currentIndex].Draw(spriteBatch);
	}
}