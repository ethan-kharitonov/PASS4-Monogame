using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;

namespace PASS4
{
    class Player : GameObject
    {
        private static readonly Texture2D image = Helper.LoadImage("Images/Player");

        //private static readonly float imgScaleFactor = 0.06f;
        private static readonly int height = 45;// (int)Math.Round(image.Height * imgScaleFactor);
        private static readonly int width = 45;// height * image.Width/image.Height;// (int)Math.Round(image.Width * imgScaleFactor);

        private bool onGround = true;
        private bool isPushing = false;

        private float gravity = 1f;
        private float xSpeed = 3f;
        private float initalJumpSpeed = -11f;

        private float xTargetPosition;
        private float xTargetVelocity = 0;

        private bool movingOnY = false;

        private bool HitWallFromBottom = false;

        private Gem lastCollidedGem = null;
        private Key lastCollectedKey = null;
        
        private int keyCount = 0;
        private int gemCount = 0;

        public event Action<int> KeyCollected;
        public event Action<int> GemCollected;

        private int KeyCount 
        {
            get => keyCount;
            set
            {
                keyCount = value;
                KeyCollected.Invoke(KeyCount);
            }
        }
        public int GemCount
        {
            get => gemCount;
            private set
            {
                gemCount = value;
                GemCollected.Invoke(GemCount);
            }
        }

        public delegate void Notify();
        public event Notify HitSpike;


        public Player(int x, int y) : base(image, x, y, width, height)
        {
            xTargetPosition = TruePosition.X;
        }

        public override void Update()
        {
            if (TruePosition.Y % LevelContainer.CELL_SIDE_LENGTH != 0 && Velocity.Y != gravity)
            {
                onGround = false;
            }


            if (movingOnY)
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
            if (TruePosition.X % 45 != 0)
            {
                TruePosition = new Vector2((float)Math.Round(TruePosition.X / LevelContainer.CELL_SIDE_LENGTH) * LevelContainer.CELL_SIDE_LENGTH, TruePosition.Y);
            }

            movingOnY = false;
            isPushing = false;
            xTargetVelocity = 0;

            switch (command)
            {
                case 'A':
                    Velocity.X = -xSpeed;
                    xTargetPosition = TruePosition.X - LevelContainer.CELL_SIDE_LENGTH;//((int)(TruePosition.X / MainGame.CELL_SIDE_LENGTH) - 1) * MainGame.CELL_SIDE_LENGTH;
                    break;
                case 'D':
                    Velocity.X = xSpeed;
                    xTargetPosition = TruePosition.X + LevelContainer.CELL_SIDE_LENGTH;//((int)(TruePosition.X / MainGame.CELL_SIDE_LENGTH) + 2) * MainGame.CELL_SIDE_LENGTH - width;
                    break;
                case 'E':
                    movingOnY = true;
                    Velocity.Y = initalJumpSpeed;
                    xTargetVelocity = xSpeed;
                    xTargetPosition = TruePosition.X + LevelContainer.CELL_SIDE_LENGTH; //((int)(TruePosition.X / MainGame.CELL_SIDE_LENGTH) + 2) * MainGame.CELL_SIDE_LENGTH - width;
                    break;
                case 'Q':
                    movingOnY = true;
                    Velocity.Y = initalJumpSpeed;
                    xTargetVelocity = -xSpeed;
                    xTargetPosition = TruePosition.X - LevelContainer.CELL_SIDE_LENGTH; //((int)(TruePosition.X / MainGame.CELL_SIDE_LENGTH) - 1) * MainGame.CELL_SIDE_LENGTH;
                    break;
                case '+':
                    LoadNextCommand('D');
                    isPushing = true;
                    break;
                case '-':
                    LoadNextCommand('A');
                    isPushing = true;
                    break;
                case 'C':
                    if (lastCollidedGem != null)
                    {
                        ++GemCount;
                        InvokeDeleteReady(lastCollidedGem);
                    }

                    if (lastCollectedKey != null)
                    {
                        ++KeyCount;
                        InvokeDeleteReady(lastCollectedKey);
                    }
                    break;
            }

            lastCollidedGem = null;
            lastCollectedKey = null;

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

        public override void CollideWith(Gem gem, IEnumerable<Side> sides)
        {
            lastCollidedGem = gem;
        }

        public override void CollideWith(Spike spike, IEnumerable<Side> sides)
        {
            HitSpike.Invoke();
        }

        public override void CollideWith(Key key, IEnumerable<Side> sides)
        {
            lastCollectedKey = key;
        }

        public override void CollideWith(Door door, IEnumerable<Side> sides)
        {
            if (sides.Contains(Side.Bottom))
            {
                onGround = true;
            }

            if (KeyCount > 0)
            {
                door.WalkThrough();
                --KeyCount;
            }
        }
        public override bool IsStandingStill()
        {
            return base.IsStandingStill() && onGround;
        }

        public void AddGem() => ++GemCount;

        public void AddKey() => ++KeyCount;

    }
}
