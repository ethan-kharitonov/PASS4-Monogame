using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    class Gem : GameObject
    {
        private static readonly new Texture2D image = Helper.LoadImage("Images/Nether_Star-0");

        private static readonly float imgScaleFactor = 0.16f;
        private static readonly int width = (int)Math.Round(image.Width * imgScaleFactor);
        private static readonly int height = (int)Math.Round(image.Height * imgScaleFactor);

        private float sinX = Helper.GetRandomBetween(0, 2 * (float)Math.PI);
        private static float amplitude = 10;
        private static int framesPerPeriod = 60;

        private int baseLine;
        
        public Gem(int x, int y) : base(image, x, y, width, height)
        {
            baseLine = y + Main.CELL_SIDE_LENGTH / 2 - height / 2;

            TruePosition = new Vector2(x + Main.CELL_SIDE_LENGTH / 2 - width / 2, baseLine);
        }

        public override void Update()
        {
          /*  //x += framesPerPeriod / (2 * (float)Math.PI) % (2 * (float)Math.PI);
            sinX = (sinX  + framesPerPeriod / (2 * (float)Math.PI)) % ((float)Math.PI);
            Velocity.Y = baseLine + (float)Math.Sin(sinX) * amplitude - TruePosition.Y;*/
        }

        public override void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides)
        {
            otherGameObject.CollideWith(this, sides);
        }

        public override void CollideWith(Player player, IEnumerable<Side> sides)
        {
            InvokeDeleteReady();
        }
    }
}
