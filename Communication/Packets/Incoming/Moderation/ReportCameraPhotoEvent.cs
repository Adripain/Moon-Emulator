using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Support;
using Moon.HabboHotel.Rooms.Chat.Moderation;
using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.HabboHotel.Moderation;

namespace Moon.Communication.Packets.Incoming.Moderation
{
    class ReportCameraPhotoEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            //if (MoonEnvironment.GetGame().GetModerationManager().(Session.GetHabbo().Id))
            //{
            //    Session.SendMessage(new BroadcastMessageAlertComposer("You currently already have a pending ticket, please wait for a response from a moderator."));
            //    return;
            //}

            int photoId;

            if (!int.TryParse(Packet.PopString(), out photoId))
            {
                return;
            }

            int roomId = Packet.PopInt();
            int creatorId = Packet.PopInt();
            int categoryId = Packet.PopInt();

           // MoonEnvironment.GetGame().GetModerationTool().SendNewTicket(Session, categoryId, creatorId, "", new List<string>(), (int) ModerationSupportTicketType.PHOTO, photoId);
            MoonEnvironment.GetGame().GetClientManager().ModAlert("A new support ticket has been submitted!");
        }
    }
}