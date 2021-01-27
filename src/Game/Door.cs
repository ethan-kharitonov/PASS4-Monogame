//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: Door.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: The door GameObject
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PASS4
{
    class Door : GameObject
    {
        //Stores the door image (the same for all doors)
        private static readonly Texture2D image = Helper.LoadImage("Images/door1");

        /// <summary>
        /// Creates a new door at the given coordinates
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        public Door(int x, int y) : base(image, x, y, LevelContainer.CELL_SIDE_LENGTH, LevelContainer.CELL_SIDE_LENGTH)
        {

        }

        /// <summary>
        /// Informs collision with this door to another GameObject
        /// </summary>
        /// <param name="otherGameObject">The object that collided with this door</param>
        /// <param name="sides">The sides of the collision</param>
        /// <seealso cref="InformCollisionTo(GameObject, IEnumerable{Side})"/>
        public override void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides)
        {
            otherGameObject.CollideWith(this, sides);
        }

        /// <summary>
        /// Used by the player to delete the door
        /// </summary>
        public void WalkThrough() => InvokeDeleteReady();
    }
}
