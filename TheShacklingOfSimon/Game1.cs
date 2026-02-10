using System;
using System.Security.Principal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Graphics;

namespace TheShacklingOfSimon;

public class Game1 : Game
{
    private AnimatedSprite _mario;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Vector2 _marioPosition;
    private bool _marioVelocity;
    private TextureAtlas atlas;
    private int _currentAction = 1; // ranges from 1-4

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        /*
         * Controls will be initialized here using RegisterCommand()
         * Use a Keys enum or MouseInput struct to register an input for mouse/keyboard
         * Then use some ICommand variable to register *what* that input does.
         *
         * e.g., _keyboardController.RegisterCommand(Keys.D0, new ExitCommand(this));
         * or _mouseController.RegisterCommand(
         *      new MouseInput(new Rectangle(0, 0, screenDimensions.Width, screenDimensions.Height), ButtonState.Pressed, MouseButton.Right), 
         *      new ExitCommand(this));
         * to register the D0 key and right click to exit the game.
         */
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");

        _mario = atlas.CreateAnimatedSprite("mario-idle");
        _mario.Scale = new Vector2(5.0f, 5.0f);
        _marioPosition.X = Window.ClientBounds.Width * 0.45f;
        _marioPosition.Y = Window.ClientBounds.Height * 0.45f;

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        _mario.Update(gameTime);

        CheckKeyboardInput();

        base.Update(gameTime);

    }

    private void CheckKeyboardInput()
    {
        
        KeyboardState keyboardState = Keyboard.GetState();
        MouseState mouseState = Mouse.GetState();

        if (keyboardState.IsKeyDown(Keys.D0) || keyboardState.IsKeyDown(Keys.NumPad0))
        {
            Exit();
        }
        if (keyboardState.IsKeyDown(Keys.D1) || keyboardState.IsKeyDown(Keys.NumPad1) || (mouseState.LeftButton == ButtonState.Pressed &&
        (mouseState.Position.X < Window.ClientBounds.Width * 0.5f && mouseState.Position.Y < Window.ClientBounds.Height * 0.5f)))
        {
            _currentAction = 1;
        }
        if (keyboardState.IsKeyDown(Keys.D2) || keyboardState.IsKeyDown(Keys.NumPad2) || (mouseState.LeftButton == ButtonState.Pressed &&
        (mouseState.Position.X >= Window.ClientBounds.Width * 0.5f && mouseState.Position.Y < Window.ClientBounds.Height * 0.5f)))
        {
            _currentAction = 2;
        }
        if (keyboardState.IsKeyDown(Keys.D3) || keyboardState.IsKeyDown(Keys.NumPad3) || (mouseState.LeftButton == ButtonState.Pressed &&
        (mouseState.Position.X < Window.ClientBounds.Width * 0.5f && mouseState.Position.Y >= Window.ClientBounds.Height * 0.5f)))
        {
            _currentAction = 3;
        }
        if (keyboardState.IsKeyDown(Keys.D4) || keyboardState.IsKeyDown(Keys.NumPad4) || (mouseState.LeftButton == ButtonState.Pressed &&
        (mouseState.Position.X >= Window.ClientBounds.Width * 0.5f && mouseState.Position.Y >= Window.ClientBounds.Height * 0.5f)))
        {
            _currentAction = 4;
        }
        DoAction();
    }

    private void DoAction()
    {
        if (_currentAction == 1)
        {
            _mario.Animation = atlas.GetAnimation("mario-idle");
        }
        if (_currentAction == 2)
        {
            if (_mario.Animation != atlas.GetAnimation("mario-twerk"))
            _mario.Animation = atlas.GetAnimation("mario-twerk");
        }
        if (_currentAction == 3)
        {
            if (_mario.Animation != atlas.GetAnimation("mario-run"))
            _mario.Animation = atlas.GetAnimation("mario-run");
            if (_marioVelocity)
            {
                _marioPosition.X += 15;
            } else
            {
                _marioPosition.X -= 15;
            }
            if (_marioPosition.X > Window.ClientBounds.Width || _marioPosition.X < 0)
            {
                _marioVelocity = !_marioVelocity;
            }

        }
        if (_currentAction == 4)
        {
            _mario.Animation = atlas.GetAnimation("mario-jump");
            if (_marioVelocity)
            {
                _marioPosition.Y += 15;
            } else
            {
                _marioPosition.Y -= 15;
            }
            if (_marioPosition.Y > Window.ClientBounds.Height || _marioPosition.Y < 0)
            {
                _marioVelocity = !_marioVelocity;
            }
        }
    }
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        // Begin the sprite batch to prepare for rendering.

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        SpriteFont font = Content.Load<SpriteFont>("Credits");
        
        _spriteBatch.DrawString(
            font,                   // font
            "Credits\nProgram Made By: Cameron Collins\nSprites from: https://www.mariouniverse.com/sprites-nes-smb/",     // text
            new Vector2(Window.ClientBounds.Width * 0.2f, Window.ClientBounds.Height * 0.7f),           // position
            Color.White             // color
        );

        _mario.Draw(
            _spriteBatch,
            _marioPosition
        );

        // Always end the sprite batch when finished.
        _spriteBatch.End();

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
    
    public void SetSprite(int choice)
    {
        Rectangle screenDimensions = GraphicsDevice.Viewport.Bounds;
        Rectangle[] sprites = new Rectangle[]
        {
            // Rectangles for grabbing sprites off whatever spritesheets we decide to use
        };
        switch (choice)
        {
            // Add cases depending on how many sprites we have
        }
    }
}
