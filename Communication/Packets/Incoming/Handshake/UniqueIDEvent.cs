using System;
using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Handshake;

namespace Moon.Communication.Packets.Incoming.Handshake
{
    public class UniqueIDEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Junk = Packet.PopString();
            string MachineId = Packet.PopString();

            Session.MachineId = MachineId;

            Session.SendMessage(new SetUniqueIdComposer(MachineId));
        }
    }
}