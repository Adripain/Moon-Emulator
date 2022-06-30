using System;
using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Users;
using Moon.Communication.Packets.Outgoing.Handshake;
using Moon.HabboHotel.Rooms;

namespace Moon.Communication.Packets.Incoming.Users
{
    class ScrGetUserInfoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
            Session.SendMessage(new UserRightsComposer(Session.GetHabbo()));

        }
    }
}
