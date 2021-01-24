﻿using Microsoft.Xna.Framework.Graphics;
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

        //private static readonly float imgScaleFactor = 0.06f;
        private static readonly int height = 45;// (int)Math.Round(image.Height * imgScaleFactor);
        private static readonly int width = 45;// height * image.Width/image.Height;// (int)Math.Round(image.Width * imgScaleFactor);

        private bool onGround = true;
        private bool isPushing = false;

        private float gravity = 1;
        private float xSpeed = 2f;
        private float initalJumpSpeed = -11.5f;

        private float xTargetPosition;
        private float xTargetVelocity = 0;

        private bool keyCollected = false;
        private bool movingOnY = false;

        private bool HitWallFromBottom = false;

        public delegate void Notify();
        public event Notify HitSpike;

        public Player(int x, int y) : base(image, x, y, width, height)
        {
            xTargetPosition = TruePosition.X;
        }

        public override void Update()
        {
            if (TruePosition.Y % MainGame.CELL_SIDE_LENGTH != 0 && Velocity.Y != gravity)
            {
                onGround = false;
            }


            if(movingOnY)
            {
                Velocity.X = xTargetVelocity;
            }

            if (Velocity.X != 0 && Math.Abs(xTargetPosition - TruePosition.X) < xSpeed)
            {
                TruePosition = new Vector2(xTargetPosition, TruePosition.Y);

                Velocity.X = 0;
                xTargetVelocity = 0;
            }


            if (!HitWallFromBottom)
            {
                Velocity.Y += gravity;
            }
            else
            {
                HitWallFromBottom = false;
            }
            
        }

        public void LoadNextCommand(char command)
        {
            if(TruePosition.X % 45 != 0)
            {
                TruePosition = new Vector2((float)Math.Round(TruePosition.X / MainGame.CELL_SIDE_LENGTH) * MainGame.CELL_SIDE_LENGTH, TruePosition.Y);
            }

            movingOnY = false;
            isPushing = false;
            xTargetVelocity = 0;

            switch (command)
            {
                case 'A':
                    Velocity.X = -xSpeed;
                    xTargetPosition = TruePosition.X - MainGame.CELL_SIDE_LENGTH;//((int)(TruePosition.X / MainGame.CELL_SIDE_LENGTH) - 1) * MainGame.CELL_SIDE_LENGTH;
                    break;
                case 'D':
                    Velocity.X = xSpeed;
                    xTargetPosition = TruePosition.X + MainGame.CELL_SIDE_LENGTH;//((int)(TruePosition.X / MainGame.CELL_SIDE_LENGTH) + 2) * MainGame.CELL_SIDE_LENGTH - width;
                    break;
                case 'E':
                    movingOnY = true;
                    Velocity.Y = initalJumpSpeed;
                    xTargetVelocity = xSpeed;
                    xTargetPosition = TruePosition.X + MainGame.CELL_SIDE_LENGTH; //((int)(TruePosition.X / MainGame.CELL_SIDE_LENGTH) + 2) * MainGame.CELL_SIDE_LENGTH - width;
                    break;
                case 'Q':
                    movingOnY = true;
                    Velocity.Y = initalJumpSpeed;
                    xTargetVelocity = -xSpeed;
                    xTargetPosition = TruePosition.X - MainGame.CELL_SIDE_LENGTH; //((int)(TruePosition.X / MainGame.CELL_SIDE_LENGTH) - 1) * MainGame.CELL_SIDE_LENGTH;
                    break;
                case '+':
                    LoadNextCommand('D');
                    isPushing = true;
                    break;
                case '-':
                    LoadNextCommand('A');
                    isPushing = true;
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

            if (sides.Contains(Side.Top))
            {
                HitWallFromBottom = true;
            }
        }

        public override void CollideWith(Crate crate, IEnumerable<Side> sides)
        {
            if (sides.Contains(Side.Bottom))
            {
                onGround = true;
            }

            if (isPushing)
            {
                crate.Push(sides);
            }
        }

        public override void CollideWith(Gem crate, IEnumerable<Side> sides)
        {
            base.CollideWith(crate, sides);
        }

        public override void CollideWith(Spike spike, IEnumerable<Side> sides)
        {
            HitSpike.Invoke();
        }

        public override void CollideWith(Key key, IEnumerable<Side> sides)
        {
            keyCollected = true;
        }

        public override void CollideWith(Door door, IEnumerable<Side> sides)
        {
            if (keyCollected)
            {
                door.WalkThrough();
            }
        }


        public override bool IsStandingStill()
        {
            return base.IsStandingStill() && onGround;
        }
    }
}
