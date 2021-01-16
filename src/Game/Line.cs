using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PASS4
{
    public class Line
    {
        private Vector2 start;
        private Vector2 end;

        private bool isVertical = false;

        private float slope = 0;

        public Line(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;

            CalcSlope();
        }

        public Line()
        {

        }

        public Vector2 Start
        {
            get => start;

            set
            {
                start = value;
                CalcSlope();
            }
        }

        public Vector2 End
        {
            get => end;

            set
            {
                end = value;
                CalcSlope();
            }
        }

        public bool IsVertical => isVertical;

        public float Slope => slope;

        private void CalcSlope()
        {
            if (start.X == end.X)
            {
                isVertical = true;
                return;
            }

            slope = (end.Y - start.Y) / (end.X - start.X);
        }
    }
}
