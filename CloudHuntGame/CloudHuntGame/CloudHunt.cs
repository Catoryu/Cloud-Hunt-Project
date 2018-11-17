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

        //Score
        int score = 0;

        public bool killBonus = false;
        public bool zoneShot = false;
        public bool instaKill = false;

        //Game states
        enum _gameState { Loading, Menu, Options, Playing, Exit };
        Stack<_gameState> gameState = new Stack<_gameState>();

        //Entities
        Player player;
        List<Entity> entities;
        List<Entity> menuEntities;//Entities for menuAnimation
        Baloon BaloonBonus;
        Animation redBaloonAnimation;
        Animation blueBaloonAnimation;
        Animation greenBaloonAnimation;
        Animation yellowBaloonAnimation;
        Animation background;
        Animation panel;
        Animation icon;
        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;
        //Mouse states used to track Mouse button press
        MouseState currentMouseState;
        MouseState previousMouseState;

        //Other
        Cloud TypeCloud = new Cloud();
        Baloon TypeBaloon = new Baloon();
        Random rnd;
        
        bool shotGamepad = false;
        bool shotKeyboard = false;

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
                entities.Add(new Cloud());
            }
            menuEntities = new List<Entity>();
            for (int i = 0; i < 5; i++)
            {
                menuEntities.Add(new Cloud());
            }
            for (int i = 0; i < 4; i++)
            {
                menuEntities.Add(new Baloon());
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
            background = new Animation(Content.Load<Texture2D>("Graphics\\sky-background"), new Vector2(250, 250), 100, 100, 1, 30, Color.White, 5f, true);
            panel = new Animation(Content.Load<Texture2D>("Graphics\\panel"), new Vector2(600, 250), 200, 500, 1, 30, Color.White, 1f, true);
            icon = new Animation(Content.Load<Texture2D>("Graphics\\icon"), new Vector2(100, 100), 50, 50, 1, 30, Color.White, 1f, true);

            //Load the player resources
            Texture2D playerTexture = Content.Load<Texture2D>("Graphics\\cursor");
            Animation playerAnimation = new Animation(playerTexture, Vector2.Zero, 50, 50, 1, 30, Color.White, 1f, true);

            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
                GraphicsDevice.Viewport.TitleSafeArea.Y);
            player.Initialize(playerAnimation, playerPosition);

            //Load the entities resources
            Texture2D cloudTexture = Content.Load<Texture2D>("Graphics\\cloud");
            Animation cloudAnimation;
            rnd = new Random();

            foreach(Entity val in entities)
            {
                cloudAnimation = new Animation(cloudTexture, Vector2.Zero, 32, 16, 1, 30, Color.White, (float)rnd.Next(3, 7)/2, true);
                val.Initialize(cloudAnimation, new Vector2(rnd.Next(0, 500), rnd.Next(0, 500)), (float)rnd.Next(5, 11), rnd.Next(0, 2) == 1);
            }
            foreach(Entity val in menuEntities)
            {
                if (val.GetType() == TypeCloud.GetType())
                {
                    cloudAnimation = new Animation(cloudTexture, Vector2.Zero, 32, 16, 1, 30, Color.White, (float)rnd.Next(3, 7) / 2, true);
                    val.Initialize(cloudAnimation, new Vector2(rnd.Next(0, 500), rnd.Next(0, 500)), (float)rnd.Next(5, 11), rnd.Next(0, 2) == 1);
                }
            }

            //Load baloons resources
            Texture2D baloonTexture = Content.Load<Texture2D>("Graphics\\baloon");
            redBaloonAnimation = new Animation(baloonTexture, Vector2.Zero, 32, 96, 1, 30, Color.Red, 1f, true);
            blueBaloonAnimation = new Animation(baloonTexture, Vector2.Zero, 32, 96, 1, 30, Color.Blue, 1f, true);
            greenBaloonAnimation = new Animation(baloonTexture, Vector2.Zero, 32, 96, 1, 30, Color.Green, 1f, true);
            yellowBaloonAnimation = new Animation(baloonTexture, Vector2.Zero, 32, 96, 1, 30, Color.Yellow, 1f, true);

            foreach(Entity val in menuEntities)
            {
                if (val.GetType() == TypeBaloon.GetType())
                {
                    switch (rnd.Next(0, 4))
                    {
                        case 0:
                            val.Initialize(redBaloonAnimation, new Vector2(rnd.Next(0, 500), rnd.Next(0, 500)), 1f, Baloon.BaloonColor.red);
                            break;
                        case 1:
                            val.Initialize(blueBaloonAnimation, new Vector2(rnd.Next(0, 500), rnd.Next(0, 500)), 1f, Baloon.BaloonColor.blue);
                            break;
                        case 2:
                            val.Initialize(greenBaloonAnimation, new Vector2(rnd.Next(0, 500), rnd.Next(0, 500)), 1f, Baloon.BaloonColor.green);
                            break;
                        case 3:
                            val.Initialize(yellowBaloonAnimation, new Vector2(rnd.Next(0, 500), rnd.Next(0, 500)), 1f, Baloon.BaloonColor.yellow);
                            break;
                        default:
                            break;
                    }
                }
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

            background.Update(gameTime);
            panel.Update(gameTime);
            switch(gameState.Peek())
            {
                case _gameState.Loading:
                    if (frame > 0)
                    {
                        gameState.Pop();
                        gameState.Push(_gameState.Menu);
                        frame = 0;
                    }
                    break;
                case _gameState.Menu:
                    icon.Update(gameTime);
                    UpdateMenuEntities(gameTime);
                    foreach(Entity val in menuEntities)
                    {
                        val.Update(gameTime);
                    }
                    if (frame > 30)
                    {
                        if (GamePad.GetState(PlayerIndex.One).Buttons.B  == ButtonState.Pressed
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

                    //Baloons
                    if (frame / 1200 == 1 && frame % 1200 == 0)
                    {
                        BaloonBonus = new Baloon();
                        switch (rnd.Next(0, 4))
                        {
                            case 0:
                                BaloonBonus.Initialize(redBaloonAnimation, new Vector2(rnd.Next(0, 500), 500 + redBaloonAnimation.FrameHeight), 4f, Baloon.BaloonColor.red);
                                break;
                            case 1:
                                BaloonBonus.Initialize(blueBaloonAnimation, new Vector2(rnd.Next(0, 500), 500 + blueBaloonAnimation.FrameHeight), 4f, Baloon.BaloonColor.blue);
                                break;
                            case 2:
                                BaloonBonus.Initialize(greenBaloonAnimation, new Vector2(rnd.Next(0, 500), 500 + greenBaloonAnimation.FrameHeight), 4f, Baloon.BaloonColor.green);
                                break;
                            case 3:
                                BaloonBonus.Initialize(yellowBaloonAnimation, new Vector2(rnd.Next(0, 500), 500 + yellowBaloonAnimation.FrameHeight), 4f, Baloon.BaloonColor.yellow);
                                break;
                            default:
                                break;
                        }
                        entities.Add(BaloonBonus);
                    }
                    if (frame / 1200 == 2 && frame % 1200 == 0)
                    {
                        instaKill = false;
                        killBonus = false;
                        zoneShot = false;
                    }

                    UpdateShoots(gameTime);

                    //Exit
                    if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed
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
                if (value.GetType() == TypeCloud.GetType())
                {
                    if (((Cloud)value).Direction)
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
                if (value.GetType() == TypeBaloon.GetType())
                {
                    value.setPosition(value.Position.X, value.Position.Y - value.MoveSpeed);
                    if (value.Position.Y < 0 - value.Height)
                    {
                        value.Active = false;
                    }
                }
            }
        }

        /// <summary>
        /// Update menu entities movements.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void UpdateMenuEntities(GameTime gameTime)
        {
            foreach (Entity value in menuEntities)
            {
                if (value.GetType() == TypeCloud.GetType())
                {
                    if (((Cloud)value).Direction)
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
                if (value.GetType() == TypeBaloon.GetType())
                {
                    value.setPosition(value.Position.X, value.Position.Y - value.MoveSpeed);
                    if (value.Position.Y < 0 - value.Height)
                    {
                        value.setPosition(value.Position.X, 500 + value.Height);
                    }
                }
            }
        }

        public void UpdateShoots(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed && !(shotGamepad || shotKeyboard))
            {
                shotGamepad = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !(shotKeyboard || shotGamepad))
            {
                shotKeyboard = true;
            }
            if (shotKeyboard || shotGamepad)
            {
                foreach (Entity value in entities)
                {
                    if (value.Position.X - (value.Width / 2) < player.Position.X && player.Position.X < value.Position.X + (value.Width / 2)
                    && value.Position.Y - (value.Height / 2) < player.Position.Y && player.Position.Y < value.Position.Y + (value.Height / 2)
                    && ((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Released && shotGamepad)
                        || (Keyboard.GetState().IsKeyUp(Keys.Enter) && shotKeyboard)))
                    {
                        score = value.getShooted(score);
                    }
                }
                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Released)
                {
                    shotGamepad = false;
                }
                if (Keyboard.GetState().IsKeyUp(Keys.Enter))
                {
                    shotKeyboard = false;
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
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            background.Draw(spriteBatch);
            switch(gameState.Peek())
            {
                case _gameState.Loading:
                    panel.Draw(spriteBatch);
                    spriteBatch.DrawString(_font, "Loading...", Vector2.Zero, Color.DarkBlue);
                    break;
                case _gameState.Menu:
                    foreach(Entity val in menuEntities)
                    {
                        val.Draw(spriteBatch);
                    }
                    panel.Draw(spriteBatch);
                    icon.Draw(spriteBatch);
                    spriteBatch.DrawString(_font, "Cloud Hunt", new Vector2(150, 80), Color.DarkBlue, 0f, Vector2.Zero, 3f, SpriteEffects.None,0f);
                    break;
                case _gameState.Playing:
                    foreach (Entity val in entities)
                    {
                        val.Draw(spriteBatch);
                    }
                    player.Draw(spriteBatch);
                    panel.Draw(spriteBatch);
                    spriteBatch.DrawString(_font, "Time : " + frame / 60, new Vector2(535, 35), Color.DarkBlue);
                    spriteBatch.DrawString(_font, "Score: " + score, new Vector2(535, 50), Color.DarkBlue);

                    break;
                case _gameState.Options:
                    panel.Draw(spriteBatch);
                    break;
                case _gameState.Exit:
                    panel.Draw(spriteBatch);
                    spriteBatch.DrawString(_font, "Good bye", new Vector2(150, 230), Color.DarkBlue, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
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
