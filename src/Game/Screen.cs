//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: Screen.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: An area of the screen that images are drawn to
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace PASS4
{
    class Screen
    {
        //The point that all other coordinates are relative to
        private Point origin;

        //The width and height of the screen
        private int width;
        private int height;

        //The SpriteBatch that is hared between all screens
        public static SpriteBatch spriteBatch;

        /// <summary>
        /// Creates a new screen at a location, with certian dimensions
        /// </summary>
        /// <param name="origin">The origin of the screen</param>
        /// <param name="width">The width of the screen</param>
        /// <param name="height">The height of the screen</param>
        public Screen(Point origin, int width, int height)
        {
            this.origin = origin;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Draws an image to the screen
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="box">The rectangle (relative to origin)</param>
        public void Draw(Texture2D image, Rectangle box)
        {
            box.Location += origin;
            spriteBatch.Draw(image, box, Color.White);
        }

        /// <summary>
        /// Draws a string to the screen
        /// </summary>
        /// <param name="spriteFont">The font</param>
        /// <param name="text">The text</param>
        /// <param name="position">The position relative to the origin</param>
        /// <param name="color">The color of the text</param>
        public void DrawText(SpriteFont spriteFont, string text, Vector2 position, Color color)
        {
            position += origin.ToVector2();
            spriteBatch.DrawString(spriteFont, text, position, color);
        }

        /// <summary>
        /// Gets the highest X value the screen reaches
        /// </summary>
        /// <returns>The highest X value the screen reaches</returns>
        public int GetMaxX() => origin.X + width;

        /// <summary>
        /// Gets the highest Y value the screen reaches
        /// </summary>
        /// <returns>The highest Y value the screen reaches</returns>
        public int GetMaxY() => origin.Y + height;
    }
}
