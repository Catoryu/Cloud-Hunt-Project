using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Animations;

namespace Hunt
{
    class Entity
    {
        private Animation texture;
        private Vector2 position;
        public Vector2 Position { get { return position; } }
        private bool active;
        public bool Active { get { return active; } set { active = value; } }
        private bool direction;
        public bool Direction { get { return direction; } }
        public int Width { get { return texture.FrameWidth; } }
        public int Height { get { return texture.FrameHeight; } }
        private float moveSpeed;
        public float MoveSpeed { get { return moveSpeed; } }

        public void Initialize(Animation Texture, Vector2 Position, float Speed, bool toRight)
        {
            this.texture = Texture;
            this.position = Position;
            this.active = true;
            this.direction = toRight;
            this.moveSpeed = Speed;
        }

        public void Update(GameTime gameTime)
        {
            if (this.active)
            {
                this.texture.Position = Position;
                this.texture.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (this.active)
            {
                this.texture.Draw(sb);
            }
        }

        public void setPosition(float x, float y)
        {
            this.position.X = x;
            this.position.Y = y;
        }
    }
}
