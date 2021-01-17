using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Game
{
    class Player : GameObject
    {
        private static readonly new Texture2D image = Helper.LoadImage("Images/Player");

        private static readonly float imgScaleFactor = 0.055f;
        private static readonly int width = (int)Math.Round(image.Width * imgScaleFactor);
        private static readonly int height = (int)Math.Round(image.Height * imgScaleFactor);

        private bool onGround = true;

        private float gravity = 1;
        private float xSpeed = 2;
        private float initalJumpSpeed = -10;
        public Player(int x, int y) : base(image, x, y, width, height)
        {

        }

        public override void Update()
        {
            KeyboardState kb = Keyboard.GetState();
            Velocity.X = xSpeed * (Convert.ToInt16(kb.IsKeyDown(Keys.D)) - Convert.ToInt32(kb.IsKeyDown(Keys.A)));

            Velocity.Y += gravity;

            if(onGround && kb.IsKeyDown(Keys.Space))
            {
                onGround = false;
                Velocity.Y = initalJumpSpeed;
            }

        }

        public override void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides)
        {
            otherGameObject.CollideWith(this, sides);
        }

        public override void CollideWith(Wall wall, IEnumerable<Side> sides) => onGround = sides.Contains(Side.Top);

        public override void CollideWith(Crate crate, IEnumerable<Side> sides)
        {
            if (sides.Contains(Side.Top)) 
            {
                onGround = true;
            }

            crate.Push(sides);
        }
    }
}
