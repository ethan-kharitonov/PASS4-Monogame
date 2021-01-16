using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PASS4
{
    abstract class GameObject
    {
        private readonly Texture2D image;

        private Rectangle box;
        private Vector2 truePosition;

        public Vector2 Velocity;

        public delegate void Notify(GameObject gameObject);

        public event Notify MoveReady;
        public event Notify DeleteReady;


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

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, box, Color.White);
        }

        protected void InvokeMoveReady() => MoveReady?.Invoke(this);

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

    }
}
