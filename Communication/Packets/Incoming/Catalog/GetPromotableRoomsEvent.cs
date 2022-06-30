using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Catalog;

namespace Moon.Communication.Packets.Incoming.Catalog
{
    class GetPromotableRoomsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            List<RoomData> Rooms = Session.GetHabbo().UsersRooms;
            Rooms = Rooms.Where(x => (x.Promotion == null || x.Promotion.TimestampExpires < MoonEnvironment.GetUnixTimestamp())).ToList();
            Session.SendMessage(new PromotableRoomsComposer(Rooms));
        }
    }
}
