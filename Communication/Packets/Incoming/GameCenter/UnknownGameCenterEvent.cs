using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Games;
using Moon.Communication.Packets.Outgoing.GameCenter;
using System.Data;

using Moon.HabboHotel.Users;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.Communication.Packets.Incoming.GameCenter
{
    class UnknownGameCenterEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int GameId = Packet.PopInt();
            int UserId = Packet.PopInt();

            GameData GameData = null;
            if (MoonEnvironment.GetGame().GetGameDataManager().TryGetGame(GameId, out GameData))
            {
               // Session.SendMessage(new Game2WeeklyLeaderboardComposer(GameId)); Comentado y funciona
            }
        }
    }
}
