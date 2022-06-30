using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Rooms;
using Moon.Communication.Packets.Outgoing.Moderation;

namespace Moon.Communication.Packets.Incoming.Moderation
{
    class ModeratorActionEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().GetPermissions().HasRight("mod_caution"))
                return;

            if (!Session.GetHabbo().InRoom)
                return;

            Room CurrentRoom = Session.GetHabbo().CurrentRoom;
            if (CurrentRoom == null)
                return;

            int AlertMode = Packet.PopInt(); 
            string AlertMessage = Packet.PopString();
            bool IsCaution = AlertMode != 3;

            AlertMessage = IsCaution ? "Alerta del moderador:\n\n" + AlertMessage : "Mensaje por Moderador:\n\n" + AlertMessage;
            Session.GetHabbo().CurrentRoom.SendMessage(new BroadcastMessageAlertComposer(AlertMessage));
        }
    }
}
