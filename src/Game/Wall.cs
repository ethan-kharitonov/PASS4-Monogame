using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    class Wall : GameObject
    {
        private static readonly Texture2D image = Helper.LoadImage("Images/Wall");

        public Wall(int x, int y) : base(image, x, y, GameView.CELL_SIDE_LENGTH, GameView.CELL_SIDE_LENGTH)
        {
        }

        public Wall(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        public override void InformCollisionTo(GameObject otherGameObject, IEnumerable<Side> sides)
        {
            otherGameObject.CollideWith(this, sides);
        }
    }
}
