﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PASS4
{
    class Crate : GameObject
    {
        private static readonly new Texture2D image = Helper.LoadImage("Images/Crate");

        private static float gravity = 1;

        public Crate(int x, int y) : base(image, x, y, MainGame.CELL_SIDE_LENGTH, MainGame.CELL_SIDE_LENGTH)
        {

        }

        public override void Update()
        {
            Velocity.Y += gravity;
        }

        public override void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides)
        {
            otherGameObject.CollideWith(this, sides);
        }

        public override void CollideWith(Player player, IEnumerable<Side> sides)
        {
            if(sides.Contains(Side.Left))
            {
                InvokeMoveReady(new Vector2(1, 0));
            }
            else if (sides.Contains(Side.Right))
            {
                InvokeMoveReady(new Vector2(-1, 0));
            }

        }
    }
}
