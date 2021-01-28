//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: Player.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: The player game object

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
        //stores the player image
        private static readonly Texture2D image = Helper.LoadImage("Images/Player");

        //stores the players dimensions
        private static readonly int height = 45;
        private static readonly int width = 45;

        //indicates whether the player is on ground and is pushing
        private bool onGround = true;
        private bool isPushing = false;

        //the velocity variables of the player
        private float gravity = 1f;
        private float xSpeed = 3f;
        private float initalJumpSpeed = -10f;

        //the target velocity when jumping and target position when moving
        private float xTargetVelocity = 0;
        private float xTargetPosition;

        //indicates if moving on the Y
        private bool movingOnY = false;

        //indicates if hit roof last frame
        private bool HitWallFromBottom = false;

        //stores the last gem and key collided with
        private Gem lastCollidedGem = null;
        private Key lastCollectedKey = null;
        
        //stores the gem and key count
        private int keyCount = 0;
        private int gemCount = 0;

        //invoked when collect collectable to update input menu
        public event Action<int> KeyCollected;
        public event Action<int> GemCollected;

        /// <summary>
        /// Updates the key count
        /// </summary>
        private int KeyCount 
        {
            get => keyCount;
            set
            {
                keyCount = value;
                KeyCollected.Invoke(KeyCount);
            }
        }
        
        /// <summary>
        /// Updates the gem count
        /// </summary>
        public int GemCount
        {
            get => gemCount;
            private set
            {
                gemCount = value;
                GemCollected.Invoke(GemCount);
            }
        }

        //Invoked when hit spike
        public event Action HitSpike;

        /// <summary>
        /// creates a player given postion
        /// </summary>
        /// <param name="x">x pos</param>
        /// <param name="y">y pos</param>
        public Player(int x, int y) : base(image, x, y, width, height)
        {
            //sets true position to given position
            xTargetPosition = TruePosition.X;
        }

        /// <summary>
        /// Updates the player
        /// </summary>
        public override void Update()
        {
            //checks if the player is off ground
            if (TruePosition.Y % LevelContainer.CELL_SIDE_LENGTH != 0 && Velocity.Y != gravity)
            {
                onGround = false;
            }

            //tries to move on x if jumping
            if (movingOnY)
            {
                Velocity.X = xTargetVelocity;
            }

            //stop once hit target position
            if (Velocity.X != 0 && Math.Abs(xTargetPosition - TruePosition.X) < xSpeed)
            {
                TruePosition = new Vector2(xTargetPosition, TruePosition.Y);

                Velocity.X = 0;
                xTargetVelocity = 0;
            }

            //dont apply gravity if hit roof last frame
            if (!HitWallFromBottom)
            {
                Velocity.Y += gravity;
            }
            else
            {
                HitWallFromBottom = false;
            }

        }

        /// <summary>
        /// Load the next command and calculate the velicty
        /// </summary>
        /// <param name="command">the last user command</param>
        public void LoadNextCommand(char command)
        {
            //make sure the player is on a square grid
            if (TruePosition.X % 45 != 0)
            {
                TruePosition = new Vector2((float)Math.Round(TruePosition.X / LevelContainer.CELL_SIDE_LENGTH) * LevelContainer.CELL_SIDE_LENGTH, TruePosition.Y);
            }

            //reset all the last command variables
            movingOnY = false;
            isPushing = false;
            xTargetVelocity = 0;

            //excecute the right command
            switch (command)
            {
                case 'A':
                    //set velocity and target position
                    Velocity.X = -xSpeed;
                    xTargetPosition = TruePosition.X - LevelContainer.CELL_SIDE_LENGTH;
                    break;
                case 'D':
                    //set velocity and target position
                    Velocity.X = xSpeed;
                    xTargetPosition = TruePosition.X + LevelContainer.CELL_SIDE_LENGTH;
                    break;
                case 'E':
                    //set velocity and target position as well as target velocity and record moving on y
                    movingOnY = true;
                    Velocity.Y = initalJumpSpeed;
                    xTargetVelocity = xSpeed;
                    xTargetPosition = TruePosition.X + LevelContainer.CELL_SIDE_LENGTH; 
                    break;
                case 'Q':
                    //set velocity and target position as well as target velocity and record moving on y
                    movingOnY = true;
                    Velocity.Y = initalJumpSpeed;
                    xTargetVelocity = -xSpeed;
                    xTargetPosition = TruePosition.X - LevelContainer.CELL_SIDE_LENGTH; 
                    break;
                case '+':
                    //move right and push
                    LoadNextCommand('D');
                    isPushing = true;
                    break;
                case '-':
                    //move left and push
                    LoadNextCommand('A');
                    isPushing = true;
                    break;
                case 'C':
                    //check if collided with gem previews move
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

            //Delete the gems collided last command
            lastCollidedGem = null;
            lastCollectedKey = null;

        }

        /// <summary>
        /// Inform collision to another object
        /// </summary>
        /// <param name="otherGameObject">the other object</param>
        /// <param name="sides">collision side</param>
        public override void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides)
        {
            otherGameObject.CollideWith(this, sides);
        }

        /// <summary>
        /// handle wall collision
        /// </summary>
        /// <param name="wall">the wall collided</param>
        /// <param name="sides">the sieds collided on</param>
        public override void CollideWith(Wall wall, IEnumerable<Side> sides)
        {
            //if landed on wall set grounded to true
            if (sides.Contains(Side.Bottom))
            {
                onGround = true;
            }

            //if hit roof set hit roof to true
            if (sides.Contains(Side.Top))
            {
                HitWallFromBottom = true;
            }
        }

        /// <summary>
        /// handle crate collision
        /// </summary>
        /// <param name="crate">the crate collided</param>
        /// <param name="sides">the sieds collided on</param>
        public override void CollideWith(Crate crate, IEnumerable<Side> sides)
        {
            //if landed on create then set grounded to true
            if (sides.Contains(Side.Bottom))
            {
                onGround = true;
            }

            //push the crate if pusing
            if (isPushing)
            {
                crate.Push(sides);
            }
        }

        public override void CollideWith(Gem gem, IEnumerable<Side> sides)
        {
            if(Velocity.Y != 0)
            {
                return;
            }
            lastCollidedGem = gem;
        }

        /// <summary>
        /// Handle collision with spike
        /// </summary>
        /// <param name="spike">the collided spike</param>
        /// <param name="sides">side collided</param>
        public override void CollideWith(Spike spike, IEnumerable<Side> sides)
        {
            //invoke collided with spike
            HitSpike.Invoke();
        }

        /// <summary>
        /// Handle collision with key
        /// </summary>
        /// <param name="key">the key collided</param>
        /// <param name="sides">the side collided</param>
        public override void CollideWith(Key key, IEnumerable<Side> sides)
        {
            //if not falling save the key
            if (Velocity.Y != 0)
            {
                return;
            }
            lastCollectedKey = key;
        }

        /// <summary>
        /// Handle collision with door
        /// </summary>
        /// <param name="door">the door collided</param>
        /// <param name="sides">the side collided</param>
        public override void CollideWith(Door door, IEnumerable<Side> sides)
        {
            //if landed on door then grounded
            if (sides.Contains(Side.Bottom))
            {
                onGround = true;
            }


            //if have key then walk through
            if (KeyCount > 0)
            {
                door.WalkThrough();
                --KeyCount;
            }
        }

        /// <summary>
        /// indicates whether the player is standing
        /// </summary>
        /// <returns>True if standing, false otherwise</returns>
        public override bool IsStandingStill()
        {
            return base.IsStandingStill() && onGround;
        }

        /// <summary>
        /// Adds a gem to the player (used by crate)
        /// </summary>
        public void AddGem() => ++GemCount;

        /// <summary>
        /// Adds a key to the player (used by crate)
        /// </summary>
        public void AddKey() => ++KeyCount;

    }
}
