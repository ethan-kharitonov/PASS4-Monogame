using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PASS4
{
    class Spike : GameObject
    {
        private static readonly Texture2D image = Helper.LoadImage("Images/SpikeImg");
        public Spike(int x, int y) : base(image, x, y, LevelContainer.CELL_SIDE_LENGTH, LevelContainer.CELL_SIDE_LENGTH)
        {
        }

        public override void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides)
        {
            otherGameObject.CollideWith(this, sides);
        }
    }
}
