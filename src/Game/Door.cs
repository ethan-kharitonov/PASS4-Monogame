using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    class Door : GameObject
    {
        private static readonly Texture2D image = Helper.LoadImage("Images/door1");
        public Door(int x, int y) : base(image, x, y, MainGame.CELL_SIDE_LENGTH, MainGame.CELL_SIDE_LENGTH)
        {

        }
        public override void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides)
        {
            otherGameObject.CollideWith(this, sides);
        }
    }
}
