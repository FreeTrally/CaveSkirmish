using System;
using System.Drawing;

namespace CaveSkirmish
{
    public static class Calculations
    {
        public static int GetDistance(Point first, Point second)
        {
            return GetDistanceX(first, second) + GetDistanceY(first, second);
        }

        public static int GetDistanceX(Point first, Point second)
        {
            return Math.Abs(first.X - second.X);
        }

        public static int GetDistanceY(Point first, Point second)
        {
            return Math.Abs(first.Y - second.Y);
        }
    }
}
