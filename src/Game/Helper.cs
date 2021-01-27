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
        public static readonly Random rnd = new Random();

        public static ContentManager Content;
        public static SpriteBatch SpriteBatch;
        public static GraphicsDeviceManager graphics;

        public static SpriteFont InputFont;

        private static Texture2D rect;
        private static Color[] data;

        private static RayCollisionInfo[] results = new RayCollisionInfo[4];
        private static (Vector2 intersection, bool isIntersecting) lineLineCollisionResult;

        private static List<Keys> keysPressedLastFrame = new List<Keys>();
        private static List<Keys> keysReleasedThisFrame = new List<Keys>();

        public static List<Keys> KeysReleasedThisFrame => keysReleasedThisFrame;

        public static string UpdateStringWithInput(string text, Predicate<Keys> predicate)
        {
            foreach (Keys key in KeysReleasedThisFrame)
            {
                if (key.ToString().Length == 1 && predicate(key))
                {
                    text += key.ToString();
                }

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
                    text = text.Length <= 1 ? string.Empty : text.Substring(0, text.Length - 1);
                }
            }


            return text;
        }

        public static string UpdateStringWithInput(string text) 
            => UpdateStringWithInput(text, k => true);

        public static string TrimString(string text, int maxChars) 
            => text.Length <= maxChars ? text : text.Substring(0, maxChars);
        public static void UpdateKeyBoard()
        {
            List<Keys> keysPressedThisFrame = Keyboard.GetState().GetPressedKeys().ToList();
            if (keysPressedThisFrame.Count() != 0)
            {

            }
            keysReleasedThisFrame = keysPressedLastFrame.Where(k => !keysPressedThisFrame.Contains(k)).ToList();

            keysPressedLastFrame = keysPressedThisFrame;
        }

        public static float GetRandomBetween(float a, float b)
            => (float)(Math.Min(a, b) + rnd.NextDouble() * Math.Abs(a - b));

        public static Texture2D LoadImage(string path)
        {
            return Content.Load<Texture2D>(path);
        }

        public static (Vector2 intersection, bool isIntersecting) LineIntersectWithVerticalLine(Line line, Line verticalLine)
        {
            if (!verticalLine.IsVertical)
            {
                throw new ArgumentException("The second line must be vertical");
            }

            if (line.IsVertical)
            {
                if (line.Start.X != verticalLine.Start.X)
                {
                    return (Vector2.Zero, false);
                }

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

        public static (Vector2 intersection, bool isIntersecting) LineIntersectsWithHorizontal(Line line, Line horizontalLine)
        {
            if (!horizontalLine.IsVertical && horizontalLine.Slope != 0)
            {
                throw new ArgumentException("Second line must be horizontal");
            }

            Line rotatedLine = RotateLine90DegreesCounterClockwise(line);
            Line rotatedHorizontalLine = RotateLine90DegreesCounterClockwise(horizontalLine);

            (Vector2 intersection, bool isIntersecting) result = LineIntersectWithVerticalLine(rotatedLine, rotatedHorizontalLine);

            return (Rotate90DegreesClockwise(result.intersection), result.isIntersecting);

        }

        public static RayCollisionInfo RayBoxFirstCollision(Line ray, Rectangle box)
        {
            lineLineCollisionResult = LineIntersectsWithHorizontal(ray, new Line(new Vector2(box.Left, box.Top), new Vector2(box.Right, box.Top)));
            results[0] = new RayCollisionInfo(lineLineCollisionResult.intersection, lineLineCollisionResult.intersection - ray.Start, Side.Top, lineLineCollisionResult.isIntersecting);

            lineLineCollisionResult = LineIntersectsWithHorizontal(ray, new Line(new Vector2(box.Left, box.Bottom), new Vector2(box.Right, box.Bottom)));
            results[1] = new RayCollisionInfo(lineLineCollisionResult.intersection, lineLineCollisionResult.intersection - ray.Start, Side.Bottom, lineLineCollisionResult.isIntersecting);

            lineLineCollisionResult = LineIntersectWithVerticalLine(ray, new Line(new Vector2(box.Right, box.Top), new Vector2(box.Right, box.Bottom)));
            results[2] = new RayCollisionInfo(lineLineCollisionResult.intersection, lineLineCollisionResult.intersection - ray.Start, Side.Right, lineLineCollisionResult.isIntersecting);

            lineLineCollisionResult = LineIntersectWithVerticalLine(ray, new Line(new Vector2(box.Left, box.Top), new Vector2(box.Left, box.Bottom)));
            results[3] = new RayCollisionInfo(lineLineCollisionResult.intersection, lineLineCollisionResult.intersection - ray.Start, Side.Left, lineLineCollisionResult.isIntersecting);

            List<RayCollisionInfo> collidedRays = results.Where(c => c.IsIntersecting).ToList();

            if (collidedRays.Count == 1)
            {
                if (IsPointInsideRectangle(ray.Start, box) || IsPointInsideRectangle(ray.End, box))
                {
                    return collidedRays[0];
                }

                return new RayCollisionInfo(false);
            }

            if (collidedRays.Count == 2)
            {
                if (collidedRays[0].Distance.Length() == collidedRays[1].Distance.Length())
                {
                    if (!IsPointInsideRectangle(ray.Start, box) && !IsPointInsideRectangle(ray.End, box))
                    {
                        return new RayCollisionInfo(false);
                    }

                    collidedRays[0].Sides.Add(collidedRays[1].Sides[0]);
                    return collidedRays[0];
                }


                if ((ray.IsVertical && (ray.Start.X == box.Left || ray.Start.X == box.Right)) ||
                    ray.Slope == 0 && (ray.Start.Y == box.Top || ray.Start.Y == box.Bottom))
                {
                    return new RayCollisionInfo(false);
                }

                return collidedRays[0].Distance.Length() < collidedRays[1].Distance.Length() ? collidedRays[0] : collidedRays[1];
            }

            return new RayCollisionInfo(false);
        }

        public static Line RotateLine90DegreesCounterClockwise(Line line)
            => new Line(Rotate90DegreesCounterClockwise(line.Start), Rotate90DegreesCounterClockwise(line.End));

        public static bool IsPointOnLine(Vector2 point, Line line)
            => IsBetween(line.Start.X, point.X, line.End.X) && IsBetween(line.Start.Y, point.Y, line.End.Y);

        public static bool IsBetween(float a, float value, float b)
            => Math.Min(a, b) <= value && value <= Math.Max(a, b);

        public static Vector2 Rotate90DegreesCounterClockwise(Vector2 point) => new Vector2(-point.Y, point.X);

        public static Vector2 Rotate90DegreesClockwise(Vector2 point) => new Vector2(point.Y, -point.X);

        public static bool IsPointInsideRectangle(Vector2 point, Rectangle box)
            => box.Left < point.X && point.X < box.Right && box.Top < point.Y && point.Y < box.Bottom;

        public static Texture2D GetRectTexture(int width, int height, Color color)
        {
            rect = new Texture2D(graphics.GraphicsDevice, width, height);
            data = new Color[width * height];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = color;
            }

            rect.SetData(data);

            return rect;
        }

        public static IEnumerable<Vector2> GetVertecies(Rectangle box)
        {
            for (int c = 0; c < 2; ++c)
            {
                for (int r = 0; r < 2; ++r)
                {
                    yield return new Vector2(box.Location.X + c * box.Width, box.Location.Y + r * box.Height);
                }
            }
        }

        public static bool IsPointInOrOnRectangle(Vector2 point, Rectangle box)
            => IsBetween(box.Left, point.X, box.Right) && IsBetween(box.Top, point.Y, box.Bottom);

        public static float Clamp(float min, float value, float max)
            => Math.Max(Math.Min(min, max), Math.Min(value, Math.Max(min, max)));

    }
}
