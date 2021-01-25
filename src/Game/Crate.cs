using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    class Crate : GameObject
    {
        private static readonly Texture2D image = Helper.LoadImage("Images/Crate");

        private static float gravity = 1;

        private HashSet<Crate> cratesAbove = new HashSet<Crate>();
        private HashSet<Crate> cratesBelow = new HashSet<Crate>();

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

        public void Push(IEnumerable<Side> sides)
        {
            if (cratesAbove.Count != 0)
            {
                return;
            }

            if(sides.Contains(Side.Right))
            {
                InvokeMoveReady(new Vector2(3, 0));
            }
            else if (sides.Contains(Side.Left))
            {
                InvokeMoveReady(new Vector2(-3, 0));
            }

            Rectangle slightlyLowerBox = Box;
            slightlyLowerBox.Location += new Point(0, 1);

            foreach (Crate crateBelow in cratesBelow)
            {
                if (!slightlyLowerBox.Intersects(crateBelow.Box) || Box.Bottom > crateBelow.Box.Top) 
                {
                    crateBelow.StepOff(this);
                }
            }

        }

        public override void CollideWith(Crate crate, IEnumerable<Side> sides)
        {
            if (sides.Contains(Side.Bottom))
            {
                crate.StepOnto(this);
                cratesBelow.Add(crate);
            }
        }

        public void StepOnto(Crate crate) => cratesAbove.Add(crate);

        public void StepOff(Crate crate) => cratesAbove.Remove(crate);

        /*public override bool IsStandingStill()
        {
            return base.IsStandingStill();
        }*/
    }
}
