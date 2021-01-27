//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: Spike.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: The spike GameObject
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PASS4
{
    class Spike : GameObject
    {
        //Stores the image of a spike
        private static readonly Texture2D image = Helper.LoadImage("Images/SpikeImg");

        /// <summary>
        /// Creates a spike given coordinates
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        public Spike(int x, int y) : base(image, x, y, LevelContainer.CELL_SIDE_LENGTH, LevelContainer.CELL_SIDE_LENGTH)
        {
        }

        /// <summary>
        /// Informs collision with this spike to another GameObject
        /// </summary>
        /// <param name="otherGameObject">The object that collided with this spike</param>
        /// <param name="sides">The sides of the collision</param>
        /// <seealso cref="InformCollisionTo(GameObject, IEnumerable{Side})"/>
        public override void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides)
        {
            otherGameObject.CollideWith(this, sides);
        }
    }
}
