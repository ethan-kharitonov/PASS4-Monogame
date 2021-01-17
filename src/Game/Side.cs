using System;

namespace Game
{
    [Flags]
    public enum Side
    {
        Top = 1,
        Left = 2,
        Right = 4,
        Bottom = 8
    }

    public static class SideExtentions
    {
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
