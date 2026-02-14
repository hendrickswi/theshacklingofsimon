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
    private IPlayer _player;
    private List<IEntity> _entities;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Rectangle screenDimensions = GraphicsDevice.Viewport.Bounds;
        
        _entities = new List<IEntity>();
        _player =
            new PlayerWithTwoSprites(new Vector2(screenDimensions.Width * 0.5f, screenDimensions.Height * 0.5f));
        _entities.Add(_player);
        _keyboardController = new KeyboardController(new MonoGameKeyboardService());
        _mouseController = new MouseController(new MonoGameMouseService());
        
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
        _keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.D), new MoveUpCommand(_player));
        
        // Attacking controls
        _keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.E), new SecondaryAttackNeutralCommand(_player));
        _keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.LeftShift), new SecondaryAttackNeutralCommand(_player));
        _keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.RightShift), new SecondaryAttackNeutralCommand(_player));
        _keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.Up), new PrimaryAttackUpCommand(_player));
        _keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.Left), new PrimaryAttackLeftCommand(_player));
        _keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.Down), new PrimaryAttackDownCommand(_player));
        _keyboardController.RegisterCommand(new KeyboardInput(BinaryInputState.Pressed, KeyboardButton.Right), new PrimaryAttackRightCommand(_player));
        
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
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _texture = Content.Load<Texture2D>("misc");
        _font = Content.Load<SpriteFont>("File");
    }

    protected override void Update(GameTime delta)
    {
        _keyboardController.Update();
        _mouseController.Update();
        
        /*
         * Add various other things that need to be updated.
         */
        
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
        foreach (IEntity e in _entities)
        {
            e.Draw(_spriteBatch);
        }
        
        _spriteBatch.End();
        base.Draw(delta);
    }
}
