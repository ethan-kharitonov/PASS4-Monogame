using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    class Wall : GameObject
    {
        private static readonly new Texture2D image = Helper.LoadImage("Images/Wall");

        public Wall(int x, int y) : base(image, x, y, MainGame.CELL_SIDE_LENGTH, MainGame.CELL_SIDE_LENGTH)
        {

        }

        public override void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides)
        {
            otherGameObject.CollideWith(this, sides);
        }
    }
}
