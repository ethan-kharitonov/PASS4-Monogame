//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: Bar.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: A progress bar display
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PASS4
{
    class Bar
    {
        //Stores the image of the bar itself and the image that fills it
        private static readonly Texture2D backgroundImg = Helper.LoadImage("Images/BarBg");
        private static readonly Texture2D insideImg = Helper.LoadImage("Images/EXP 2");

        //Stores the distance from the TL corner of the bar to the TL corner of the filling
        private static readonly Point margins = new Point(17, 3);

        //Stores the rectangle the bar will be drawn to
        private Rectangle box;

        //stores the full amount required to fill the bar and the current amount
        private float amount = 0;
        public float FullAmount;


        /// <summary>
        /// Creates a bar at a given rectangle
        /// </summary>
        /// <param name="box">The rectangle of the bar</param>
        public Bar(Rectangle box) => this.box = box;

        /// <summary>
        /// Updates the current amount
        /// </summary>
        /// <param name="amount">New amount value</param>
        public void Update(float amount)
        {
            //Calculates the pecentage of the bar that is full 
            this.amount = (Helper.Clamp(0, amount, FullAmount) / FullAmount) * (box.Width - 2 * margins.X);
            this.amount += 2;
        }

        /// <summary>
        /// Draws the bar and the inside bar
        /// </summary>
        /// <param name="screen">The screen to which it will be drawn to</param>
        public void Draw(Screen screen)
        {
            //Draw the bar and the inside bar
            screen.Draw(backgroundImg, box);
            screen.Draw(insideImg, new Rectangle(box.X + margins.X, box.Y + margins.Y, (int)Math.Round(amount), box.Height - 2 * margins.Y + 1));
        }
    }
}
