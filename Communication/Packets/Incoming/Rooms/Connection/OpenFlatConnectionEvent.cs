using System;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing;
using Moon.Communication.Packets.Outgoing.Nux;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.Communication.Packets.Incoming.Rooms.Connection
{
    public class OpenFlatConnectionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            int RoomId = Packet.PopInt();
            string Password = Packet.PopString();

            if (Session.GetHabbo().Rank > 3 && !Session.GetHabbo().StaffOk)
                Session.SendMessage(new RoomCustomizedAlertComposer("No te has autentificado como Staff del hotel."));

            Session.GetHabbo().PrepareRoom(RoomId, Password);
            
        }
    }
}