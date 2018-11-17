using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Animations;

namespace Hunt
{
    public abstract class Entity
    {
        private Animation texture;
        private Vector2 position;
        public Vector2 Position { get { return position; } }
        private bool active;
        public bool Active { get { return active; } set { active = value; } }
        public int Width { get { return texture.FrameWidth; } }
        public int Height { get { return texture.FrameHeight; } }
        private float moveSpeed;
        public float MoveSpeed { get { return moveSpeed; } }

        public virtual void Initialize(Animation Texture, Vector2 Position, float Speed, object specialParameter)
        {
            this.texture = Texture;
            this.position = Position;
            this.active = true;
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

        public virtual void getShooted()
        {
            this.active = false;
        }
    }

    public class Cloud : Entity
    {
        private bool direction;
        public bool Direction { get { return direction; } }

        public override void Initialize(Animation Texture, Vector2 Position, float Speed, object specialParameter)
        {
            base.Initialize(Texture, Position, Speed, specialParameter);
            this.direction = (bool)specialParameter;
        }
    }

    class Baloon : Entity
    {
        public enum BaloonColor
        {
            red,
            green,
            blue,
            yellow
        }
        private BaloonColor color;
        public BaloonColor Color { get { return color; } }

        public override void Initialize(Animation Texture, Vector2 Position, float Speed, object specialParameter)
        {
            base.Initialize(Texture, Position, Speed, specialParameter);
            this.color = (BaloonColor)specialParameter;
        }

        public override void getShooted()
        {
            base.getShooted();
            switch(this.color)
            {
                case BaloonColor.blue:
                    //Gagne grande quantité de points
                    break;
                case BaloonColor.green:
                    //Prochains kills x5 points
                    break;
                case BaloonColor.red:
                    //Tir de zone
                    break;
                case BaloonColor.yellow:
                    //Clean à points divisés par 2
                    break;
            }
        }
    }
}
