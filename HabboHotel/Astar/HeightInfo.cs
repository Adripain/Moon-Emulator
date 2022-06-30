using System;
using System.Runtime.InteropServices;

namespace Moon.HabboHotel.Astar
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HeightInfo
    {
        private double[,] mMap;
        private int mMaxX;
        private int mMaxY;

        public HeightInfo(int MaxX, int MaxY, double[,] Map)
        {
            this.mMap = Map;
            this.mMaxX = MaxX;
            this.mMaxY = MaxY;
        }

        public double GetState(int x, int y)
        {
            if ((x >= this.mMaxX) || (x < 0))
            {
                return 0.0;
            }
            if ((y >= this.mMaxY) || (y < 0))
            {
                return 0.0;
            }
            return this.mMap[x, y];
        }
    }
}