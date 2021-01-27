﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PASS4
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
            TruePosition = new Vector2(x + LevelContainer.CELL_SIDE_LENGTH / 2 - width / 2, y + LevelContainer.CELL_SIDE_LENGTH / 2 - height / 2);
        }

        public override void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides)
        {
            otherGameObject.CollideWith(this, sides);
        }
    }
}
