using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Controllers;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _texture;
    private SpriteFont _font;

    private IController<Keys> _keyboardController;
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
        _keyboardController = new KeyboardController();
        _mouseController = new MouseController();
        
        /*
         * Controls are initialized here using RegisterCommand()
         * Use a Keys enum or MouseInput struct to register an input for mouse/keyboard
         * Then use some ICommand concrete class to register *what* that input does.
         *
         * e.g., _keyboardController.RegisterCommand(Keys.D0, new ExitCommand(this));
         * or _mouseController.RegisterCommand(
         *      new MouseInput(new Rectangle(0, 0, screenDimensions.Width, screenDimensions.Height), ButtonState.Pressed, MouseButton.Right), 
         *      new ExitCommand(this));
         * to register the D0 key and right click to exit the game.
         */
        // Movement controls
        _keyboardController.RegisterCommand(Keys.W, new MoveUpCommand(player));
        _keyboardController.RegisterCommand(Keys.A, new MoveLeftCommand(player));
        _keyboardController.RegisterCommand(Keys.S, new MoveRightCommand(player));
        _keyboardController.RegisterCommand(Keys.D, new MoveDownCommand(player));
        
        // Attacking controls
        _keyboardController.RegisterCommand(Keys.E, new SecondaryAttackNeutralCommand(player));
        _keyboardController.RegisterCommand(Keys.LeftShift, new SecondaryAttackNeutralCommand(player));
        _keyboardController.RegisterCommand(Keys.RightShift, new SecondaryAttackNeutralCommand(player));
        _keyboardController.RegisterCommand(Keys.Up, new PrimaryAttackUpCommand(player));
        _keyboardController.RegisterCommand(Keys.Left, new PrimaryAttackLeftCommand(player));
        _keyboardController.RegisterCommand(Keys.Down, new PrimaryAttackDownCommand(player));
        _keyboardController.RegisterCommand(Keys.Right, new PrimaryAttackRightCommand(player));
        
        _mouseController.RegisterCommand(
            new MouseInput(
                new Rectangle(0, 0, screenDimensions.Width, screenDimensions.Height),
                ButtonState.Pressed, 
                MouseButton.Right), 
            new SecondaryAttackNeutralCommand(player));
        _mouseController.RegisterCommand(
            new MouseInput(
                new Rectangle(0, 0, screenDimensions.Width, screenDimensions.Height),
                ButtonState.Pressed,
                MouseButton.Left),
            new PrimaryAttackDynamicMouseCommand(player)
            );
        
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
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }
        
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
