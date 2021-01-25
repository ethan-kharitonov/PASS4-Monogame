using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    class Bar
    {
        private static readonly Texture2D backgroundImg = Helper.LoadImage("Images/BarBg");
        private static readonly Texture2D insideImg = Helper.LoadImage("Images/EXP 2");

        private static readonly Point margins = new Point(17, 3);
        private Rectangle box;

        private float amount = 0;
        public float FullAmount;


        public Bar(Rectangle box)
        {
            this.box = box;
        }

        public void Update(float amount)
        {
            this.amount = (Helper.Clamp(0, amount, FullAmount) / FullAmount) * (box.Width - 2 * margins.X);
            this.amount += 2;
        }

        public void Draw(Screen screen)
        {
            screen.Draw(backgroundImg, box);
            screen.Draw(insideImg, new Rectangle(box.X + margins.X, box.Y + margins.Y, (int)Math.Round(amount), box.Height - 2 * margins.Y + 1));
        }

        internal void Reset() => amount = 0;
    }
}
