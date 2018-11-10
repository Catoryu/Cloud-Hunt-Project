using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Animations;

namespace Hunt
{
    class Player
    {
        //public Texture2D PlayerTexture;
        public Animation Texture;
        public Vector2 Position;
        public bool Active;
        public int Width { get { return Texture.FrameWidth; } }
        public int Height { get { return Texture.FrameHeight; } }
        public float moveSpeed;

        public void Initialize(Animation a, Vector2 p)
        {
            Texture = a;
            Position = p;
            Active = true;
            moveSpeed = 5.0f;
            //Enable the FreeDrag gesture.
            TouchPanel.EnabledGestures = GestureType.FreeDrag;
        }

        public void Update(GameTime gameTime)
        {
            Texture.Position = Position;
            Texture.Update(gameTime);
        }

        public void Draw(SpriteBatch sb)
        {
            Texture.Draw(sb);
        }
    }
}
