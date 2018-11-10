using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Animations
{
    class Animation
    {
        // The image representing the collection of images used for animation
        Texture2D spriteStrip;
        // The scale used to display the sprite strip
        float scale;
        // The time since we last updated the frame
        int elapsedTime;
        // The time we display a frame until the next one
        int frameTime;
        // The number of frames that the animation contains
        int frameCount;
        // The index of the current frame we are displaying
        int currentFrame;
        // The color of the frame we will be displaying
        Color color;
        // The area of the image strip we want to display
        Rectangle sourceRect = new Rectangle();
        // The area where we want to display the image strip in the game
        Rectangle destinationRect = new Rectangle();
        // Width of a given frame
        private int frameWidth;
        public int FrameWidth { get { return (int)(frameWidth * scale); } }
        // Height of a given frame
        private int frameHeight;
        public int FrameHeight { get { return (int)(frameHeight * scale); } }
        // The state of the Animation
        private bool active;
        public bool Active { get { return active; } set { active = value; } }
        // Determines if the animation will keep playing or deactivate after one run
        private bool looping;
        public bool Looping { get { return looping; } }
        // Width of a given frame
        private Vector2 position;
        public Vector2 Position { get { return position; } set { position = value; } }

        public Animation(Texture2D Texture, Vector2 Position, int frameWidth,
            int frameHeight, int frameCount, int frametime,
            Color color, float scale, bool Looping)
        {
            // Keep a local copy of the values passed in
            this.color = color;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.frameCount = frameCount;
            this.frameTime = frametime;
            this.scale = scale;

            this.looping = Looping;
            this.position = Position;
            this.spriteStrip = Texture;

            // Set the time to zero
            this.elapsedTime = 0;
            this.currentFrame = 0;

            // Set the Animation to active by default
            this.active = true;
        }

        public void Update(GameTime gameTime)
        {
            // Do not update the game if we are not active
            if (!this.active) return;
            // Update the elapsed time
            this.elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            // If the elapsed time is larger than the frame time we need to switch frames
            if (this.elapsedTime > this.frameTime)
            {
                // Move to the next frame
                this.currentFrame++;

                // If the currentFrame is equal to frameCount reset currentFrame to zero
                if (this.currentFrame == this.frameCount)
                {
                    this.currentFrame = 0;
                    // If we are not looping deactivate the animation
                    if (!this.looping) this.active = false;
                }

                // Reset the elapsed time to zero
                this.elapsedTime = 0;
            }

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the Frame width
            this.sourceRect = new Rectangle(this.currentFrame * this.frameWidth, 0, this.frameWidth, this.frameHeight);
            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            this.destinationRect = new Rectangle((int)this.position.X - (int)(this.frameWidth * this.scale) / 2,
            (int)this.position.Y - (int)(this.frameHeight * this.scale) / 2,
            (int)(this.frameWidth * this.scale),
            (int)(this.frameHeight * this.scale));
        }

        // Draw the Animation Strip
        public void Draw(SpriteBatch spriteBatch)
        {
            // Only draw the animation when we are active
            if (this.active)
            {
                spriteBatch.Draw(this.spriteStrip, this.destinationRect, this.sourceRect, this.color);
            }
        }
    }
}
