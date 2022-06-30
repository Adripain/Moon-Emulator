using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Games;
using Moon.Communication.Packets.Outgoing.GameCenter;
using System.Data;
using System.Globalization;

using Moon.HabboHotel.Users;
using Moon.HabboHotel.GameClients;

namespace Moon.Communication.Packets.Incoming.GameCenter
{
    class Game2GetWeeklyLeaderboardEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int GameId = Packet.PopInt();
            int weekNum = new GregorianCalendar(GregorianCalendarTypes.Localized).GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            int lastWeekNum = 0;

            if(weekNum == 1) { lastWeekNum = 52; } else { lastWeekNum = weekNum - 1; }

            GameData GameData = null;


            if (MoonEnvironment.GetGame().GetGameDataManager().TryGetGame(GameId, out GameData))
            {

            }
        }
    }
}
