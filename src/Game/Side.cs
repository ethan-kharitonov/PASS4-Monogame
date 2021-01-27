//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: Side.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: Stores the side enum and extention functions of the side enum 
namespace PASS4
{
    /// <summary>
    /// Used to indicate the side a collision happened
    /// </summary>
    public enum Side
    {
        Top,
        Left,
        Right,
        Bottom
    }

    /// <summary>
    /// Stores extention functions for the side enum
    /// </summary>
    public static class SideExtentions
    {
        /// <summary>
        /// Flips a given side
        /// </summary>
        /// <param name="side">The side to flip</param>
        /// <returns>The fliped side</returns>
        public static Side Flip(this Side side)
        {
            if (side == Side.Top)
            {
                return Side.Bottom;
            }

            if (side == Side.Bottom)
            {
                return Side.Top;
            }

            if (side == Side.Left)
            {
                return Side.Right;
            }

            return Side.Left;
        }
    }
}
