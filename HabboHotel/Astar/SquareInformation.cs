using System;
using System.Runtime.InteropServices;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Pathfinding;

namespace Moon.HabboHotel.Astar
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SquareInformation
    {
        private int mX;
        private int mY;
        private SquarePoint[] mPos;
        private SquarePoint mTarget;
        private SquarePoint mPoint;

        public SquareInformation(RoomUser User, Vector2D From, SquarePoint pTarget, ModelInfo pMap, bool pUserOverride, bool CalculateDiagonal, Gamemap mMap)
        {
            this.mX = From.X;
            this.mY = From.Y;
            this.mTarget = pTarget;
            this.mPoint = new SquarePoint(User, From, pTarget.X, pTarget.Y, pMap.GetState(From.X, From.Y), pUserOverride, mMap);
            this.mPos = new SquarePoint[8];
            if (CalculateDiagonal)
            {
                this.mPos[1] = new SquarePoint(User, new Vector2D(From.X - 1, From.Y - 1), pTarget.X, pTarget.Y, pMap.GetState(From.X - 1, From.Y - 1), pUserOverride, mMap);
                this.mPos[3] = new SquarePoint(User, new Vector2D(From.X - 1, From.Y + 1), pTarget.X, pTarget.Y, pMap.GetState(From.X - 1, From.Y + 1), pUserOverride, mMap);
                this.mPos[5] = new SquarePoint(User, new Vector2D(From.X + 1, From.Y + 1), pTarget.X, pTarget.Y, pMap.GetState(From.X + 1, From.Y + 1), pUserOverride, mMap);
                this.mPos[7] = new SquarePoint(User, new Vector2D(From.X + 1, From.Y - 1), pTarget.X, pTarget.Y, pMap.GetState(From.X + 1, From.Y - 1), pUserOverride, mMap);
            }
            this.mPos[0] = new SquarePoint(User, new Vector2D(From.X, From.Y - 1), pTarget.X, pTarget.Y, pMap.GetState(From.X, From.Y - 1), pUserOverride, mMap);
            this.mPos[2] = new SquarePoint(User, new Vector2D(From.X - 1, From.Y), pTarget.X, pTarget.Y, pMap.GetState(From.X - 1, From.Y), pUserOverride, mMap);
            this.mPos[4] = new SquarePoint(User, new Vector2D(From.X, From.Y + 1), pTarget.X, pTarget.Y, pMap.GetState(From.X, From.Y + 1), pUserOverride, mMap);
            this.mPos[6] = new SquarePoint(User, new Vector2D(From.X + 1, From.Y), pTarget.X, pTarget.Y, pMap.GetState(From.X + 1, From.Y), pUserOverride, mMap);
        }

        public SquarePoint Pos(int val)
        {
            return this.mPos[val];
        }

        public SquarePoint Point
        {
            get
            {
                return this.mPoint;
            }
        }
    }
}