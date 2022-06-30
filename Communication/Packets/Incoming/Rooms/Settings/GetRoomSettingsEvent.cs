using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.Communication.Packets.Outgoing.Rooms.Settings;

namespace Moon.Communication.Packets.Incoming.Rooms.Settings
{
    class GetRoomSettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Room = MoonEnvironment.GetGame().GetRoomManager().LoadRoom(Packet.PopInt());
            if (Room == null || !Room.CheckRights(Session, true))
                return;

            Session.SendMessage(new RoomSettingsDataComposer(Room));
        }
    }
}
