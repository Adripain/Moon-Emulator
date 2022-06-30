using System;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Groups;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Handshake;

namespace Moon.Communication.Packets.Incoming.Handshake
{
    public class InfoRetrieveEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new UserObjectComposer(Session.GetHabbo()));
            Session.SendMessage(new UserPerksComposer(Session.GetHabbo()));
        }
    }
}