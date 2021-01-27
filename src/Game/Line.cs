//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: Line.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: Represents a geometrical line with all its properties
using Microsoft.Xna.Framework;

namespace PASS4
{
    public class Line
    {
        //The endpoints of the line
        private Vector2 start;
        private Vector2 end;


        /// <summary>
        /// Crates a new line with specific end points and calculates its slope
        /// </summary>
        /// <param name="start">The starting point of the line</param>
        /// <param name="end">The ending point of the line</param>
        public Line(Vector2 start, Vector2 end)
        {
            //Stores endpoints
            this.start = start;
            this.end = end;

            //Calculates slope
            CalcSlope();
        }

        /// <summary>
        /// Start point property which does all the nessecary calculations when start point is chaged
        /// </summary>
        public Vector2 Start
        {
            get => start;

            set
            {
                //calculates the new slope
                start = value;
                CalcSlope();
            }
        }

        /// <summary>
        /// End point property which does all the nessecary calculations when end point is chaged
        /// </summary>
        public Vector2 End
        {
            get => end;

            set
            {
                //calculates the new slope
                end = value;
                CalcSlope();
            }
        }

        /// <summary>
        /// True if the line is vertical, false otherwise. false by default.
        /// </summary>
        public bool IsVertical { get; private set; } = false;

        /// <summary>
        /// The slope of the line. zero when vertical.
        /// </summary>
        public float Slope { get; private set; } = 0;

        /// <summary>
        /// Calculates the slope of the line
        /// </summary>
        private void CalcSlope()
        {
            //Checks if the line is vertica;
            if (start.X == end.X)
            {
                //Indicates it is vertical and leaves
                IsVertical = true;
                return;
            }

            //Calculates and stores the slope
            Slope = (end.Y - start.Y) / (end.X - start.X);
        }
    }
}
