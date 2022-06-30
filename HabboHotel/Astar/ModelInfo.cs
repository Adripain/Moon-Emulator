using System;
using System.Runtime.InteropServices;

namespace Moon.HabboHotel.Astar
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ModelInfo
    {
        private byte[,] mMap;
        private int mMaxX;
        private int mMaxY;
        internal ModelInfo(int MaxX, int MaxY, byte[,] Map)
        {
            this.mMap = Map;
            this.mMaxX = MaxX;
            this.mMaxY = MaxY;
        }

        public byte GetState(int x, int y)
        {
            if ((x >= this.mMaxX) || (x < 0))
            {
                return 0;
            }
            if ((y >= this.mMaxY) || (y < 0))
            {
                return 0;
            }
            return this.mMap[x, y];
        }
    }
}