//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: Wall.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: The wall GameObject
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PASS4
{
    class Wall : GameObject
    {
        //Stores the image for a wall
        private static readonly Texture2D image = Helper.LoadImage("Images/Wall");

        /// <summary>
        /// Creates a Wall given coordinates
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        public Wall(int x, int y) : base(image, x, y, LevelContainer.CELL_SIDE_LENGTH, LevelContainer.CELL_SIDE_LENGTH)
        {
        }

        /// <summary>
        /// Creates a spike given coordinates and dimensions (used for the screen walls)
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="width">The width of the wall</param>
        /// <param name="height">The height of the wall</param>
        public Wall(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        /// <summary>
        /// Informs collision with this wall to another GameObject
        /// </summary>
        /// <param name="otherGameObject">The object that collided with this wall</param>
        /// <param name="sides">The sides of the collision</param>
        /// <seealso cref="InformCollisionTo(GameObject, IEnumerable{Side})"/>
        public override void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides)
        {
            otherGameObject.CollideWith(this, sides);
        }
    }
}
