using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PASS4
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

        public Screen(Point origin) => this.origin = origin;

        public void Draw(Texture2D image, Rectangle box)
        {
            box.Location += origin;
            spriteBatch.Draw(image, box, Color.White);
        }

        public void DrawText(SpriteFont spriteFont, string text, Vector2 position, Color color)
        {
            position += origin.ToVector2();
            spriteBatch.DrawString(spriteFont, text, position, color);
        }

        public int GetMaxX() => origin.X + width;

        public int GetMaxY() => origin.Y + height;
    }
}
