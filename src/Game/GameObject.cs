//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: GameObject.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: The abstract base class for any physical object in the game
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace PASS4
{
    abstract class GameObject
    {
        //Stores the image of the gameObject
        private readonly Texture2D image;

        //Stores the rectangle it is drawn to and its true (float) position
        private Rectangle box;
        private Vector2 truePosition;

        //Stores the velocity of the gameObject
        public Vector2 Velocity;

        //Events invoked when the object should be deleted or moved
        public event Action<GameObject> MoveReady;
        public event Action<GameObject> DeleteReady;

        //Indicates whether other objects can pass through this object
        protected bool isCollidable = true;
        public bool IsCollidable => isCollidable;

        /// <summary>
        /// The point one udner and to the right of the top left point of the grid square the object is in
        /// Used to check if it is above something else
        /// </summary>
        public Point TopLeftGridPoint => new Point(box.X / LevelContainer.CELL_SIDE_LENGTH * LevelContainer.CELL_SIDE_LENGTH, box.Y / LevelContainer.CELL_SIDE_LENGTH * LevelContainer.CELL_SIDE_LENGTH) + new Point(1);

        /// <summary>
        /// Creates a new gameobject with a given image, position and dimentions
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <param name="width">The width of the object</param>
        /// <param name="height">The height of the object</param>
        public GameObject(Texture2D image, int x, int y, int width, int height)
        {
            //Store its image and position
            this.image = image;
            TruePosition = new Vector2(x, y);

            //Construct its rectangle
            box = new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Creates a gameobject with a position and dimenstions but no image (used for invisible walls on the sides of the screen)
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <param name="width">The width of the object</param>
        /// <param name="height">The height of the object</param>
        public GameObject(int x, int y, int width, int height)
        {
            //Saves the position and constructs the rectangle
            TruePosition = new Vector2(x, y);
            box = new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Used to change the objects true position
        /// Automatically updates the box position as well
        /// </summary>
        public Vector2 TruePosition
        {
            get => truePosition;

            set
            {
                //Updates true position and box position
                truePosition = value;
                box.Location = value.ToPoint();
            }
        }

        /// <summary>
        /// Returns the objects rectangle
        /// </summary>
        public Rectangle Box => box;

        /// <summary>
        /// Moves the game object by a given amount on each axis
        /// </summary>
        /// <param name="x">The amount to move on the x axis</param>
        /// <param name="y">The amount to move on the y axis</param>
        public void Move(float x, float y) => TruePosition = new Vector2(TruePosition.X + x, truePosition.Y + y);

        /// <summary>
        /// Moves the object by a given vector
        /// </summary>
        /// <param name="delta">The amount to move by</param>
        /// <seealso cref="Move(float, float)"/>
        public void Move(Vector2 delta) => Move(delta.X, delta.Y);

        /// <summary>
        /// Virtual function that should be called every frame by who ever instantiated a gameObject
        /// </summary>
        public virtual void Update()
        {

        }

        /// <summary>
        /// Draws the gameObject to a given screen
        /// </summary>
        /// <param name="screen">A tool used to draw the gameObject</param>
        public void Draw(Screen screen)
        {
            //Leave the function if the object has no image, otherwise draw it
            if(image == null)
            {
                return;
            }

            screen.Draw(image, box);
        }

        /// <summary>
        /// A function used by derrived classes to invoke their move event
        /// </summary>
        /// <param name="delta">The amount the want to move by</param>
        protected void InvokeMoveReady(Vector2 delta)
        {
            //Stores the objects old velocity, then sets it to delta
            Vector2 oldVelocity = Velocity;
            Velocity = delta;
            
            //Move by delta, then restore the old velocity
            MoveReady?.Invoke(this);
            Velocity = oldVelocity;
        }

        /// <summary>
        /// A function used by derrived classes to invoke their delete event on themselves
        /// </summary>
        protected void InvokeDeleteReady() => DeleteReady?.Invoke(this);

        /// <summary>
        /// A function used by derrived classes to invoke their delete event on other gameObejcts
        /// </summary>
        /// <param name="gameObject">The gameObject that should be deleted</param>
        protected void InvokeDeleteReady(GameObject gameObject) => DeleteReady?.Invoke(gameObject);

        /// <summary>
        /// A function each derrived class has to implement that lets them inform collision with themselves to other gameObejcts
        /// </summary>
        /// <param name="otherGameObject">The object they are infroming collison to</param>
        /// <param name="sides">The sides that this gameObject was collided on</param>
        public abstract void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides);

        /// <summary>
        /// Called when this gameObject collides with a Player
        /// </summary>
        /// <param name="player">The player that it collided with</param>
        /// <param name="sides">The sides that this gameObject was collided on</param>
        public virtual void CollideWith(Player player, IEnumerable<Side> sides)
        {

        }

        /// <summary>
        /// Called when this gameObject collides with a Wall
        /// </summary>
        /// <param name="wall">The wall that it collided with</param>
        /// <param name="sides">The sides that this gameObject was collided on</param>
        public virtual void CollideWith(Wall wall, IEnumerable<Side> sides)
        {

        }

        /// <summary>
        /// Called when this gameObject collides with a Crate
        /// </summary>
        /// <param name="Crate">The Crate that it collided with</param>
        /// <param name="sides">The sides that this gameObject was collided on</param>
        public virtual void CollideWith(Crate crate, IEnumerable<Side> sides)
        {

        }

        /// <summary>
        /// Called when this gameObject collides with a Gem
        /// </summary>
        /// <param name="Gem">The Gem that it collided with</param>
        /// <param name="sides">The sides that this gameObject was collided on</param>
        public virtual void CollideWith(Gem gem, IEnumerable<Side> sides)
        {

        }

        /// <summary>
        /// Called when this gameObject collides with a Spike
        /// </summary>
        /// <param name="Spike">The Spike that it collided with</param>
        /// <param name="sides">The sides that this gameObject was collided on</param>
        public virtual void CollideWith(Spike spike, IEnumerable<Side> sides)
        {

        }

        /// <summary>
        /// Called when this gameObject collides with a Key
        /// </summary>
        /// <param name="Key">The Key that it collided with</param>
        /// <param name="sides">The sides that this gameObject was collided on</param>
        public virtual void CollideWith(Key key, IEnumerable<Side> sides)
        {

        }

        /// <summary>
        /// Called when this gameObject collides with a Door
        /// </summary>
        /// <param name="Door">The Door that it collided with</param>
        /// <param name="sides">The sides that this gameObject was collided on</param>
        public virtual void CollideWith(Door door, IEnumerable<Side> sides)
        {

        }

        /// <summary>
        /// Indicates wheter this gameObject is not currently moving
        /// </summary>
        /// <returns></returns>
        public virtual bool IsStandingStill() => Velocity == Vector2.Zero;
    }
}
