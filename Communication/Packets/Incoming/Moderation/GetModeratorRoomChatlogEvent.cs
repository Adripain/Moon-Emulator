using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Support;
using Moon.Communication.Packets.Outgoing.Moderation;

namespace Moon.Communication.Packets.Incoming.Moderation
{
    class GetModeratorRoomChatlogEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            int Junk = Packet.PopInt();

            Room Room = null;
            if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(Packet.PopInt(), out Room))
                return;

            try
            {
                Session.SendMessage(new ModeratorRoomChatlogComposer(Room));
            }
            catch { Session.SendNotification("Overflow :/"); }
        }
    }
}