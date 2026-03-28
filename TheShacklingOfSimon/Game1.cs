using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TheShacklingOfSimon.Commands.Item_Commands_and_Temporary_Manager;
using TheShacklingOfSimon.Controllers;
using TheShacklingOfSimon.Controllers.Gamepad;
using TheShacklingOfSimon.Controllers.Keyboard;
using TheShacklingOfSimon.Controllers.Mouse;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Items.Active_Items;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomConstructor;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomManager;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Weapons;
using KeyboardInput = TheShacklingOfSimon.Controllers.Keyboard.KeyboardInput;

namespace TheShacklingOfSimon;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    
    private IController<KeyboardInput> _keyboardController;
    private IController<MouseInput> _mouseController;
    private IGamepadController _gamepadController;

    private RoomManager _roomManager; //room manager for sprint 3
    private ItemManager _itemManager; //Temporary item switching for sprint 2
    private PickupManager _pickupManager;
    private InputManager _inputManager;

    private IPlayer _player;
    private ProjectileManager _projectileManager; //_projectileManager = new ProjectileManager();
    private CollisionBulkLoader _collisionBulkLoader;
    private CollisionManager _collisionManager;

    // Entities that should always be registered as dynamic colliders regardless of room
    private readonly List<IEntity> _persistentDynamicEntities = new();

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        _graphics.PreferredBackBufferWidth = 1024;
        _graphics.PreferredBackBufferHeight = 768;
    }

    protected override void Initialize()
    {
        _keyboardController = new KeyboardController(new MonoGameKeyboardService());
        _mouseController = new MouseController(new MonoGameMouseService());
        _gamepadController = new GamepadController(new MonoGameGamepadService(PlayerIndex.One));
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
        SpriteFactory.Instance.LoadTexture(Content, "BlackMaw.json", "BlackMaw");

        // Load Projectile Sprites and Manager_projectileManager.Update(delta);
        _projectileManager = new ProjectileManager();

        //load Tile Sprites and room Manager
        SpriteFactory.Instance.LoadTexture(Content, "images/Rocks.json", "images/Rocks");
        SpriteFactory.Instance.LoadTexture(Content, "images/Spikes.json", "images/Spikes");
        SpriteFactory.Instance.LoadTexture(Content, "images/Fire.json", "images/Fire");
        SpriteFactory.Instance.LoadTexture(Content, "images/RoomBackground.json", "images/RoomBackground");

        var roomReader = new JsonRoomReader(Content);
        var indexReader = new RoomIndexReader(Content);
        var roomFactory = new RoomFactory();
        
        // TEMPORARY until weapons for enemies can be directly loaded from JSON
        _collisionManager = new CollisionManager();
        roomFactory.OnProjectileCreated += _collisionManager.AddDynamicEntity;
        roomFactory.OnProjectileCreated += _projectileManager.AddProjectile;
        
        _roomManager = new RoomManager(roomReader, indexReader, roomFactory, GraphicsDevice, preserveRoomState: true);

        // Create entities now that the sprite factory has textures
        _player = new PlayerWithTwoSprites(new Vector2(screenDimensions.Width * 0.5f, screenDimensions.Height * 0.5f));

        IWeapon playerBasicWeapon = new BasicWeapon(
            new BasicProjectile(
                new Vector2(0, 0), 
                new Vector2(0, 1), 
                SpriteFactory.Instance.CreateStaticSprite("BasicProjectile"), 
                new ProjectileStats(1, 200.0f, ProjectileOwner.Player)
                )
            );
        IWeapon playerBombWeapon = new BombWeapon(
            new BombProjectile(
                new Vector2(0, 0),
                SpriteFactory.Instance.CreateAnimatedSprite("PlayerHeadShootingDown", 0.1f),
                new ProjectileStats(1, 0.0f, ProjectileOwner.Player)
                )
            );

        _player.AddWeaponToInventory(playerBasicWeapon);
        _player.EquipPrimaryWeapon(0);

        _player.AddWeaponToInventory(playerBombWeapon);
        _player.EquipSecondaryWeapon(1);

        // temporary items for demo
        _player.AddItemToInventory(new TeleportItem(_player, pos => true));
        _player.AddItemToInventory(new AdrenalineItem(_player));
        
        //load Item Sprites and manager
        SpriteFactory.Instance.LoadTexture(Content, "images/8Ball.json", "images/8Ball");
        SpriteFactory.Instance.LoadTexture(Content, "images/Red_Heart.json", "images/Red_Heart");

        _itemManager = new ItemManager(_player, SpriteFactory.Instance);
        _pickupManager = new PickupManager();

        // Register controls now that the player exists
        _inputManager = new InputManager(
            _keyboardController,
            _mouseController,
            _gamepadController,
            _player, 
            this, 
            _roomManager, 
            _itemManager,
            _pickupManager,
            Reset
        );
        _inputManager.LoadDefaultControls();

        // Persistent dynamic colliders (demo setup)
        _persistentDynamicEntities.Clear();
        _persistentDynamicEntities.Add(_player);

        /*
         * Subscribe the collision manager to the event of a new projectile
         * being created from basicWeapon and bombWeapon
         */
        playerBasicWeapon.OnProjectileFired += _collisionManager.AddDynamicEntity;
        playerBombWeapon.OnProjectileFired += _collisionManager.AddDynamicEntity;

        // Re-register collidables whenever the room changes
        _collisionBulkLoader = new CollisionBulkLoader(_collisionManager, _persistentDynamicEntities);
        _roomManager.RoomChanged += _collisionBulkLoader.RegisterRoomCollidables;

        // Register current room (starting room) once
        _collisionBulkLoader.RegisterRoomCollidables(_roomManager.CurrentRoom);
        
        playerBasicWeapon.OnProjectileFired += _projectileManager.AddProjectile;
        playerBombWeapon.OnProjectileFired += _projectileManager.AddProjectile;

        _pickupManager.OnPickupAdded += _collisionManager.AddStaticEntity;
    }

    protected override void Update(GameTime delta)
    {
        _keyboardController.Update();
        _mouseController.Update();

        _projectileManager.Update(delta);
        _roomManager.Update(delta);
        _itemManager.Update(delta);
        _pickupManager.Update(delta);
        _player.CurrentItem?.Update(delta);
        
        _player.Update(delta);
        
		_collisionManager.Update(delta);
		_roomManager.ResolvePendingRoomSwitch();

		base.Update(delta);
    }

    protected override void Draw(GameTime delta)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        GraphicsDevice.Clear(Color.Black);

        _roomManager.Draw(_spriteBatch);
        _itemManager.Draw(_spriteBatch);
        _pickupManager.Draw(_spriteBatch);
        _projectileManager.Draw(_spriteBatch);
        
        _player.Draw(_spriteBatch);

        _spriteBatch.End();
        base.Draw(delta);
    }

    // Master reset method
    private void Reset()
    {
        Rectangle screenDimensions = GraphicsDevice.Viewport.Bounds;

		_player.Reset(new Vector2(screenDimensions.Width * 0.5f, screenDimensions.Height * 0.5f));
        _projectileManager.ClearAllProjectiles();
		_roomManager.ResetToGameStart();

		_collisionBulkLoader.RegisterRoomCollidables(_roomManager.CurrentRoom);
    }
}