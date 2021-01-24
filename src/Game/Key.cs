using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    class Key : GameObject
    {
        private static readonly Texture2D image = Helper.LoadImage("Images/Key_01");

        private static readonly float imgScaleFactor = 0.35f;
        private static readonly int width = (int)Math.Round(image.Width * imgScaleFactor);
        private static readonly int height = (int)Math.Round(image.Height * imgScaleFactor);

        public Key(int x, int y) : base(image, x + MainGame.CELL_SIDE_LENGTH/2 - width/2, y + MainGame.CELL_SIDE_LENGTH/2 - height/2, width, height)
        {
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
