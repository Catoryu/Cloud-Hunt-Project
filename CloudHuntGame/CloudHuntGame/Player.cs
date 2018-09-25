using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hunt
{
    class Player
    {
        public Texture2D Texture;
        public Vector2 Position;
        public bool Active;
        public int Width { get { return Texture.Width; } }
        public int Height { get { return Texture.Height; } }

        public void Initialize(Texture2D t, Vector2 p)
        {
            Texture = t;
            Position = p;
            Active = true;
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
