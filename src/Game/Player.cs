using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Game
{
    class Player : GameObject
    {
        private static readonly Texture2D image = Helper.LoadImage("Images/Player");

        private static readonly float imgScaleFactor = 0.06f;
        private static readonly int width = (int)Math.Round(image.Width * imgScaleFactor);
        private static readonly int height = (int)Math.Round(image.Height * imgScaleFactor);

        private bool onGround = true;

        private float gravity = 1;
        private float xSpeed = 2f;
        private float initalJumpSpeed = -13f;

        private float xTargetPosition;
        private float xTargetVelocity = 0;


        public Player(int x, int y) : base(image, x + 1, y, width, height)
        {
            TruePosition = new Vector2(TruePosition.X + MainGame.CELL_SIDE_LENGTH - width, TruePosition.Y);
            xTargetPosition = TruePosition.X;
        }

        public override void Update()
        {
            if (Velocity.X != 0 && Math.Abs(xTargetPosition - TruePosition.X) < xSpeed)
            {
                TruePosition = new Vector2(xTargetPosition, TruePosition.Y);

                Velocity.X = 0;
                xTargetVelocity = 0;
            }

            if (Velocity.Y != 0 && xTargetVelocity != Velocity.X)
            {
                Velocity.X = xTargetVelocity;
            }

            Velocity.Y += gravity;
        }

        public void LoadNextCommand(char command)
        {
            xTargetVelocity = 0;
            switch (command)
            {
                case 'A':
                    Velocity.X = -xSpeed;
                    xTargetPosition = ((int)(TruePosition.X / MainGame.CELL_SIDE_LENGTH) - 1) * MainGame.CELL_SIDE_LENGTH;
                    break;
                case 'D':
                    Velocity.X = xSpeed;
                    xTargetPosition = ((int)(TruePosition.X / MainGame.CELL_SIDE_LENGTH) + 2) * MainGame.CELL_SIDE_LENGTH - width;
                    break;
                case 'E':
                    Velocity.Y = initalJumpSpeed;
                    xTargetVelocity = xSpeed;
                    xTargetPosition = ((int)(TruePosition.X / MainGame.CELL_SIDE_LENGTH) + 2) * MainGame.CELL_SIDE_LENGTH - width;
                    break;
                case 'Q':
                    Velocity.Y = initalJumpSpeed;
                    xTargetVelocity = -xSpeed;
                    xTargetPosition = ((int)(TruePosition.X / MainGame.CELL_SIDE_LENGTH) - 1) * MainGame.CELL_SIDE_LENGTH;
                    break;
            }
        }

        public override void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides)
        {
            otherGameObject.CollideWith(this, sides);
        }

        public override void CollideWith(Wall wall, IEnumerable<Side> sides)
        {
            if (sides.Contains(Side.Bottom))
            {
                onGround = true;
            }
        }

        public override void CollideWith(Crate crate, IEnumerable<Side> sides)
        {
            if (sides.Contains(Side.Bottom))
            {
                onGround = true;
            }

            crate.Push(sides);
        }

        public override void CollideWith(Gem crate, IEnumerable<Side> sides)
        {
            base.CollideWith(crate, sides);
        }
    }
}
