using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Hunt;
using Animations;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;

namespace CloudHuntAndroid
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class CloudHunt : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont _font;

        float wantedWidth = 500;
        float wantedHeight = 700;

        //Timer
        int frame;
        public float timeLimit = 30;

        //Score
        int score = 0;
        int lastScore = 0;
        int bestScore = 0;

        public bool killBonus = false;
        public bool zoneShot = false;
        public bool instaKill = false;

        //Game states
        enum _gameState { Loading, Menu, Options, Playing, Exit };
        Stack<_gameState> gameState = new Stack<_gameState>();

        //Entities
        Player player;
        List<Entity> entities;
        Texture2D cloudTexture;
        Animation cloudAnimation;
        List<Entity> menuEntities;//Entities for menuAnimation
        Baloon BaloonBonus;
        Animation redBaloonAnimation;
        Animation blueBaloonAnimation;
        Animation greenBaloonAnimation;
        Animation yellowBaloonAnimation;
        Animation background;
        Texture2D Screen;
        Animation scoreScreen;
        Animation bonusScreen;
        Texture2D panel;
        Vector2 panelPosition;
        Animation icon;

        //Other
        Cloud TypeCloud = new Cloud();
        Baloon TypeBaloon = new Baloon();
        Random rnd;
        float virtualScale;

        bool shot = false;

        public CloudHunt()
        {
            graphics = new GraphicsDeviceManager(this);
            var metric = new Android.Util.DisplayMetrics();
            Activity.WindowManager.DefaultDisplay.GetMetrics(metric);

            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = metric.WidthPixels;
            graphics.PreferredBackBufferHeight = (int)wantedHeight;
            graphics.SupportedOrientations = DisplayOrientation.PortraitDown;
            graphics.ApplyChanges();
           
            //Virtual scale
            virtualScale = graphics.PreferredBackBufferWidth / wantedWidth;
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
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            var store = IsolatedStorageFile.GetUserStoreForApplication();
            string scoreFile = "score.txt";
            if (store.FileExists(scoreFile))
            {
                var fs = store.OpenFile(scoreFile, FileMode.Open);
                using (StreamReader sr = new StreamReader(fs))
                {
                    bestScore = Convert.ToInt32(sr.ReadLine());
                }
            }
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Consola");
            background = new Animation(Content.Load<Texture2D>("Graphics\\sky-background"), new Vector2(250 * virtualScale, 250 * virtualScale), 100, 100, 1, 30, Color.White, 5f * virtualScale, true);
            panel = Content.Load<Texture2D>("Graphics\\panel");
            Screen = Content.Load<Texture2D>("Graphics\\write-screen");
            scoreScreen = new Animation(Screen, new Vector2(121 * virtualScale, 573 * virtualScale), 64, 32, 1, 30, Color.White, 3f * virtualScale, true);
            bonusScreen = new Animation(Screen, new Vector2(185 * virtualScale, 605 * virtualScale), 64, 32, 1, 30, Color.White, 5f * virtualScale, true);
            panelPosition = new Vector2(0, 500 * virtualScale);
            icon = new Animation(Content.Load<Texture2D>("Graphics\\icon"), new Vector2(100 * virtualScale, 100 * virtualScale), 50, 50, 1, 30, Color.White, virtualScale, true);

            //Load the player resources
            Texture2D playerTexture = Content.Load<Texture2D>("Graphics\\cursor");
            Animation playerAnimation = new Animation(playerTexture, Vector2.Zero, 50, 50, 1, 30, Color.White, virtualScale, true);

            Vector2 playerPosition = new Vector2(0, 0);
            player.Initialize(playerAnimation, playerPosition);

            //Load the entities resources
            cloudTexture = Content.Load<Texture2D>("Graphics\\cloud");
            
            rnd = new Random();

            foreach (Entity val in entities)
            {
                cloudAnimation = new Animation(cloudTexture, Vector2.Zero, 32, 16, 1, 30, Color.White, ((float)rnd.Next(3, 7) / 2) * virtualScale, true);
                val.Initialize(cloudAnimation, new Vector2(rnd.Next(0, (int)(500 * virtualScale)), rnd.Next(0, (int)(500 * virtualScale))), (float)rnd.Next(5, 11), rnd.Next(0, 2) == 1);
            }
            foreach (Entity val in menuEntities)
            {
                if (val.GetType() == TypeCloud.GetType())
                {
                    cloudAnimation = new Animation(cloudTexture, Vector2.Zero, 32, 16, 1, 30, Color.White, ((float)rnd.Next(3, 7) / 2) * virtualScale, true);
                    val.Initialize(cloudAnimation, new Vector2(rnd.Next(0, (int)(500 * virtualScale)), rnd.Next(0, (int)(500 * virtualScale))), (float)rnd.Next(5, 11), rnd.Next(0, 2) == 1);
                }
            }

            //Load baloons resources
            Texture2D baloonTexture = Content.Load<Texture2D>("Graphics\\baloon");
            redBaloonAnimation = new Animation(baloonTexture, Vector2.Zero, 32, 96, 1, 30, Color.Red, virtualScale, true);
            blueBaloonAnimation = new Animation(baloonTexture, Vector2.Zero, 32, 96, 1, 30, Color.Blue, virtualScale, true);
            greenBaloonAnimation = new Animation(baloonTexture, Vector2.Zero, 32, 96, 1, 30, Color.Green, virtualScale, true);
            yellowBaloonAnimation = new Animation(baloonTexture, Vector2.Zero, 32, 96, 1, 30, Color.Yellow, virtualScale, true);

            foreach (Entity val in menuEntities)
            {
                if (val.GetType() == TypeBaloon.GetType())
                {
                    switch (rnd.Next(0, 4))
                    {
                        case 0:
                            val.Initialize(redBaloonAnimation, new Vector2(rnd.Next(0, (int)(500 * virtualScale)), rnd.Next(0, (int)(500 * virtualScale))), 1f, Baloon.BaloonColor.red);
                            break;
                        case 1:
                            val.Initialize(blueBaloonAnimation, new Vector2(rnd.Next(0, (int)(500 * virtualScale)), rnd.Next(0, (int)(500 * virtualScale))), 1f, Baloon.BaloonColor.blue);
                            break;
                        case 2:
                            val.Initialize(greenBaloonAnimation, new Vector2(rnd.Next(0, (int)(500 * virtualScale)), rnd.Next(0, (int)(500 * virtualScale))), 1f, Baloon.BaloonColor.green);
                            break;
                        case 3:
                            val.Initialize(yellowBaloonAnimation, new Vector2(rnd.Next(0, (int)(500 * virtualScale)), rnd.Next(0, (int)(500 * virtualScale))), 1f, Baloon.BaloonColor.yellow);
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
            //virtualScale = GraphicsDevice.PresentationParameters.BackBufferWidth / graphics.PreferredBackBufferWidth;
            TouchCollection touchCollection = TouchPanel.GetState();
            background.Update(gameTime);
            switch (gameState.Peek())
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
                    scoreScreen.Update(gameTime);
                    foreach (Entity val in menuEntities)
                    {
                        val.Update(gameTime);
                    }
                    if (frame > 30)
                    {
                        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                        {
                            gameState.Pop();
                            gameState.Push(_gameState.Exit);
                            frame = 0;
                        }
                        else if (touchCollection.Count > 0) 
                        {
                            if (touchCollection[0].State == TouchLocationState.Moved)
                            {
                                gameState.Push(_gameState.Playing);
                                timeLimit = 30;
                                frame = 0;
                            }
                        }
                    }
                    break;
                case _gameState.Playing:
                    bonusScreen.Update(gameTime);

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
                    if (frame / (60 * (int)timeLimit) == 0 && frame % 600 == 0)
                    {
                        BaloonBonus = new Baloon();
                        switch (rnd.Next(0, 4))
                        {
                            case 0:
                                BaloonBonus.Initialize(redBaloonAnimation, new Vector2(rnd.Next(0, (int)(500 * virtualScale)), (500 + redBaloonAnimation.FrameHeight) * virtualScale), 4f, Baloon.BaloonColor.red);
                                break;
                            case 1:
                                BaloonBonus.Initialize(blueBaloonAnimation, new Vector2(rnd.Next(0, (int)(500 * virtualScale)), (500 + blueBaloonAnimation.FrameHeight) * virtualScale), 4f, Baloon.BaloonColor.blue);
                                break;
                            case 2:
                                BaloonBonus.Initialize(greenBaloonAnimation, new Vector2(rnd.Next(0, (int)(500 * virtualScale)), (500 + greenBaloonAnimation.FrameHeight) * virtualScale), 4f, Baloon.BaloonColor.green);
                                break;
                            case 3:
                                BaloonBonus.Initialize(yellowBaloonAnimation, new Vector2(rnd.Next(0, (int)(500 * virtualScale)), (500 + yellowBaloonAnimation.FrameHeight) * virtualScale), 4f, Baloon.BaloonColor.yellow);
                                break;
                            default:
                                break;
                        }
                        entities.Add(BaloonBonus);
                    }

                    if (instaKill)
                    {
                        foreach (Entity value in entities)
                        {
                            score = value.getShooted(score, this);
                        }
                        instaKill = false;
                    }

                    if (frame / (60 * (int)timeLimit - 300) == 0 && frame % 60 == 0)
                    {
                        Cloud newCloud = new Cloud();
                        cloudAnimation = new Animation(cloudTexture, Vector2.Zero, 32, 16, 1, 30, Color.White, ((float)rnd.Next(3, 7) / 2) * virtualScale, true);
                        newCloud.Initialize(cloudAnimation, new Vector2(0 - cloudAnimation.FrameWidth * virtualScale, rnd.Next(0, (int)(500 * virtualScale))), (float)rnd.Next(5, 11), rnd.Next(0, 2) == 1);
                        entities.Add(newCloud);
                    }
                    if (frame / (60 * (int)timeLimit + 600) == 0 && frame % 600 == 300)
                    {
                        instaKill = false;
                        killBonus = false;
                        zoneShot = false;
                    }

                    UpdateShoots(gameTime);

                    if (frame / (60 * (int)timeLimit) == 1)
                    {
                        gameState.Pop();
                        frame = 0;
                        bool finalBonus = true;
                        foreach (Entity val in entities)
                        {
                            finalBonus &= !val.Active;
                        }
                        if (finalBonus)
                        {
                            score += 100;
                        }
                        entities.Clear();
                        for (int i = 0; i < 20; i++)
                        {
                            entities.Add(new Cloud());
                        }
                        foreach (Entity val in entities)
                        {
                            cloudAnimation = new Animation(cloudTexture, Vector2.Zero, 32, 16, 1, 30, Color.White, ((float)rnd.Next(3, 7) / 2) * virtualScale, true);
                            val.Initialize(cloudAnimation, new Vector2(rnd.Next(0, (int)(500 * virtualScale)), rnd.Next(0, (int)(500 * virtualScale))), (float)rnd.Next(5, 11), rnd.Next(0, 2) == 1);
                        }
                        lastScore = score;
                        score = 0;
                        if (lastScore > bestScore)
                        {
                            bestScore = lastScore;
                            var store = IsolatedStorageFile.GetUserStoreForApplication();
                            string scoreFile = "score.txt";
                            if (store.FileExists(scoreFile))
                            {
                                var fs = store.OpenFile(scoreFile, FileMode.Open);
                                using (StreamWriter sw = new StreamWriter(fs))
                                {
                                    sw.Write(bestScore);
                                }
                            }
                            else
                            {
                                var fs = store.CreateFile(scoreFile);
                                using (StreamWriter sw = new StreamWriter(fs))
                                {
                                    sw.Write(bestScore);
                                }
                            }
                        }
                    }

                    //Exit
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    {
                        gameState.Pop();
                        frame = 0;
                        entities.Clear();
                        for (int i = 0; i < 20; i++)
                        {
                            entities.Add(new Cloud());
                        }
                        foreach (Entity val in entities)
                        {
                            cloudAnimation = new Animation(cloudTexture, Vector2.Zero, 32, 16, 1, 30, Color.White, ((float)rnd.Next(3, 7) / 2) * virtualScale, true);
                            val.Initialize(cloudAnimation, new Vector2(rnd.Next(0, (int)(500 * virtualScale)), rnd.Next(0, (int)(500 * virtualScale))), (float)rnd.Next(5, 11), rnd.Next(0, 2) == 1);
                        }
                        score = 0;
                    }
                    break;
                case _gameState.Options:
                    gameState.Pop();
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
            TouchCollection touchCollection = TouchPanel.GetState();

            if (touchCollection.Count > 0)
            {
                //Only Fire Select Once it's been released
                if (touchCollection[0].State == TouchLocationState.Moved)
                {
                    player.setPosition(touchCollection[0].Position.X, touchCollection[0].Position.Y);
                }
            }

            //previousMouseState = currentMouseState;
            //currentMouseState = Mouse.GetState();

            //Get Mouse State then Capture the Button type and Respond Button Press
            //Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);

            /*if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                player.Position = mousePosition;
            }*/

            // Get Thumbstick Controls
            //player.Position.X += currentGamePadState.ThumbSticks.Left.X * player.moveSpeed;
            //player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * player.moveSpeed;
            /*
            // Use the Keyboard / Dpad
            if (currentKeyboardState.IsKeyDown(Keys.A) || currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                player.Position.X -= player.moveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.D) || currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                player.Position.X += player.moveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.W) || currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                player.Position.Y -= player.moveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.S) || currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                player.Position.Y += player.moveSpeed;
            }
            */
            // Make sure that the player does not go out of bounds
            player.setPosition(MathHelper.Clamp(player.Position.X, 0, 500 * virtualScale),
                MathHelper.Clamp(player.Position.Y, 0, 500 * virtualScale));
        }

        /// <summary>
        /// Update entities movements.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void UpdateEntities(GameTime gameTime)
        {
            foreach (Entity value in entities)
            {
                if (value.GetType() == TypeCloud.GetType())
                {
                    if (((Cloud)value).Direction)
                    {
                        value.setPosition(value.Position.X + value.MoveSpeed * virtualScale, value.Position.Y);
                        if (value.Position.X > (500 + value.Width) * virtualScale)
                        {
                            value.setPosition(0 - value.Width * virtualScale, value.Position.Y);
                        }
                    }
                    else
                    {
                        value.setPosition(value.Position.X - value.MoveSpeed * virtualScale, value.Position.Y);
                        if (value.Position.X < 0 - value.Width * virtualScale)
                        {
                            value.setPosition((500 + value.Width) * virtualScale, value.Position.Y);
                        }
                    }
                }
                if (value.GetType() == TypeBaloon.GetType())
                {
                    value.setPosition(value.Position.X, value.Position.Y - value.MoveSpeed * virtualScale);
                    if (value.Position.Y < 0 - value.Height * virtualScale)
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
                        value.setPosition(value.Position.X + value.MoveSpeed * virtualScale, value.Position.Y);
                        if (value.Position.X > (500 + value.Width) * virtualScale)
                        {
                            value.setPosition(0 - value.Width * virtualScale, value.Position.Y);
                        }
                    }
                    else
                    {
                        value.setPosition(value.Position.X - value.MoveSpeed * virtualScale, value.Position.Y);
                        if (value.Position.X < 0 - value.Width * virtualScale)
                        {
                            value.setPosition((500 + value.Width) * virtualScale, value.Position.Y);
                        }
                    }
                }
                if (value.GetType() == TypeBaloon.GetType())
                {
                    value.setPosition(value.Position.X, value.Position.Y - value.MoveSpeed * virtualScale);
                    if (value.Position.Y < 0 - value.Height * virtualScale)
                    {
                        value.setPosition(value.Position.X, (500 + value.Height) * virtualScale);
                    }
                }
            }
        }

        /// <summary>
        /// update shots of player
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateShoots(GameTime gameTime)
        {
            TouchCollection touchCollection = TouchPanel.GetState();
            if (touchCollection.Count > 0)
            {
                if (touchCollection[0].State == TouchLocationState.Moved)
                {
                    shot = true;
                }
            }
            if (shot)
            {
                foreach (Entity value in entities)
                {
                    if (value.Position.X - (value.Width / 2) * virtualScale < player.Position.X 
                        && player.Position.X < value.Position.X + (value.Width / 2) * virtualScale
                    && value.Position.Y - (value.Height / 2) * virtualScale < player.Position.Y 
                    && player.Position.Y < value.Position.Y + (value.Height / 2) * virtualScale
                    && (touchCollection.Count == 0 && shot))
                    {
                        score = value.getShooted(score, this);
                    }
                    if (zoneShot)
                    {
                        if (value.Position.X - (value.Width) * virtualScale < player.Position.X
                        && player.Position.X < value.Position.X + (value.Width) * virtualScale
                    && value.Position.Y - (value.Height) * virtualScale < player.Position.Y
                    && player.Position.Y < value.Position.Y + (value.Height) * virtualScale
                    && (touchCollection.Count == 0 && shot))
                        {
                            score = value.getShooted(score, this);
                        }
                    }
                }
                if (touchCollection.Count == 0)
                {
                    shot = false;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            background.Draw(spriteBatch);
            switch (gameState.Peek())
            {
                case _gameState.Loading:
                    spriteBatch.Draw(panel, panelPosition, null, Color.White, (float)(Math.PI * 0.5f), new Vector2(0, 500), 1f * virtualScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(_font, "Loading...", Vector2.Zero, Color.Black, 0f, Vector2.Zero, virtualScale, SpriteEffects.None, 0f);
                    break;
                case _gameState.Menu:
                    foreach (Entity val in menuEntities)
                    {
                        val.Draw(spriteBatch);
                    }
                    spriteBatch.Draw(panel, panelPosition, null, Color.White, (float)(Math.PI * 0.5f), new Vector2(0, 500), 1f * virtualScale, SpriteEffects.None, 0f);
                    icon.Draw(spriteBatch);
                    scoreScreen.Draw(spriteBatch);
                    spriteBatch.DrawString(_font, "Cloud Hunt", new Vector2(150 * virtualScale, 80 * virtualScale), Color.DarkBlue, 0f, Vector2.Zero, 3f * virtualScale, SpriteEffects.None, 0f);
                    if (lastScore > 0)
                    {
                        spriteBatch.DrawString(_font, "Last Score: " + lastScore, new Vector2(35 * virtualScale, 535 * virtualScale), Color.Black, 0f, Vector2.Zero, virtualScale, SpriteEffects.None, 0f);
                    }
                    if (bestScore > 0)
                    {
                        spriteBatch.DrawString(_font, "Best Score: " + bestScore, new Vector2(35 * virtualScale, 550 * virtualScale), Color.Black, 0f, Vector2.Zero, virtualScale, SpriteEffects.None, 0f);
                    }
                    break;
                case _gameState.Playing:
                    foreach (Entity val in entities)
                    {
                        val.Draw(spriteBatch);
                    }
                    player.Draw(spriteBatch);
                    spriteBatch.Draw(panel, panelPosition, null, Color.White, (float)(Math.PI * 0.5f), new Vector2(0, 500), 1f * virtualScale, SpriteEffects.None, 0f);
                    bonusScreen.Draw(spriteBatch);
                    spriteBatch.DrawString(_font, "Time : " + (int)(timeLimit - frame / 60), new Vector2(35 * virtualScale, 535 * virtualScale), Color.Black, 0f, Vector2.Zero, virtualScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(_font, "Score: " + score, new Vector2(35 * virtualScale, 550 * virtualScale), Color.Black, 0f, Vector2.Zero, virtualScale, SpriteEffects.None, 0f);
                    if (zoneShot) spriteBatch.DrawString(_font, "Zone Shot Activated", new Vector2(135 * virtualScale, 535 * virtualScale), Color.Black, 0f, Vector2.Zero, virtualScale, SpriteEffects.None, 0f);
                    if (killBonus) spriteBatch.DrawString(_font, "Bonus Points Activated", new Vector2(135 * virtualScale, 550 * virtualScale), Color.Black, 0f, Vector2.Zero, virtualScale, SpriteEffects.None, 0f);
                    if (instaKill) spriteBatch.DrawString(_font, "Insta-Kill Activated", new Vector2(135 * virtualScale, 565 * virtualScale), Color.Black, 0f, Vector2.Zero, virtualScale, SpriteEffects.None, 0f);
                    break;
                case _gameState.Options:
                    spriteBatch.Draw(panel, panelPosition, null, Color.White, (float)(Math.PI * 0.5f), new Vector2(0, 500), 1f * virtualScale, SpriteEffects.None, 0f);
                    break;
                case _gameState.Exit:
                    spriteBatch.Draw(panel, panelPosition, null, Color.White, (float)(Math.PI * 0.5f), new Vector2(0, 500), 1f * virtualScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(_font, "Good bye", new Vector2(150 * virtualScale, 230 * virtualScale), Color.DarkBlue, 0f, Vector2.Zero, 3f * virtualScale, SpriteEffects.None, 0f);
                    break;
                default:
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
