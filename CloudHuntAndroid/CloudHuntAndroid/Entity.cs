using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Animations;
using CloudHuntAndroid;

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

        public virtual int getShooted(int score, CloudHunt game)
        {
            this.active = false;
            return score;
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

        public override int getShooted(int score, CloudHunt game)
        {
            score = base.getShooted(score, game);
            //Augmentation de points
            score += 5;
            if (game.killBonus)
            {
                score += 15;
            }
            return score;
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

        public override int getShooted(int score, CloudHunt game)
        {
            score = base.getShooted(score, game);
            switch (this.color)
            {
                case BaloonColor.blue:
                    //Gagne grande quantité de points
                    score += 100;
                    break;
                case BaloonColor.green:
                    //Prochains kills x5 points
                    game.killBonus = true;
                    break;
                case BaloonColor.red:
                    //Tir de zone
                    game.zoneShot = true;
                    break;
                case BaloonColor.yellow:
                    //Clean à points divisés par 2
                    game.instaKill = true;
                    break;
            }
            return score;
        }
    }
}
