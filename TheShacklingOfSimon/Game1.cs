using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Commands.Item_Commands_and_Temporary_Manager;
using TheShacklingOfSimon.Commands.PlayerAttack;
using TheShacklingOfSimon.Commands.PlayerMovement;
using TheShacklingOfSimon.Commands.Temporary_Commands;
using TheShacklingOfSimon.Commands.Tile_Commands_and_temporary_Manager;
using TheShacklingOfSimon.Controllers;
using TheShacklingOfSimon.Controllers.Keyboard;
using TheShacklingOfSimon.Controllers.Mouse;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Keyboard;
using TheShacklingOfSimon.Input.Mouse;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Items.Active_Items;
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
	
	private IKeyboardService _keyboardService;
	private IController<KeyboardInput> _keyboardController;
	private IMouseService _mouseService;
	private IController<MouseInput> _mouseController;
	
	private TileManager _tileManager; //Temporary tile switching for sprint 2
	private ItemManager _itemManager; //Temporary item switching for sprint 2

	private IPlayer _player;
	private List<IEntity> _entities;
	private ProjectileManager _projectileManager; //_projectileManager = new ProjectileManager();

	//temp for enemy
	private EnemyManager _enemyManager;
	private IEnemy _spiderEnemy1;
	private IEnemy _spiderEnemy2;
	private IEnemy _spiderEnemy3;


	public Game1()
	{
		_graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
	}

	protected override void Initialize()
	{
		_keyboardService = new MonoGameKeyboardService();
		_keyboardController = new KeyboardController(_keyboardService);
		
		_mouseService = new MonoGameMouseService();
		_mouseController = new MouseController(_mouseService);
		
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

		// Load Enemy Sprites into factory
		SpriteFactory.Instance.LoadTexture(Content, "SpiderEnemy.json", "SpiderEnemy");

		// Load Projectile Sprites and Manager_projectileManager.Update(delta);

		_projectileManager = new ProjectileManager();


		//load Tile Sprites and Manager
		SpriteFactory.Instance.LoadTexture(Content, "images/Rocks.json", "images/Rocks");
		SpriteFactory.Instance.LoadTexture(Content, "images/Spikes.json", "images/Spikes");
        SpriteFactory.Instance.LoadTexture(Content, "images/Fire.json", "images/Fire");


        _tileManager = new TileManager(SpriteFactory.Instance);

		// Create entities now that the sprite factory has textures
		_entities = new List<IEntity>();
		_player =
			new PlayerWithTwoSprites(new Vector2(screenDimensions.Width * 0.5f, screenDimensions.Height * 0.5f));
		_entities.Add(_player);

		_player.AddWeaponToInventory(new BasicWeapon(_projectileManager));
		_player.EquipPrimaryWeapon(0);
		
		_player.AddWeaponToInventory(new BombWeapon(_projectileManager));
		_player.EquipSecondaryWeapon(1);

        // temporary items for demo
        _player.AddItemToInventory(new TeleportItem(_player, pos => true));
        _player.AddItemToInventory(new AdrenalineItem(_player));

		_spiderEnemy1 = new SpiderEnemy(new Vector2(screenDimensions.Width * 0.5f, screenDimensions.Height * 0.25f));
		//_entities.Add(_spiderEnemy1);
		_spiderEnemy2 = new SpiderEnemy(new Vector2(screenDimensions.Width * 0.15f, screenDimensions.Height * 0.15f));
		//_entities.Add(_spiderEnemy2);
		_spiderEnemy3 = new SpiderEnemy(new Vector2(screenDimensions.Width * 0.8f, screenDimensions.Height * 0.7f));
		//_entities.Add(_spiderEnemy3);

		//to allow for cycling through enemies
		_enemyManager = new EnemyManager();
		_enemyManager.AddEnemy(_spiderEnemy1);
		_enemyManager.AddEnemy(_spiderEnemy2);
		_enemyManager.AddEnemy(_spiderEnemy3);

        //load Item Sprites and manager
        SpriteFactory.Instance.LoadTexture(Content, "images/8Ball.json", "images/8Ball");
        SpriteFactory.Instance.LoadTexture(Content, "images/Red_Heart.json", "images/Red_Heart");

        _itemManager = new ItemManager(_player, SpriteFactory.Instance);

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
        _player.CurrentItem?.Update(delta);
		_enemyManager.Update(delta);

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
		_enemyManager.Draw(_spriteBatch);

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
		_keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.W), new MoveUpCommand(_player));
		_keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.A), new MoveLeftCommand(_player));
		_keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.S), new MoveDownCommand(_player));
		_keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.D), new MoveRightCommand(_player));

		// Attacking controls
		_keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.LeftShift), new SecondaryAttackDownCommand(_player));
		_keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.RightShift), new SecondaryAttackDownCommand(_player));
		_keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.Up), new PrimaryAttackUpCommand(_player));
		_keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.Left), new PrimaryAttackLeftCommand(_player));
		_keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.Down), new PrimaryAttackDownCommand(_player));
		_keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.Right), new PrimaryAttackRightCommand(_player));

		//Tile Manager Controls
		_keyboardController.RegisterCommand(
			new KeyboardInput(InputState.JustPressed, KeyboardButton.Y),
			new NextTileCommand(_tileManager));

		_keyboardController.RegisterCommand(
			new KeyboardInput(InputState.JustPressed, KeyboardButton.T),
			new PreviousTileCommand(_tileManager));

		//Item Manager Controls
		_keyboardController.RegisterCommand(
			new KeyboardInput(InputState.JustPressed, KeyboardButton.I),
			new NextItemCommand(_itemManager));

		_keyboardController.RegisterCommand(
			new KeyboardInput(InputState.JustPressed, KeyboardButton.U),
			new PreviousItemCommand(_itemManager));
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.Space),
            new UseItemCommand(_player));
        
        // Temporary key for triggering player damaged and deadstate for sprint 2
        _keyboardController.RegisterCommand(
	        new KeyboardInput(InputState.Pressed, KeyboardButton.E),
	        new TriggerPlayerDamagedStateCommand(_player, 1)
	        );
        
        // Temporary key for triggering player Reset() for sprint 2
        _keyboardController.RegisterCommand(
	        new KeyboardInput(InputState.Pressed, KeyboardButton.R),
	        new ResetPlayerCommand(_player, 
		        new Vector2(screenDimensions.Width * 0.5f, screenDimensions.Height * 0.5f)
		        )
	        );

		// Temporary keys for cycling through enemies for sprint 2
		_keyboardController.RegisterCommand(
			new KeyboardInput(InputState.JustPressed, KeyboardButton.O),
			new NextEnemyCommand(_enemyManager));

		_keyboardController.RegisterCommand(
			new KeyboardInput(InputState.JustPressed, KeyboardButton.P),
			new PreviousEnemyCommand(_enemyManager));

//Mouse controls
        _mouseController.RegisterCommand(
			new MouseInput(
				new InputRegion(0, 0, screenDimensions.Width, screenDimensions.Height),
				InputState.Pressed,
				MouseButton.Right),
			new SecondaryAttackDynamicMouseCommand(_player, _mouseService)
			);
		_mouseController.RegisterCommand(
			new MouseInput(
				new InputRegion(0, 0, screenDimensions.Width, screenDimensions.Height),
				InputState.Pressed,
				MouseButton.Left),
			new PrimaryAttackDynamicMouseCommand(_player, _mouseService)
			);
		
		_keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.Escape), new ExitCommand(this));
		_keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.Q), new ExitCommand(this));
	}
}

