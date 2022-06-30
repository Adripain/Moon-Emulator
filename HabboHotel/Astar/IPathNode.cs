using System;

namespace Moon.HabboHotel.Astar
{
    public interface IPathNode
    {
        bool IsBlocked(int x, int y, bool lastTile);
    }
}