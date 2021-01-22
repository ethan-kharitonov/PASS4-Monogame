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

            foreach(Vector2 vertex in Helper.GetVertecies(box))
            {
                if(!Helper.IsPointInOrOnRectangle(vertex, new Rectangle(origin, new Point(width, height))))
                {
                    return;
                }
            }
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
