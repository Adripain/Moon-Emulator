using System;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Pathfinding;

namespace Moon.HabboHotel.Astar
{
    public static class DreamPathfinder
    {
        private static SquarePoint GetClosetSqare(SquareInformation pInfo, HeightInfo Height)
        {
            double getDistance = pInfo.Point.GetDistance;
            SquarePoint point = pInfo.Point;
            double state = Height.GetState(pInfo.Point.X, pInfo.Point.Y);

            for (int i = 0; i < 8; i++)
            {
                SquarePoint point2 = pInfo.Pos(i);
                if ((point2.InUse && point2.CanWalk) && ((Height.GetState(point2.X, point2.Y) - state) <= 1.7))
                {
                    double num4 = point2.GetDistance;
                    if (getDistance > num4)
                    {
                        getDistance = num4;
                        point = point2;
                    }
                }
            }
            return point;
        }

        public static double GetDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow((double)(x1 - x2), 2.0) + Math.Pow((double)(y1 - y2), 2.0));
        }

        public static SquarePoint GetNextStep(RoomUser User, Vector2D From, Vector2D To, Gamemap mMap)
        {
            ModelInfo pMap = new ModelInfo(mMap.Model.MapSizeX, mMap.Model.MapSizeY, mMap.GameMap);
            SquarePoint pTarget = new SquarePoint(User, new Vector2D(To.X, To.Y), To.X, To.Y, pMap.GetState(To.X, To.Y), User.AllowOverride, mMap);
            if ((From.X == To.X) && (From.Y == To.Y))
            {
                return pTarget;
            }

            SquareInformation pInfo = new SquareInformation(User, From, pTarget, pMap, User.AllowOverride, mMap.DiagonalEnabled, mMap);
            return GetClosetSqare(pInfo, new HeightInfo(mMap.Model.MapSizeX, mMap.Model.MapSizeY, mMap.ItemHeightMap));
        }
    }
}