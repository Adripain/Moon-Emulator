using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Utilities;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Messenger;
using Moon.Database.Interfaces;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.Communication.Packets.Incoming.Messenger
{
    class SendRoomInviteEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().TimeMuted > 0)
            {
                Session.SendNotification("Oops, estas muteado - No puedes enviar invitaciones a salas");
                return;
            }

            int Amount = Packet.PopInt();
            if (Amount > 500)
                return; // don't send at all

            List<int> Targets = new List<int>();
            for (int i = 0; i < Amount; i++)
            {
                int uid = Packet.PopInt();
                if (i < 100) // limit to 100 people, keep looping until we fulfil the request though
                {
                    Targets.Add(uid);
                }
            }

            string Message = StringCharFilter.Escape(Packet.PopString());
            if (Message.Length > 121)
                Message = Message.Substring(0, 121);

            if (Message.Contains("&#1Âº;") || Message.Contains("&#1Âº") || Message.Contains("&#"))
            { Session.SendMessage(new MassEventComposer("habbopages/spammer.txt")); return; }

            string word;

            if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override") &&
                MoonEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Message, out word))
            {
                Session.GetHabbo().BannedPhraseCount++;
                if (Session.GetHabbo().BannedPhraseCount >= 1)
                {
                    Session.GetHabbo().TimeMuted = 25;
                    Session.SendNotification("¡Has sido silenciad@ mientras un moderador revisa tu caso, al parecer nombraste un hotel! Aviso " + Session.GetHabbo().BannedPhraseCount + "/3");
                    MoonEnvironment.GetGame().GetClientManager().StaffAlert1(new RoomInviteComposer(int.MinValue, "Spammer: " + Session.GetHabbo().Username + " / Frase: " + Message + " / Palabra: " + word.ToUpper() + " / Fase: " + Session.GetHabbo().BannedPhraseCount + " / 10."));
                    MoonEnvironment.GetGame().GetClientManager().StaffAlert2(new RoomNotificationComposer("Alerta de publicista:",
                    "<b><font color=\"#B40404\">Por favor, recuerda investigar bien antes de recurrir a una sanción.</font></b><br><br>Palabra: <b>" + word.ToUpper() + "</b>.<br><br><b>Frase:</b><br><i>" + Message +
                    "</i>.<br><br><b>Tipo:</b><br>Chat de sala.\r\n" + "<b>Usuario: " + Session.GetHabbo().Username + "</b><br><b>Secuencia:</b> " + Session.GetHabbo().BannedPhraseCount + "/ 10.", "foto", "Investigar", "event:navigator/goto/" +
                    Session.GetHabbo().CurrentRoomId));
                    return;
                }
                if (Session.GetHabbo().BannedPhraseCount >= 10)
                {
                    MoonEnvironment.GetGame().GetModerationManager().BanUser("System", HabboHotel.Moderation.ModerationBanType.USERNAME, Session.GetHabbo().Username, "Baneado por hacer Spam con la Frase (" + Message + ")", (MoonEnvironment.GetUnixTimestamp() + 78892200));
                    Session.Disconnect();
                    return;
                }
                return;
            }

            foreach (int UserId in Targets)
            {
                if (!Session.GetHabbo().GetMessenger().FriendshipExists(UserId))
                    continue;

                GameClient Client = MoonEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().AllowMessengerInvites == true || Client.GetHabbo().AllowConsoleMessages == false)
                    continue;

                Client.SendMessage(new RoomInviteComposer(Session.GetHabbo().Id, Message));
                Client.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", "" + Session.GetHabbo().Username + " te invita a un nuevo evento en su sala. Su mensaje es " + Message + ".", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));

            }

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `chatlogs_console_invitations` (`user_id`,`message`,`timestamp`) VALUES ('" + Session.GetHabbo().Id + "', @message, UNIX_TIMESTAMP())");
                dbClient.AddParameter("message", Message);
                dbClient.RunQuery();
            }
        }
    }
}