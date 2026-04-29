#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Entities.Projectiles.Augmentation;
using TheShacklingOfSimon.Entities.Projectiles.Decorators;
using TheShacklingOfSimon.Entities.Projectiles.Implementations;
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
using TheShacklingOfSimon.StatusEffects.Implementations.Recurring;
using TheShacklingOfSimon.StatusEffects.Templates;
using TheShacklingOfSimon.UI;
using TheShacklingOfSimon.Weapons;

#endregion

namespace TheShacklingOfSimon;

public class Game1 : Game
{
    private const int ScreenWidth = 1024;
    private const int ScreenHeight = 768;

    private readonly GraphicsDeviceManager _graphics;
    private readonly List<IEntity> _persistentDynamicEntities = new();
    private readonly HashSet<string> _initializedSpecialRooms = new();

    private SpriteBatch _spriteBatch;

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
        _inputManager = new InputManager(GraphicsDevice);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _projectileManager = new ProjectileManager();
        _collisionManager = new CollisionManager();
        _soundManager = SoundManager.Instance;
        _pickupManager = new PickupManager();

        LoadFonts();
        LoadSpriteAssets();
        LoadSounds();
        LoadMusic();
        PlayMusic("basement");

        RoomFactory roomFactory = CreateRoomFactory();

        CreatePlayer();
        roomFactory.PlayerProvider = () => _player;

        CreateRoomManager(roomFactory);

        // _roomManager holds a reference to roomFactory, so these subscriptions are safe
        roomFactory.OnItemDropped += _pickupManager.AddPickup;
        roomFactory.OnEnemySpawned += _collisionManager.AddDynamicEntity;

        CreatePlayerWeapons();
        CreatePlayerItems();
        ConfigureCollisionAndProjectileHooks();
        CreateGameStates();
    }

    protected override void Update(GameTime delta)
    {
        _gameStateManager.Update(delta);
        _inputManager.Update();
        base.Update(delta);
    }

    protected override void Draw(GameTime delta)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        _gameStateManager.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(delta);
    }

    private void LoadFonts()
    {
        SpriteFactory.Instance.LoadFont(Content, "fonts/spritefont/Roboto", "Roboto");
        SpriteFactory.Instance.LoadFont(Content, "fonts/spritefont/Arial", "Arial");
        SpriteFactory.Instance.LoadFont(Content, "fonts/spritefont/OptimusPrinceps16", "OptimusPrinceps16");
        SpriteFactory.Instance.LoadFont(Content, "fonts/spritefont/OptimusPrinceps28", "OptimusPrinceps28");
        SpriteFactory.Instance.LoadFont(Content, "fonts/spritefont/Upheaval16", "Upheaval16");
        SpriteFactory.Instance.LoadFont(Content, "fonts/spritefont/Upheaval32", "Upheaval32");
    }
    
    private void LoadSpriteAssets()
    {
        SpriteFactory.Instance.LoadTexture(Content, "PlayerDefaultSprite.json", "player-full");
        SpriteFactory.Instance.LoadTexture(Content, "SpiderEnemy.json", "SpiderEnemy");
        SpriteFactory.Instance.LoadTexture(Content, "BlackMaw.json", "BlackMaw");
        SpriteFactory.Instance.LoadTexture(Content, "AngelicBaby.json", "AngelicBaby");
        SpriteFactory.Instance.LoadTexture(Content, "AdultLeech.json", "AdultLeech");
        SpriteFactory.Instance.LoadTexture(Content, "Cohort.json", "Cohort");
        SpriteFactory.Instance.LoadTexture(Content, "Clotty.json", "Clotty");
        SpriteFactory.Instance.LoadTexture(Content, "BlindCreep.json", "BlindCreep");

        SpriteFactory.Instance.LoadTexture(Content, "images/Rocks.json", "images/Rocks");
        SpriteFactory.Instance.LoadTexture(Content, "images/Spikes.json", "images/Spikes");
        SpriteFactory.Instance.LoadTexture(Content, "images/Fire.json", "images/Fire");
        SpriteFactory.Instance.LoadTexture(Content, "images/RoomBackground.json", "images/RoomBackground");

        SpriteFactory.Instance.LoadTexture(Content, "images/8Ball.json", "images/8Ball");
        SpriteFactory.Instance.LoadTexture(Content, "images/Red_Heart.json", "images/Red_Heart");
        SpriteFactory.Instance.LoadTexture(Content, "images/chestplate.json", "images/chestplate");
        SpriteFactory.Instance.LoadTexture(Content, "images/fireball.json", "images/fireball");
        SpriteFactory.Instance.LoadTexture(Content, "images/feather.json", "images/feather");
        SpriteFactory.Instance.LoadTexture(Content, "images/Coin.json", "images/Coin");
        SpriteFactory.Instance.LoadTexture(Content, "images/key.json", "images/key");

        // These are the upright door sprites. The door tile rotates them by side.
        SpriteFactory.Instance.LoadTexture(Content, "images/DoorLockedUp.json", "images/DoorLockedUp");
        SpriteFactory.Instance.LoadTexture(Content, "images/DoorUnlockedUp.json", "images/DoorUnlockedUp");
        SpriteFactory.Instance.LoadTexture(Content, "images/HeartsUI.json", "images/HeartsUI");
        SpriteFactory.Instance.LoadTexture(Content, "images/fire_mind_tears.json", "images/fire_mind_tears");
        SpriteFactory.Instance.LoadTexture(Content, "images/metallic_tears.json", "images/metallic_tears");
        SpriteFactory.Instance.LoadTexture(Content, "images/BombImg.json", "images/BombImg");
        SpriteFactory.Instance.LoadTexture(Content, "images/WinGamble.json", "images/WinGamble");
        SpriteFactory.Instance.LoadTexture(Content, "images/LoseGamble.json", "images/LoseGamble");
        SpriteFactory.Instance.LoadTexture(Content, "images/fogofwar.json", "images/fogofwar");
        SpriteFactory.Instance.LoadTexture(Content, "images/AdrenalinIndicator.json", "images/AdrenalinIndicator");
        SpriteFactory.Instance.LoadTexture(Content, "images/InvincibilityIndicator.json", "images/InvincibilityIndicator");
        SpriteFactory.Instance.LoadTexture(Content, "images/TeleportIndicator.json", "images/TeleportIndicator");
        
        // 1x1 white pixel used for background stuff
        SpriteFactory.Instance.LoadTexture(Content, "1x1white.json", "1x1white");
    }

    private void LoadSounds()
    {
        SoundFactory.Instance.LoadSFX(Content, "sounds/enemy/goodeath0");
        SoundFactory.Instance.LoadSFX(Content, "sounds/enemy/deathburst");
        SoundFactory.Instance.LoadSFX(Content, "sounds/enemy/TearImpacts0");

        SoundFactory.Instance.LoadSFX(Content, "sounds/isaac/1up");
        SoundFactory.Instance.LoadSFX(Content, "sounds/isaac/Isaac_Hurt_Grunt0");
        SoundFactory.Instance.LoadSFX(Content, "sounds/isaac/isaacdies");

        SoundFactory.Instance.LoadSFX(Content, "sounds/items/itemrecharge");
        SoundFactory.Instance.LoadSFX(Content, "sounds/items/plop");
        SoundFactory.Instance.LoadSFX(Content, "sounds/items/rocketexplode04");
        SoundFactory.Instance.LoadSFX(Content, "sounds/items/Powerup2");
        SoundFactory.Instance.LoadSFX(Content, "sounds/items/warp");
        SoundFactory.Instance.LoadSFX(Content, "sounds/items/boost");
        SoundFactory.Instance.LoadSFX(Content, "sounds/items/clang");
        SoundFactory.Instance.LoadSFX(Content, "sounds/items/equip_armor");

        SoundFactory.Instance.LoadSFX(Content, "sounds/other/Coin_Slot");
        SoundFactory.Instance.LoadSFX(Content, "sounds/other/dark-souls-you-died-sound-effect_hm5sYFG");
        SoundFactory.Instance.LoadSFX(Content, "sounds/other/Fart");
        SoundFactory.Instance.LoadSFX(Content, "sounds/other/doorClose");
        SoundFactory.Instance.LoadSFX(Content, "sounds/other/doorOpen");
        SoundFactory.Instance.LoadSFX(Content, "sounds/other/Rock_crumble 0");
        SoundFactory.Instance.LoadSFX(Content, "sounds/other/unlock");

        SoundFactory.Instance.LoadSFX(Content, "sounds/projectiles/splatter00");
        SoundFactory.Instance.LoadSFX(Content, "sounds/projectiles/stoneshoot2");

        SoundFactory.Instance.LoadSFX(Content, "sounds/items/coinpickup");
        SoundFactory.Instance.LoadSFX(Content, "sounds/items/keypickup");
    }

    private void LoadMusic()
    {
        SoundFactory.Instance.LoadSong(Content, "sounds/music/basement");
        SoundFactory.Instance.LoadSong(Content, "sounds/music/ffVictory");
    }

    private void PlayMusic(string songName)
    {
        string songStr = "sounds/music/"+songName;
        Song song = SoundFactory.Instance.GetSong(songStr);
        MediaPlayer.Play(song);
        MediaPlayer.IsRepeating = true;
        
        // Allow game states to control if the music is playing or not
        MediaPlayer.Pause(); 
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
    }

    private void CreatePlayerWeapons()
    {
        IPrimaryWeapon playerBasicWeapon = new BasicWeapon(
            new BasicProjectile(
                Vector2.Zero,
                new Vector2(0, 1),
                SpriteFactory.Instance.CreateStaticSprite("BasicProjectile"),
                new ProjectileStats(1, 200f, ProjectileOwner.Player)));
        
        // projectile augmentation test
        StatusEffectAugment poisonAugment = new StatusEffectAugment(
            new DeltaHealthEffect(
                "Poison", EffectType.Poison, _player, -1f, 3f, 3f
            )
        );
        playerBasicWeapon.AddAugment(poisonAugment);

        ISecondaryWeapon playerBombWeapon = new BombWeapon(
            new BombProjectile(
                Vector2.Zero,
                SpriteFactory.Instance.CreateAnimatedSprite("PlayerHeadShootingDown", 0.1f),
                new ProjectileStats(1, 0.0f, ProjectileOwner.Player)));

        // Fireball should arbitrarily always apply "On Fire!" status effect,
        // so the weapon is instantiated with a decorator
        IProjectile fireballProjectile = new StatusEffectProjectileDecorator(
            new FireballProjectile(
                Vector2.Zero,
                new Vector2(0, 1),
                SpriteFactory.Instance.CreateStaticSprite("BasicProjectile"),
                new ProjectileStats(2, 100.0f, ProjectileOwner.Player)
            ),
            new DeltaHealthEffect(
                "On Fire!",
                EffectType.OnFire,
                _player, // Dummy owner which is changed when status effect cloning happens
                -1f,
                3f,
                2f
            )
        );
        IPrimaryWeapon playerGamblingWeapon = new GamblingWeapon(
            new GamblingProjectile(
                Vector2.Zero,
                new Vector2(0, 1),
               null,
                new ProjectileStats(1, 50f, ProjectileOwner.Player)));

        

        IPrimaryWeapon playerFireballWeapon = new FireballWeapon(fireballProjectile);
            
        _player.Inventory.Add(playerBasicWeapon);
  
        _player.Inventory.Add(playerGamblingWeapon);
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

        playerGamblingWeapon.OnProjectileFired += _collisionManager.AddDynamicEntity;
        playerGamblingWeapon.OnProjectileFired += _projectileManager.AddProjectile;
    }

    private void CreatePlayerItems()
    {
        TeleportItem teleportItem = new TeleportItem(
            _player,
            () => _roomManager?.CurrentRoom?.TileMap
        );

        _player.Inventory.Add(teleportItem);
        _player.Inventory.Add(new AdrenalineItem(_player));
        _player.Inventory.Add(new InvincibilityItem(_player));
        
        
        _player.Inventory.CurrentActiveItem = teleportItem;
    }

    private void ConfigureCollisionAndProjectileHooks()
    {
        _persistentDynamicEntities.Clear();
        _persistentDynamicEntities.Add(_player);

        _collisionBulkLoader = new CollisionBulkLoader(_collisionManager, _persistentDynamicEntities);
        _roomManager.RoomChanged += _collisionBulkLoader.RegisterRoomCollidables;
        _collisionBulkLoader.RegisterRoomCollidables(_roomManager.CurrentRoom);

        _pickupManager.OnPickupAdded += _collisionManager.AddStaticEntity;
        
        _pickupManager.OnPickupAdded += pickup => _roomManager.CurrentRoom?.AddEntity(pickup);
        _projectileManager.OnProjectileAdded += proj => _roomManager.CurrentRoom?.AddEntity(proj);
    }

    private void CreateGameStates()
    {
        string fogEffectPath = System.IO.Path.Combine(
            AppContext.BaseDirectory,
            "Content",
            "Effects",
            "FogOfWarEffect.xnb"
        );

        // System.Diagnostics.Debug.WriteLine("Fog effect expected at: " + fogEffectPath);
        // System.Diagnostics.Debug.WriteLine("Fog effect exists: " + System.IO.File.Exists(fogEffectPath));

        Effect fogEffect = Content.Load<Effect>("Effects/FogOfWarEffect");
        HUD = new HUD(_player, _roomManager, GraphicsDevice, fogEffect);

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
                    Exit),
                _pickupManager,
                _soundManager,
                _player,
                _projectileManager,
                _collisionManager,
                HUD,
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
        _projectileManager.Clear();
        _pickupManager.Clear();
        _roomManager.ResetToGameStart();
        HUD.Reset();
        _collisionBulkLoader.RegisterRoomCollidables(_roomManager.CurrentRoom);
        CreatePlayerWeapons();
        CreatePlayerItems();
        MediaPlayer.Stop();
        SoundManager.Instance.StopAllSFX();
        PlayMusic("basement");
    }
}