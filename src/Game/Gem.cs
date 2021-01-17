﻿using Microsoft.Xna.Framework;
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

        float x = Helper.GetRandomBetween(0, 2 * (float)Math.PI);
        
        public Gem(int x, int y) : base(image, x, y, width, height)
        {
            TruePosition = new Vector2(x + MainGame.CELL_SIDE_LENGTH / 2 - width / 2, y + MainGame.CELL_SIDE_LENGTH / 2 - height / 2);
        }

        public override void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides)
        {
            otherGameObject.CollideWith(this, sides);
        }
    }
}
