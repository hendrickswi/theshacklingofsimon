using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Controllers;
using TheShacklingOfSimon.Controllers.Keyboard;
using TheShacklingOfSimon.Controllers.Mouse;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Keyboard;
using TheShacklingOfSimon.Input.Mouse;
using TheShacklingOfSimon.Room_Manager;
using TheShacklingOfSimon.Item_Manager;
using TheShacklingOfSimon.Projectiles;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Weapons;
using KeyboardInput = TheShacklingOfSimon.Controllers.Keyboard.KeyboardInput;

namespace TheShacklingOfSimon;

public class Game1 : Game
{
	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;
	private Texture2D _texture;
	private SpriteFont _font;

	private IController<KeyboardInput> _keyboardController;
	private IController<MouseInput> _mouseController;
	private TileManager _tileManager; //Temporary tile switching for sprint 2
	private ItemManager _itemManager; //Temporary item switching for sprint 2

	private IPlayer _player;
	private List<IEntity> _entities;
	private ProjectileManager _projectileManager; //_projectileManager = new ProjectileManager();



	public Game1()
	{
		_graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
	}

	protected override void Initialize()
	{
		_keyboardController = new KeyboardController(new MonoGameKeyboardService());
		_mouseController = new MouseController(new MonoGameMouseService());
		base.Initialize();
	}

	protected override void LoadContent()
	{
		Rectangle screenDimensions = GraphicsDevice.Viewport.Bounds;

		_spriteBatch = new SpriteBatch(GraphicsDevice);

		// Create Content pipeline 
		_font = Content.Load<SpriteFont>("File");

		// Load Player Sprites into factory 
		SpriteFactory.Instance.LoadTexture(Content, "PlayerDefaultSprite.json", "player");

		// Load Projectile Sprites and Manager_projectileManager.Update(delta);

		_projectileManager = new ProjectileManager();


		//load Tile Sprites and Manager
		SpriteFactory.Instance.LoadTexture(Content, "images/Rocks.json", "images/Rocks");
		SpriteFactory.Instance.LoadTexture(Content, "images/Spikes.json", "images/Spikes");

		_tileManager = new TileManager(SpriteFactory.Instance);

		//load Item Sprites and manager
		SpriteFactory.Instance.LoadTexture(Content, "images/8Ball.json", "images/8Ball");
		SpriteFactory.Instance.LoadTexture(Content, "images/Red_Heart.json", "images/Red_Heart");

		_itemManager = new ItemManager(SpriteFactory.Instance);

		// Create entities now that the sprite factory has textures
		_entities = new List<IEntity>();
		_player =
			new PlayerWithTwoSprites(new Vector2(screenDimensions.Width * 0.5f, screenDimensions.Height * 0.5f));
		_entities.Add(_player);

		_player.AddWeaponToInventory(new BasicWeapon(_projectileManager));
		_player.EquipPrimaryWeapon(0);

		// Register controls now that the player exists
		RegisterControls(screenDimensions);


	}

	protected override void Update(GameTime delta)
	{
		_keyboardController.Update();
		_mouseController.Update();

		/*
         * Add various other things that need to be updated.
         */

		_projectileManager.Update(delta);
		_tileManager.Update(delta);
		_itemManager.Update(delta);

		foreach (IEntity e in _entities)
		{
			e.Update(delta);
		}
		base.Update(delta);

	}

	protected override void Draw(GameTime delta)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);
		_spriteBatch.Begin();

		/*
         * Add various other things that need to be drawn
         *      e.g., ITile objects, GUI, etc.
         */
		_projectileManager.Draw(_spriteBatch);
		_tileManager.Draw(_spriteBatch);
		_itemManager.Draw(_spriteBatch);

		foreach (IEntity e in _entities)
		{
			e.Draw(_spriteBatch);
		}

		_spriteBatch.End();
		base.Draw(delta);
	}

	private void RegisterControls(Rectangle screenDimensions)
	{
		/*
         * Controls are initialized here using RegisterCommand()
         * Use a KeyboardInput or MouseInput struct to register an input for mouse/keyboard
         * Then use some ICommand concrete class to register *what* that input does.
         *
         * e.g., _keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.Escape), new ExitCommand(this));
         * or _mouseController.RegisterCommand(
         *      new MouseInput(new InputRegion(0, 0, screenDimensions.Width, screenDimensions.Height), BinaryInputState.Pressed, MouseButton.Right), 
         *      new ExitCommand(this));
         * to register the D0 key and right click to exit the game.
         */
		// Movement controls
		_keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.W), new MoveUpCommand(_player));
		_keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.A), new MoveLeftCommand(_player));
		_keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.S), new MoveDownCommand(_player));
		_keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.D), new MoveRightCommand(_player));

		// Attacking controls
		_keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.E), new SecondaryAttackNeutralCommand(_player));
		_keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.LeftShift), new SecondaryAttackNeutralCommand(_player));
		_keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.RightShift), new SecondaryAttackNeutralCommand(_player));
		_keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.Up), new PrimaryAttackUpCommand(_player));
		_keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.Left), new PrimaryAttackLeftCommand(_player));
		_keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.Down), new PrimaryAttackDownCommand(_player));
		_keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.Right), new PrimaryAttackRightCommand(_player));

		//Tile Manager Controls
		_keyboardController.RegisterCommand(
			new KeyboardInput(BinaryInputState.JustPressed, KeyboardButton.Y),
			new NextTileCommand(_tileManager));

		_keyboardController.RegisterCommand(
			new KeyboardInput(BinaryInputState.JustPressed, KeyboardButton.T),
			new PreviousTileCommand(_tileManager));

		//Item Manager Controls
		_keyboardController.RegisterCommand(
			new KeyboardInput(BinaryInputState.JustPressed, KeyboardButton.I),
			new NextItemCommand(_itemManager));

		_keyboardController.RegisterCommand(
			new KeyboardInput(BinaryInputState.JustPressed, KeyboardButton.U),
			new PreviousItemCommand(_itemManager));

		//Mouse controls
		_mouseController.RegisterCommand(
			new MouseInput(
				new InputRegion(0, 0, screenDimensions.Width, screenDimensions.Height),
				BinaryInputState.Pressed,
				MouseButton.Right),
			new SecondaryAttackNeutralCommand(_player));
		_mouseController.RegisterCommand(
			new MouseInput(
				new InputRegion(0, 0, screenDimensions.Width, screenDimensions.Height),
				BinaryInputState.Pressed,
				MouseButton.Left),
			new PrimaryAttackDynamicMouseCommand(_player)
			);



		_keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.Escape), new ExitCommand(this));
	}
}

