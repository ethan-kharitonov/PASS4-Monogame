using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    class Screen
    {
        private Point origin;

        private int width;
        private int height;

        public static SpriteBatch spriteBatch;

        public Screen(Point origin, int width, int height)
        {
            this.origin = origin;
            this.width = width;
            this.height = height;
        }

        public void Draw(Texture2D image, Rectangle box)
        {
            box.Location += origin;
            spriteBatch.Draw(image, box, Color.White);
        }
    }
}
