using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    class Gem : GameObject
    {
        private static readonly Texture2D image = Helper.LoadImage("Images/Nether_Star-0");

        private static readonly float imgScaleFactor = 0.16f;
        private static readonly int width = (int)Math.Round(image.Width * imgScaleFactor);
        private static readonly int height = (int)Math.Round(image.Height * imgScaleFactor);

        
        public Gem(int x, int y) : base(image, x, y, width, height)
        {
            isCollidable = false;
            TruePosition = new Vector2(x + MainGame.CELL_SIDE_LENGTH / 2 - width / 2, y + MainGame.CELL_SIDE_LENGTH / 2 - height / 2);
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
    }
}
