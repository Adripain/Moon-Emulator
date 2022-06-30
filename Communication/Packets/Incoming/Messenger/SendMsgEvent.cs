using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Messenger;

namespace Moon.Communication.Packets.Incoming.Messenger
{
    class SendMsgEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
                return;

            int userId = Packet.PopInt();
            if (userId == 0 || userId == Session.GetHabbo().Id)
                return;

            string message = Packet.PopString();
            if (string.IsNullOrWhiteSpace(message)) return;
            if (Session.GetHabbo().TimeMuted > 0)
            {
                Session.SendWhisper("Oops, has sido silenciado durante 15 segundos, no podrás enviar mensajes durante este lapso de tiempo.");
                return;
            }

            if (message.Contains("&#1Âº;") || message.Contains("&#1Âº") || message.Contains("&#"))
            { Session.SendMessage(new MassEventComposer("habbopages/spammer.txt")); return; }

            if (Session.GetHabbo().LastMessage == message)
            {
                Session.GetHabbo().LastMessageCount++;
                if (Session.GetHabbo().LastMessageCount > 3)
                {
                    MoonEnvironment.GetGame().GetClientManager().RepeatAlert(new RoomInviteComposer(int.MinValue, "Repeat: " + Session.GetHabbo().Username + " / Frase: " + message + " / Veces: " + Session.GetHabbo().LastMessageCount + "."));
                    Session.GetHabbo().LastMessageCount = 0;
                }
            }

            string word;
            if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override") &&
                MoonEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(message, out word))
            {
                Session.GetHabbo().BannedPhraseCount++;
                if (Session.GetHabbo().BannedPhraseCount >= 1)
                {
                    Session.GetHabbo().TimeMuted = 15;
                    Session.SendNotification("Acabas de mencionar una palabra prohibida en el filtro de " + MoonEnvironment.GetDBConfig().DBData["hotel.name"] + ", tal vez pueda tratarse de un error. Asegúrate de no volver a repetirla, este sistema está hecho para evitar publicistas. Recuerda que se acaba de advertir a los miembros del equipo, si no se trata de un caso de publicidad no te asustes. Aviso " + Session.GetHabbo().BannedPhraseCount + " / 10");
                    MoonEnvironment.GetGame().GetClientManager().StaffAlert1(new RoomInviteComposer(int.MinValue, "Spammer: " + Session.GetHabbo().Username + " / Frase: " + message + " / Palabra: " + word.ToUpper() + " / Fase: " + Session.GetHabbo().BannedPhraseCount + " / 10."));
                    MoonEnvironment.GetGame().GetClientManager().StaffAlert2(new RoomNotificationComposer("Alerta de publicista:",
                    "<b><font color=\"#B40404\">Por favor, recuerda investigar bien antes de recurrir a una sanción.</font></b><br><br>Palabra: <b>" + word.ToUpper() + "</b>.<br><br><b>Frase:</b><br><i>" + message +
                    "</i>.<br><br><b>Tipo:</b><br>Chat de sala.\r\n" + "<b>Usuario: " + Session.GetHabbo().Username + "</b><br><b>Secuencia:</b> " + Session.GetHabbo().BannedPhraseCount + "/ 10.", "foto", "Investigar", "event:navigator/goto/" +
                    Session.GetHabbo().CurrentRoomId));
                    return;
                }
                if (Session.GetHabbo().BannedPhraseCount >= 10)
                {
                    MoonEnvironment.GetGame().GetModerationManager().BanUser("Moon", HabboHotel.Moderation.ModerationBanType.USERNAME, Session.GetHabbo().Username, "Baneado por hacer spam con la Frase (" + message + ")", (MoonEnvironment.GetUnixTimestamp() + 78892200));
                    Session.Disconnect();
                    return;
                }
                return;
            }

            Session.GetHabbo().LastMessage = message;
            Session.GetHabbo().GetMessenger().SendInstantMessage(userId, message);

        }
    }
}