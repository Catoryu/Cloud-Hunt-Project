using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Hunt;
using Animations;

namespace CloudHuntGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class CloudHunt : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont _font;

        //Timer
        int frame;

        //Game states
        enum _gameState { Loading, Menu, Options, Playing, Exit };
        Stack<_gameState> gameState = new Stack<_gameState>();

        //Entities
        Player player;
        List<Entity> entities;
        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;
        //Mouse states used to track Mouse button press
        MouseState currentMouseState;
        MouseState previousMouseState;

        public CloudHunt()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            player = new Player();
            entities = new List<Entity>();
            for (int i = 0; i < 20; i++)
            {
                entities.Add(new Entity());
            }

            base.Initialize();

            graphics.PreferredBackBufferWidth = 700;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 500;   // set this value to the desired height of your window
            graphics.ApplyChanges();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Font\\Consola");

            //Load the player resources
            Texture2D playerTexture = Content.Load<Texture2D>("Graphics\\cursor");
            Animation playerAnimation = new Animation(playerTexture, Vector2.Zero, 50, 50, 1, 30, Color.White, 1f, true);

            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
                GraphicsDevice.Viewport.TitleSafeArea.Y);
            player.Initialize(playerAnimation, playerPosition);

            //Load the entities resources
            Texture2D cloudTexture = Content.Load<Texture2D>("Graphics\\cloud");
            Animation cloudAnimation;
            Random rnd = new Random();

            foreach(Entity val in entities)
            {
                cloudAnimation = new Animation(cloudTexture, Vector2.Zero, 32, 16, 1, 30, Color.White, (float)rnd.Next(3, 7)/2, true);
                val.Initialize(cloudAnimation, new Vector2(rnd.Next(0, 500), rnd.Next(0, 500)), (float)rnd.Next(5, 11), rnd.Next(0, 2) == 1);
            }

            //Initial Game State
            gameState.Push(_gameState.Loading);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Save the previous state of the keyboard and game pad so we can determine single key/button presses
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;

            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            switch(gameState.Peek())
            {
                case _gameState.Loading:
                    if (frame > 180)
                    {
                        gameState.Pop();
                        gameState.Push(_gameState.Menu);
                        frame = 0;
                    }
                    break;
                case _gameState.Menu:
                    if (frame > 30)
                    {
                        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                        || Keyboard.GetState().IsKeyDown(Keys.Escape))
                        {
                            gameState.Pop();
                            gameState.Push(_gameState.Exit);
                            frame = 0;
                        }
                        else if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed
                            || Keyboard.GetState().IsKeyDown(Keys.Enter))
                        {
                            gameState.Push(_gameState.Playing);
                            frame = 0;
                        }
                    }
                    break;
                case _gameState.Playing:
                    //Update the player
                    UpdatePlayer(gameTime);
                    player.Update(gameTime);

                    //Update entities
                    UpdateEntities(gameTime);
                    foreach (Entity val in entities)
                    {
                        val.Update(gameTime);
                    }

                    //Exit
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                        || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        gameState.Pop();
                        frame = 0;
                    }
                    break;
                case _gameState.Options:

                    break;
                case _gameState.Exit:
                    if (frame > 180)
                    {
                        gameState.Pop();
                        Exit();
                    }
                    break;
                default:
                    gameState.Push(_gameState.Loading);
                    break;
            }
            base.Update(gameTime);
            frame++;
        }

        /// <summary>
        /// Update player movements.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void UpdatePlayer(GameTime gameTime)
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            //Get Mouse State then Capture the Button type and Respond Button Press
            Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);

            /*if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                player.Position = mousePosition;
            }*/

            // Get Thumbstick Controls
            player.setPosition(player.Position.X + currentGamePadState.ThumbSticks.Left.X * player.MoveSpeed,
                player.Position.Y - currentGamePadState.ThumbSticks.Left.Y * player.MoveSpeed);

            // Use the Keyboard / Dpad
            if (currentKeyboardState.IsKeyDown(Keys.A) || currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                player.setPosition(player.Position.X - player.MoveSpeed, player.Position.Y);
            }

            if (currentKeyboardState.IsKeyDown(Keys.D) || currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                player.setPosition(player.Position.X + player.MoveSpeed, player.Position.Y);
            }

            if (currentKeyboardState.IsKeyDown(Keys.W) || currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                player.setPosition(player.Position.X, player.Position.Y - player.MoveSpeed);
            }

            if (currentKeyboardState.IsKeyDown(Keys.S) || currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                player.setPosition(player.Position.X, player.Position.Y + player.MoveSpeed);
            }

            // Make sure that the player does not go out of bounds
            player.setPosition(MathHelper.Clamp(player.Position.X, 0, 500),
                MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height));
        }

        /// <summary>
        /// Update entities movements.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void UpdateEntities(GameTime gameTime)
        {
            foreach(Entity value in entities)
            {
                if (value.Direction)
                {
                    value.setPosition(value.Position.X + value.MoveSpeed, value.Position.Y);
                    if (value.Position.X > 500 + value.Width)
                    {
                        value.setPosition(0 - value.Width, value.Position.Y);
                    }
                }
                else
                {
                    value.setPosition(value.Position.X - value.MoveSpeed, value.Position.Y);
                    if (value.Position.X < 0 - value.Width)
                    {
                        value.setPosition(500 + value.Width, value.Position.Y);
                    }
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            switch(gameState.Peek())
            {
                case _gameState.Loading:
                    spriteBatch.DrawString(_font, "Loading...", Vector2.Zero, Color.DarkBlue);
                    break;
                case _gameState.Menu:
                    spriteBatch.DrawString(_font, "Menu", Vector2.Zero, Color.DarkBlue);
                    break;
                case _gameState.Playing:
                    foreach (Entity val in entities)
                    {
                        val.Draw(spriteBatch);
                    }
                    player.Draw(spriteBatch);

                    break;
                case _gameState.Options:

                    break;
                case _gameState.Exit:
                    spriteBatch.DrawString(_font, "Exit", Vector2.Zero, Color.DarkBlue);
                    break;
                default:
                    gameState.Push(_gameState.Loading);
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
