//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: Key.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: The key GameObject
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace PASS4
{
    class Key : GameObject
    {
        //Stores the image of a key
        private static readonly Texture2D image = Helper.LoadImage("Images/Key_01");

        //The width and height of the key and the scale factor from the image size
        private static readonly float imgScaleFactor = 0.30f;
        private static readonly int width = (int)Math.Round(image.Width * imgScaleFactor);
        private static readonly int height = (int)Math.Round(image.Height * imgScaleFactor);

        /// <summary>
        /// Creates a new key given coordinates
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        public Key(int x, int y) : base(image, x, y, width, height)
        {
            //Indicates that the player can walk through keys
            isCollidable = false;

            //Centers the key in its square
            TruePosition = new Vector2(x + LevelContainer.CELL_SIDE_LENGTH / 2 - width / 2, y + LevelContainer.CELL_SIDE_LENGTH / 2 - height / 2);
        }

        /// <summary>
        /// Informs collision with this key to another GameObject
        /// </summary>
        /// <param name="otherGameObject">The object that collided with this key</param>
        /// <param name="sides">The sides of the collision</param>
        /// <seealso cref="InformCollisionTo(GameObject, IEnumerable{Side})"/>
        public override void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides)
        {
            otherGameObject.CollideWith(this, sides);
        }
    }
}
