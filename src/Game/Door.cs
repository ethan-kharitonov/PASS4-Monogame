using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PASS4
{
    class Door : GameObject
    {
        private static readonly Texture2D image = Helper.LoadImage("Images/door1");
        public Door(int x, int y) : base(image, x, y, GameView.CELL_SIDE_LENGTH, GameView.CELL_SIDE_LENGTH)
        {

        }
        public override void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides)
        {
            otherGameObject.CollideWith(this, sides);
        }

        public void WalkThrough() => InvokeDeleteReady();
    }
}
