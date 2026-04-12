#region

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Controllers;
using TheShacklingOfSimon.Controllers.Gamepad;
using TheShacklingOfSimon.Controllers.Keyboard;
using TheShacklingOfSimon.Controllers.Mouse;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.GameStates;
using TheShacklingOfSimon.GameStates.States;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Items.Active_Items;
using TheShacklingOfSimon.Level_Handling.Implementations;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomConstructor;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomManager;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles.Border.Doors;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.UI;
using TheShacklingOfSimon.Weapons;
using KeyboardInput = TheShacklingOfSimon.Controllers.Keyboard.KeyboardInput;

#endregion

namespace TheShacklingOfSimon;

public class Game1 : Game
{
    private const int ScreenWidth = 1024;
    private const int ScreenHeight = 768;

    private readonly GraphicsDeviceManager _graphics;
    private readonly List<IEntity> _persistentDynamicEntities = new();

    private SpriteBatch _spriteBatch;

    private IController<KeyboardInput> _keyboardController;
    private IController<MouseInput> _mouseController;
    private IGamepadController _gamepadController;

    private RoomManager _roomManager;
    private PickupManager _pickupManager;
    private InputManager _inputManager;
    private SoundManager _soundManager;

    private IPlayer _player;
    private ProjectileManager _projectileManager;
    private CollisionBulkLoader _collisionBulkLoader;
    private CollisionManager _collisionManager;
    private GameStateManager _gameStateManager;

    private HUD HUD;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = ScreenWidth;
        _graphics.PreferredBackBufferHeight = ScreenHeight;
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
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        _projectileManager = new ProjectileManager();
        _collisionManager = new CollisionManager();
        _soundManager = new SoundManager();
        
        LoadFonts();
        LoadSpriteAssets();
        LoadSounds();

        RoomFactory roomFactory = CreateRoomFactory();
        CreateRoomManager(roomFactory);
        CreatePlayer();
        CreatePlayerWeapons();
        CreatePlayerItems();
        CreateItemAndPickupManagers();
        CreateInputManager();
        CreateSoundManager();
        ConfigureCollisionAndProjectileHooks();
        CreateGameStates();
    }

    protected override void Update(GameTime delta)
    {
        // I update input before the active state so the current state's bindings fire first.
        _keyboardController.Update();
        _mouseController.Update();
        _gamepadController.Update();

        _gameStateManager.Update(delta);
        //HUD.Update();
        base.Update(delta);
    }

    protected override void Draw(GameTime delta)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        _gameStateManager.Draw(_spriteBatch);
        HUD.Draw(_spriteBatch);
        _spriteBatch.End();
       

        base.Draw(delta);
    }

    private void LoadFonts()
    {
        SpriteFactory.Instance.LoadFont(Content, "fonts/spritefont/Roboto", "Roboto");
        SpriteFactory.Instance.LoadFont(Content, "fonts/spritefont/Arial", "Arial");
        SpriteFactory.Instance.LoadFont(Content, "fonts/spritefont/OptimusPrinceps", "OptimusPrinceps");
        SpriteFactory.Instance.LoadFont(Content, "fonts/spritefont/Upheaval16", "Upheaval16");
        SpriteFactory.Instance.LoadFont(Content, "fonts/spritefont/Upheaval32", "Upheaval32");
    }
    
    private void LoadSpriteAssets()
    {
        SpriteFactory.Instance.LoadTexture(Content, "PlayerDefaultSprite.json", "player");
        SpriteFactory.Instance.LoadTexture(Content, "SpiderEnemy.json", "SpiderEnemy");
        SpriteFactory.Instance.LoadTexture(Content, "BlackMaw.json", "BlackMaw");
        SpriteFactory.Instance.LoadTexture(Content, "AngelicBaby.json", "AngelicBaby");
        SpriteFactory.Instance.LoadTexture(Content, "AdultLeech.json", "AdultLeech");

        SpriteFactory.Instance.LoadTexture(Content, "images/Rocks.json", "images/Rocks");
        SpriteFactory.Instance.LoadTexture(Content, "images/Spikes.json", "images/Spikes");
        SpriteFactory.Instance.LoadTexture(Content, "images/Fire.json", "images/Fire");
        SpriteFactory.Instance.LoadTexture(Content, "images/RoomBackground.json", "images/RoomBackground");

        SpriteFactory.Instance.LoadTexture(Content, "images/8Ball.json", "images/8Ball");
        SpriteFactory.Instance.LoadTexture(Content, "images/Red_Heart.json", "images/Red_Heart");

        // These are the upright door sprites. The door tile rotates them by side.
        SpriteFactory.Instance.LoadTexture(Content, "images/DoorLockedUp.json", "images/DoorLockedUp");
        SpriteFactory.Instance.LoadTexture(Content, "images/DoorUnlockedUp.json", "images/DoorUnlockedUp");
        SpriteFactory.Instance.LoadTexture(Content, "images/HeartsUI.json", "images/HeartsUI");
        SpriteFactory.Instance.LoadTexture(Content, "images/fire_mind_tears.json", "images/fire_mind_tears");
        SpriteFactory.Instance.LoadTexture(Content, "images/metallic_tears.json", "images/metallic_tears");
        SpriteFactory.Instance.LoadTexture(Content, "images/BombImg.json", "images/BombImg");
        
        // 1x1 white pixel used for background stuff
        SpriteFactory.Instance.LoadTexture(Content, "1x1white.json", "1x1white");
    }

    private void LoadSounds()
    {
        SoundFactory.Instance.LoadSFX(Content, "sounds/enemy/goodeath0");
        SoundFactory.Instance.LoadSFX(Content, "sounds/enemy/Meaty_Deaths_0");
        SoundFactory.Instance.LoadSFX(Content, "sounds/enemy/TearImpacts0");

        SoundFactory.Instance.LoadSFX(Content, "sounds/isaac/1up");
        SoundFactory.Instance.LoadSFX(Content, "sounds/isaac/Isaac_Hurt_Grunt0");
        SoundFactory.Instance.LoadSFX(Content, "sounds/isaac/isaacdies");

        SoundFactory.Instance.LoadSFX(Content, "sounds/items/itemrecharge");
        SoundFactory.Instance.LoadSFX(Content, "sounds/items/plop");
        SoundFactory.Instance.LoadSFX(Content, "sounds/items/Powerup2");
        SoundFactory.Instance.LoadSFX(Content, "sounds/items/warp");

        SoundFactory.Instance.LoadSFX(Content, "sounds/other/Coin_Slot");
        SoundFactory.Instance.LoadSFX(Content, "sounds/other/dark-souls-you-died-sound-effect_hm5sYFG");
        SoundFactory.Instance.LoadSFX(Content, "sounds/other/Fart");
        SoundFactory.Instance.LoadSFX(Content, "sounds/other/Rock_crumble 0");

        SoundFactory.Instance.LoadSFX(Content, "sounds/projectiles/splatter00");
        SoundFactory.Instance.LoadSFX(Content, "sounds/projectiles/stoneshoot2");
    }

    private RoomFactory CreateRoomFactory()
    {
        var roomFactory = new RoomFactory();

        // I assign door textures before RoomManager is created because the starting room
        // gets loaded inside the RoomManager constructor.
        roomFactory.DoorTextures = new DoorTextureSet(
            Content.Load<Texture2D>("images/DoorLockedUp"),
            Content.Load<Texture2D>("images/DoorUnlockedUp")
        );

        roomFactory.OnProjectileCreated += _collisionManager.AddDynamicEntity;
        roomFactory.OnProjectileCreated += _projectileManager.AddProjectile;

        return roomFactory;
    }

    private void CreateRoomManager(RoomFactory roomFactory)
    {
        var roomReader = new JsonRoomReader(Content);
        var indexReader = new RoomIndexReader(Content);

        _roomManager = new RoomManager(
            roomReader,
            indexReader,
            roomFactory,
            GraphicsDevice,
            preserveRoomState: true);
    }

    private void CreatePlayer()
    {
        _player = new PlayerWithTwoSprites(GetScreenCenter());
        HUD = new HUD(_player, _roomManager, GraphicsDevice);
    }

    private void CreatePlayerWeapons()
    {
        IPrimaryWeapon playerBasicWeapon = new BasicWeapon(
            new BasicProjectile(
                Vector2.Zero,
                new Vector2(0, 1),
                SpriteFactory.Instance.CreateStaticSprite("BasicProjectile"),
                new ProjectileStats(1, 200f, ProjectileOwner.Player)));

        ISecondaryWeapon playerBombWeapon = new BombWeapon(
            new BombProjectile(
                Vector2.Zero,
                SpriteFactory.Instance.CreateAnimatedSprite("PlayerHeadShootingDown", 0.1f),
                new ProjectileStats(1, 0.0f, ProjectileOwner.Player)));

        IPrimaryWeapon playerFireballWeapon = new FireballWeapon(
            new FireballProjectile(
                Vector2.Zero,
                new Vector2(0, 1),
                SpriteFactory.Instance.CreateStaticSprite("BasicProjectile"),
                new ProjectileStats(2, 100.0f, ProjectileOwner.Player)));
        _player.Inventory.Add(playerBasicWeapon);
        _player.Inventory.CurrentPrimaryWeapon = playerBasicWeapon;

        _player.Inventory.Add(playerBombWeapon);
        _player.Inventory.CurrentSecondaryWeapon = playerBombWeapon;

        _player.Inventory.Add(playerFireballWeapon);

        playerBasicWeapon.OnProjectileFired += _collisionManager.AddDynamicEntity;
        playerBombWeapon.OnProjectileFired += _collisionManager.AddDynamicEntity;

        playerBasicWeapon.OnProjectileFired += _projectileManager.AddProjectile;
        playerBombWeapon.OnProjectileFired += _projectileManager.AddProjectile;

        playerFireballWeapon.OnProjectileFired += _collisionManager.AddDynamicEntity;
        playerFireballWeapon.OnProjectileFired += _projectileManager.AddProjectile;
    }

    private void CreatePlayerItems()
    {
        _player.Inventory.Add(new TeleportItem(_player, pos => true));
        _player.Inventory.Add(new AdrenalineItem(_player));
    }

    private void CreateItemAndPickupManagers()
    {
        _pickupManager = new PickupManager();
    }

    private void CreateInputManager()
    {
        _inputManager = new InputManager(
            _keyboardController,
            _mouseController,
            _gamepadController,
            _player,
            this,
            _roomManager,
            _pickupManager,
            Reset);
    }

    private void CreateSoundManager()
    {
        List<SoundEffect> sounds = SoundFactory.Instance.GetAllSFX();
        foreach(SoundEffect x in sounds)
        {
            _soundManager.AddSFX(x);
        }
    }

    private void ConfigureCollisionAndProjectileHooks()
    {
        _persistentDynamicEntities.Clear();
        _persistentDynamicEntities.Add(_player);

        _collisionBulkLoader = new CollisionBulkLoader(_collisionManager, _persistentDynamicEntities);
        _roomManager.RoomChanged += _collisionBulkLoader.RegisterRoomCollidables;
        _collisionBulkLoader.RegisterRoomCollidables(_roomManager.CurrentRoom);

        _pickupManager.OnPickupAdded += _collisionManager.AddStaticEntity;
    }

    private void CreateGameStates()
    {
        _gameStateManager = new GameStateManager();
        _gameStateManager.AddState(
            new PlayGameState(
                _gameStateManager,
                _inputManager,
                GraphicsDevice,
                this,
                _roomManager,
                new StandardLevelObjectiveManager(
                    _gameStateManager, 
                    _inputManager, 
                    GraphicsDevice, 
                    _player, 
                    _roomManager, 
                    Reset, 
                    Exit), // can create functionality to start in different game modes
                _pickupManager,
                _soundManager,
                _player,
                _projectileManager,
                _collisionManager,
                Reset
                )
            );
    }

    private Vector2 GetScreenCenter()
    {
        Rectangle screenDimensions = GraphicsDevice.Viewport.Bounds;
        return new Vector2(screenDimensions.Width * 0.5f, screenDimensions.Height * 0.5f);
    }

    private void Reset()
    {
        _player.Reset(GetScreenCenter());
        _projectileManager.ClearAllProjectiles();
        _roomManager.ResetToGameStart();
        HUD.Reset();
        _collisionBulkLoader.RegisterRoomCollidables(_roomManager.CurrentRoom);
        CreatePlayerWeapons();
        CreatePlayerItems();
    }
}