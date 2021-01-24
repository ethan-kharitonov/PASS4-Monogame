using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Game
{
    abstract class GameObject
    {
        private readonly Texture2D image;

        private Rectangle box;
        private Vector2 truePosition;

        public Vector2 Velocity;

        public delegate void GameObjectEvent(GameObject gameObject);

        public event GameObjectEvent MoveReady;
        public event GameObjectEvent DeleteReady;


        public GameObject(Texture2D image, int x, int y, int width, int height)
        {
            this.image = image;
            TruePosition = new Vector2(x, y);

            box = new Rectangle(x, y, width, height);
        }

        public Vector2 TruePosition
        {
            get => truePosition;

            set
            {
                truePosition = value;
                box.Location = value.ToPoint();
            }
        }

        public Rectangle Box => box;

        public void Move(float x, float y) => TruePosition = new Vector2(TruePosition.X + x, truePosition.Y + y);

        public void Move(Vector2 delta) => Move(delta.X, delta.Y);

        public virtual void Update()
        {

        }

        public virtual void Draw(Screen screen)
        {
            screen.Draw(image, box);
        }

        protected void InvokeMoveReady(Vector2 delta)
        {
            Vector2 oldVelocity = Velocity;
            Velocity = delta;
            
            MoveReady?.Invoke(this);

            Velocity = oldVelocity;
        }

        protected void InvokeDeleteReady() => DeleteReady?.Invoke(this);

        public abstract void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides);

        public virtual void CollideWith(Player player, IEnumerable<Side> sides)
        {

        }

        public virtual void CollideWith(Wall wall, IEnumerable<Side> sides)
        {

        }

        public virtual void CollideWith(Crate crate, IEnumerable<Side> sides)
        {

        }

        public virtual void CollideWith(Gem crate, IEnumerable<Side> sides)
        {

        }

        public virtual void CollideWith(Spike spike, IEnumerable<Side> sides)
        {

        }

        public virtual void CollideWith(Key key, IEnumerable<Side> sides)
        {

        }

        public virtual void CollideWith(Door key, IEnumerable<Side> sides)
        {

        }

        public virtual bool IsStandingStill() => Velocity == Vector2.Zero;


    }
}
