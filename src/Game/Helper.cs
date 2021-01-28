//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: Helper.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: A static class containing a bunch of usefull function
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PASS4
{
    public static class Helper
    {
        //Variable used to generate random numbers
        public static readonly Random rnd = new Random();

        //Variables used for drawing to the screen
        public static ContentManager Content;
        public static SpriteBatch SpriteBatch;
        public static GraphicsDeviceManager graphics;

        //The most commonly used font (for input)
        public static SpriteFont InputFont;

        private static Texture2D rect;
        private static Color[] data;

        //Variables used for ray box collision (global so they dont get erased and recreated every frame)
        private static RayCollisionInfo[] results = new RayCollisionInfo[4];
        private static (Vector2 intersection, bool isIntersecting) lineLineCollisionResult;

        //The keys pressed last frame and the keys released this frame
        private static List<Keys> keysPressedLastFrame = new List<Keys>();
        public static List<Keys> KeysReleasedThisFrame { get; private set; } = new List<Keys>();

        /// <summary>
        /// Appends all of this frames input onto a string
        /// </summary>
        /// <param name="text">The string to add onto</param>
        /// <returns>The new updated string</returns>
        public static string UpdateStringWithInput(string text)
        {
            //Add all the keys released this frame
            foreach (Keys key in KeysReleasedThisFrame)
            {
                //Simply add it if it is one charachter or a digit between 0 and 10
                if (key.ToString().Length == 1)
                {
                    text += key.ToString();
                }
                else
                {
                    //Try converting to an int and adding
                    try
                    {
                        int keyNumber = Convert.ToInt32(key.ToString()[1]) - '0';
                        text += 0 < keyNumber && keyNumber < 10 ? keyNumber.ToString() : string.Empty;
                    }
                    catch (Exception)
                    {
                    }
                }
               
                //Proparly handle special keys like plus, minus and backspace
                if (key == Keys.OemPlus)
                {
                    text += '+';
                }
                else if (key == Keys.OemMinus)
                {
                    text += '-';
                }
                else if (key == Keys.Back)
                {
                    //Remove the last letter of strings with more than two charachters or empty the string
                    text = text.Length <= 1 ? string.Empty : text.Substring(0, text.Length - 1);
                }
            }

            //Return the updated string
            return text;
        }

        /// <summary>
        /// Limit a string to a certain length
        /// </summary>
        /// <param name="text">The string</param>
        /// <param name="maxChars">The length</param>
        /// <returns></returns>
        public static string TrimString(string text, int maxChars) 
            => text.Length <= maxChars ? text : text.Substring(0, maxChars);

        /// <summary>
        /// Update the keys pressed last frame and keys released this frame variables
        /// </summary>
        public static void UpdateKeyBoard()
        {
            //Set keys released this frame to keys that were pressed last frame but not pressed this frame
            List<Keys> keysPressedThisFrame = Keyboard.GetState().GetPressedKeys().ToList();
            KeysReleasedThisFrame = keysPressedLastFrame.Where(k => !keysPressedThisFrame.Contains(k)).ToList();

            //Update the keys pressed last frame
            keysPressedLastFrame = keysPressedThisFrame;
        }

        /// <summary>
        /// Loads an image from a given file path
        /// </summary>
        /// <param name="path">The filepath of the image</param>
        /// <returns>The loaded image</returns>
        public static Texture2D LoadImage(string path)
        {
            return Content.Load<Texture2D>(path);
        }


        /// <summary>
        /// Checks if some line is intersecting a vertical line (if both end points are on the vertical line, prioritise start)
        /// </summary>
        /// <param name="line">Any line</param>
        /// <param name="verticalLine">Vertical line</param>
        /// <returns>The point of intersection and true if they intersect, return (0, 0) and false otherwise</returns>
        public static (Vector2 intersection, bool isIntersecting) LineIntersectWithVerticalLine(Line line, Line verticalLine)
        {
            //Throw an exception if the vertical line is not vertical
            if (!verticalLine.IsVertical)
            {
                throw new ArgumentException("The second line must be vertical");
            }

            //Tests incase the regular line is also vertical
            if (line.IsVertical)
            {
                //Check for trivial non intersection
                if (line.Start.X != verticalLine.Start.X)
                {
                    return (Vector2.Zero, false);
                }

                //Check if start is on the vertical line
                if (IsPointOnLine(line.Start, verticalLine))
                {
                    return (line.Start, true);
                }

                bool vStartOnLine = IsPointOnLine(verticalLine.Start, verticalLine);
                bool vEndOnLine = IsPointOnLine(verticalLine.End, verticalLine);

                if (vStartOnLine && vEndOnLine)
                {
                    return (Math.Abs(verticalLine.Start.Y - line.Start.Y) < Math.Abs(verticalLine.End.Y - line.Start.Y) ? verticalLine.Start : verticalLine.End, true);
                }

                if (vStartOnLine)
                {
                    return (verticalLine.Start, true);
                }

                if (vEndOnLine)
                {
                    return (verticalLine.End, true);
                }

                return (Vector2.Zero, false);
            }

            Vector2 intersection = new Vector2(verticalLine.Start.X, line.Start.Y + (verticalLine.Start.X - line.Start.X) * line.Slope);

            return IsPointOnLine(intersection, line) && IsPointOnLine(intersection, verticalLine) ? (intersection, true) : (Vector2.Zero, false);
        }

        /// <summary>
        /// Checks if regular line intersects with a hoizontal line
        /// </summary>
        /// <param name="line">The regular line</param>
        /// <param name="horizontalLine">The horizontal line</param>
        /// <returns>The point of intersection and true if they intersect, return (0, 0) and false otherwise</returns>
        /// <see cref="LineIntersectWithVerticalLine(Line, Line)"/>
        public static (Vector2 intersection, bool isIntersecting) LineIntersectsWithHorizontal(Line line, Line horizontalLine)
        {
            //Throw an exception if the horizontal line is not horizontal
            if (!horizontalLine.IsVertical && horizontalLine.Slope != 0)
            {
                throw new ArgumentException("Second line must be horizontal");
            }

            //Rotate both lines by 90 degrees (horizontal --> vertical)
            Line rotatedLine = RotateLine90DegreesCounterClockwise(line);
            Line rotatedHorizontalLine = RotateLine90DegreesCounterClockwise(horizontalLine);

            //Check if the regular line intersects with the now vertical line
            (Vector2 intersection, bool isIntersecting) result = LineIntersectWithVerticalLine(rotatedLine, rotatedHorizontalLine);

            //Rotate the intersection point back 90 degrees and return the results
            return (Rotate90DegreesClockwise(result.intersection), result.isIntersecting);
        }

        /// <summary>
        /// Finds information about the collision between a ray and a rectangle.
        /// Ignore collisions that lie on the rectangle
        /// </summary>
        /// <param name="ray">A line (but treated like a lazer beam - cut after collision)</param>
        /// <param name="box">The box</param>
        /// <returns>Collision information</returns>
        public static RayCollisionInfo RayBoxFirstCollision(Line ray, Rectangle box)
        {
            //Checks if the ray intersects with all four sides of the rectangle
            lineLineCollisionResult = LineIntersectsWithHorizontal(ray, new Line(new Vector2(box.Left, box.Top), new Vector2(box.Right, box.Top)));
            results[0] = new RayCollisionInfo(lineLineCollisionResult.intersection, lineLineCollisionResult.intersection - ray.Start, Side.Top, lineLineCollisionResult.isIntersecting);

            lineLineCollisionResult = LineIntersectsWithHorizontal(ray, new Line(new Vector2(box.Left, box.Bottom), new Vector2(box.Right, box.Bottom)));
            results[1] = new RayCollisionInfo(lineLineCollisionResult.intersection, lineLineCollisionResult.intersection - ray.Start, Side.Bottom, lineLineCollisionResult.isIntersecting);

            lineLineCollisionResult = LineIntersectWithVerticalLine(ray, new Line(new Vector2(box.Right, box.Top), new Vector2(box.Right, box.Bottom)));
            results[2] = new RayCollisionInfo(lineLineCollisionResult.intersection, lineLineCollisionResult.intersection - ray.Start, Side.Right, lineLineCollisionResult.isIntersecting);

            lineLineCollisionResult = LineIntersectWithVerticalLine(ray, new Line(new Vector2(box.Left, box.Top), new Vector2(box.Left, box.Bottom)));
            results[3] = new RayCollisionInfo(lineLineCollisionResult.intersection, lineLineCollisionResult.intersection - ray.Start, Side.Left, lineLineCollisionResult.isIntersecting);

            //Get the information about collided sides
            List<RayCollisionInfo> collidedRays = results.Where(c => c.IsIntersecting).ToList();

            
            if (collidedRays.Count == 1)
            {
                //If that the line doesnt lie on the rectangle return its collision info, otherwise return false
                if (IsPointInsideRectangle(ray.Start, box) || IsPointInsideRectangle(ray.End, box))
                {
                    return collidedRays[0];
                }

                return new RayCollisionInfo(false);
            }

            if (collidedRays.Count == 2)
            {
                //If it collided on a corner make sure one side of the ray is on the corner
                if (collidedRays[0].Distance.Length() == collidedRays[1].Distance.Length())
                {
                    if (!IsPointInsideRectangle(ray.Start, box) && !IsPointInsideRectangle(ray.End, box))
                    {
                        return new RayCollisionInfo(false);
                    }

                    //return one of the collision infos with the sides of both collisions
                    collidedRays[0].Sides.Add(collidedRays[1].Sides[0]);
                    return collidedRays[0];
                }


                //Discard lines that cover an entire side
                if ((ray.IsVertical && (ray.Start.X == box.Left || ray.Start.X == box.Right)) ||
                    ray.Slope == 0 && (ray.Start.Y == box.Top || ray.Start.Y == box.Bottom))
                {
                    return new RayCollisionInfo(false);
                }

                //return the shortest distance collision
                return collidedRays[0].Distance.Length() < collidedRays[1].Distance.Length() ? collidedRays[0] : collidedRays[1];
            }

            //Return no collision if no sides collided
            return new RayCollisionInfo(false);
        }

        /// <summary>
        /// Rotates a line 90 degrees around (0, 0) by rotating both its points by 90 degrees
        /// </summary>
        /// <param name="line">The line to rotate</param>
        /// <returns>The rotated line</returns>
        public static Line RotateLine90DegreesCounterClockwise(Line line)
            => new Line(Rotate90DegreesCounterClockwise(line.Start), Rotate90DegreesCounterClockwise(line.End));

        /// <summary>
        /// Checks if a point is on a line
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="line">The line</param>
        /// <returns>True if point is on the line, false otherwise</returns>
        public static bool IsPointOnLine(Vector2 point, Line line)
            => IsBetween(line.Start.X, point.X, line.End.X) && IsBetween(line.Start.Y, point.Y, line.End.Y);

        /// <summary>
        /// Checks if a given value is between two other values
        /// </summary>
        /// <param name="a">A boundry</param>
        /// <param name="value">The value in the middle</param>
        /// <param name="b">A boundry</param>
        /// <returns>True if it is between, false otherwise</returns>
        public static bool IsBetween(float a, float value, float b)
            => Math.Min(a, b) <= value && value <= Math.Max(a, b);

        /// <summary>
        /// Rotates a point 90 degrees around (0, 0)
        /// </summary>
        /// <param name="point">The point to rotate</param>
        /// <returns>The rotated point</returns>
        public static Vector2 Rotate90DegreesCounterClockwise(Vector2 point) => new Vector2(-point.Y, point.X);

        /// <summary>
        /// Rotates a point a point -90 degrees around (0, 0)
        /// </summary>
        /// <param name="point">The point to rotate</param>
        /// <returns>The rotated point</returns>
        public static Vector2 Rotate90DegreesClockwise(Vector2 point) => new Vector2(point.Y, -point.X);

        /// <summary>
        /// Checks if a point is inside (not on) the rectangle
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="box">The rectangle</param>
        /// <returns>True if the point is inside, false otherwise</returns>
        public static bool IsPointInsideRectangle(Vector2 point, Rectangle box)
            => box.Left < point.X && point.X < box.Right && box.Top < point.Y && point.Y < box.Bottom;

        /// <summary>
        /// Yields the four vertecies of a rectangle
        /// </summary>
        /// <param name="box">The rectangle</param>
        /// <returns>An Ienumerable containing his vertacies</returns>
        public static IEnumerable<Vector2> GetVertecies(Rectangle box)
        {
            //Alternate adding the rectangles width and height to his position
            for (int c = 0; c < 2; ++c)
            {
                for (int r = 0; r < 2; ++r)
                {
                    //Calculate and yield the vertex
                    yield return new Vector2(box.Location.X + c * box.Width, box.Location.Y + r * box.Height);
                }
            }
        }

        /// <summary>
        /// Checks ig a given point is inside or on a given rectanle
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="box">The rectangle</param>
        /// <returns>True if it is inside or on the rectangle, false otherwise</returns>
        public static bool IsPointInOrOnRectangle(Vector2 point, Rectangle box)
            => IsBetween(box.Left, point.X, box.Right) && IsBetween(box.Top, point.Y, box.Bottom);

        /// <summary>
        /// Clamps a value between two other numbers
        /// </summary>
        /// <param name="min">A boundry</param>
        /// <param name="value">The middle value</param>
        /// <param name="max">A boundry</param>
        /// <returns>The clamped value</returns>
        public static float Clamp(float min, float value, float max)
            => Math.Max(Math.Min(min, max), Math.Min(value, Math.Max(min, max)));

        public static string[] MergeSort(string[] items, Func<string, string> Filter) => Divide(items, 0, items.Length - 1, Filter);

        private static string[] Divide(string[] items, int left, int right, Func<string, string> Filter)
        {
            if (left < right)
            {
                int mid = (left + right) / 2;
                return Sort(Divide(items, left, mid, Filter), Divide(items, mid + 1, right, Filter), Filter);
            }
            return new[] { items[left] };
        }

        private static int CompareTwoStrigns(string item1, string item2)
        {
            try
            {
                int num1 = Convert.ToInt32(item1);
                int num2 = Convert.ToInt32(item2);

                return num1 < num2 ? -1 : 1;
            }
            catch (Exception)
            {
                return item1.CompareTo(item2);
            }
        }
        private static string[] Sort(string[] items1, string[] items2, Func<string, string> Filter)
        {
            int index1 = 0;
            int index2 = 0;

            string[] sortedNumbers = new string[items1.Length + items2.Length];

            for (int i = 0; i < sortedNumbers.Length; ++i)
            {
                if (index2 >= items2.Length)
                {
                    sortedNumbers[i] = items1[index1];
                    ++index1;
                }
                else if (index1 >= items1.Length)
                {
                    sortedNumbers[i] = items2[index2];
                    ++index2;
                }
                else if (CompareTwoStrigns(Filter(items1[index1]), Filter(items2[index2])) == -1)
                {
                    sortedNumbers[i] = items1[index1];
                    ++index1;
                }
                else
                {
                    sortedNumbers[i] = items2[index2];
                    ++index2;
                }
            }

            return sortedNumbers;
        }
        
    }
}
