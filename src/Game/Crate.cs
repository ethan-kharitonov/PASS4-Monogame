//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: Crate.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: The crate GameObject
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PASS4
{
    class Crate : GameObject
    {
        //Stores the image of the crate (the same for all the crates)
        private static readonly Texture2D image = Helper.LoadImage("Images/Crate");

        //Stores the gravity that gets applied on the crate
        private static float gravity = 1;

        //Stores a collection of crates that are currently directly above or below this crate
        private HashSet<Crate> cratesAbove = new HashSet<Crate>();
        private HashSet<Crate> cratesBelow = new HashSet<Crate>();

        //Gets invoked when the crate begins moving to a new square
        public event Func<Crate, bool> CrateMove;

        //Gets invoked when the crate collides witha a gem or a key
        public event Action CollideWithGem;
        public event Action CollideWithKey;


        /// <summary>
        /// Creates a new crate at the given coordinates
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <seealso cref="GameObject"/>
        public Crate(int x, int y) : base(image, x, y, LevelContainer.CELL_SIDE_LENGTH, LevelContainer.CELL_SIDE_LENGTH)
        {
        }

        /// <summary>
        /// Used to apply gravity every frame
        /// </summary>
        public override void Update() => Velocity.Y += gravity;

        /// <summary>
        /// Lets this crate inform collision to another gameObject
        /// </summary>
        /// <param name="otherGameObject">The other gameObject</param>
        /// <param name="sides">The sides of the collision</param>
        /// <seealso cref="GameObject.InformCollisionTo(GameObject, IEnumerable{Side})"/>
        public override void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides)
        {
            otherGameObject.CollideWith(this, sides);
        }

        /// <summary>
        /// Pushes the crate depending on the sides it was collided on
        /// </summary>
        /// <param name="sides">The side the other object collided on (Player)</param>
        public void Push(IEnumerable<Side> sides)
        {
            //If the crate is starting to move to a new square check that there is no collectables blocking it
            if (TruePosition.X % 45 == 0)
            {
                if (CrateMove.Invoke(this))
                {
                    return;
                }
            }

            //Check that there are no crates above it
            if (cratesAbove.Count != 0)
            {
                return;
            }

            //Try (collision) to move the crate depending on the side
            if(sides.Contains(Side.Right))
            {
                InvokeMoveReady(new Vector2(3, 0));
            }
            else if (sides.Contains(Side.Left))
            {
                InvokeMoveReady(new Vector2(-3, 0));
            }

            //Check if the crate still intersects with any of the crates it was above
            Rectangle slightlyLowerBox = Box;
            slightlyLowerBox.Location += new Point(0, 1);

            foreach (Crate crateBelow in cratesBelow)
            {
                if (!slightlyLowerBox.Intersects(crateBelow.Box) || Box.Bottom > crateBelow.Box.Top) 
                {
                    //Inform the other crate that this crate is no longer above it
                    crateBelow.StepOff(this);
                }
            }

        }

        /// <summary>
        /// Check if this crate landed on another crate
        /// </summary>
        /// <param name="crate">The other crate</param>
        /// <param name="sides">The sides of this crate that there was a collision on</param>
        public override void CollideWith(Crate crate, IEnumerable<Side> sides)
        {
            if (sides.Contains(Side.Bottom))
            {
                //Add to crates above and crates below accordingly
                crate.StepOnto(this);
                cratesBelow.Add(crate);
            }
        }

        /// <summary>
        /// Add this crate to another crates above list
        /// </summary>
        /// <param name="crate">The other crate</param>
        public void StepOnto(Crate crate) => cratesAbove.Add(crate);

        /// <summary>
        /// Remove this crate from the other crates above list
        /// </summary>
        /// <param name="crate">The other crate</param>
        public void StepOff(Crate crate) => cratesAbove.Remove(crate);

        /// <summary>
        /// Called when this crate collides with a gem
        /// </summary>
        /// <param name="gem">The gem</param>
        /// <param name="sides">The collision sides</param>
        public override void CollideWith(Gem gem, IEnumerable<Side> sides)
        {
            //Delete the gem and invoke the collided with gem event
            InvokeDeleteReady(gem);
            CollideWithGem.Invoke();
        }

        /// <summary>
        /// Called when this crate collides with a key
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="sides">The collision sides</param>
        public override void CollideWith(Key key, IEnumerable<Side> sides)
        {
            //Delete the key and invoke the collided with key event
            InvokeDeleteReady(key);
            CollideWithKey.Invoke();
        }
    }
}
