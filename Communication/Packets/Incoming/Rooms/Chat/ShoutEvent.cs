using System;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Quests;
using Moon.HabboHotel.Rooms;
using Moon.Utilities;
using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.HabboHotel.Rooms.Chat.Styles;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing;
using System.Collections.Generic;
using System.Linq;
using Moon.Communication.Packets.Outgoing.Messenger;

namespace Moon.Communication.Packets.Incoming.Rooms.Chat
{
    public class ShoutEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            string Message = StringCharFilter.Escape(Packet.PopString());
            if (Message.Length > 100)
                Message = Message.Substring(0, 100);

            int Colour = Packet.PopInt();

            ChatStyle Style = null;
            if (!MoonEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Colour, out Style) || (Style.RequiredRight.Length > 0 && !Session.GetHabbo().GetPermissions().HasRight(Style.RequiredRight)))
                Colour = 0;

            User.LastBubble = Session.GetHabbo().CustomBubbleId == 0 ? Colour : Session.GetHabbo().CustomBubbleId;

            if (MoonEnvironment.GetUnixTimestamp() < Session.GetHabbo().FloodTime && Session.GetHabbo().FloodTime != 0)
                return;

            if (Session.GetHabbo().TimeMuted > 0)
            {
                Session.SendMessage(new MutedComposer(Session.GetHabbo().TimeMuted));
                return;
            }

            if (!Room.CheckRights(Session, false) && Room.muteSignalEnabled == true)
            {
                Session.SendWhisper("La sala está silenciada, no puedes hablar en ella hasta tanto el dueño o alguien con permisos en ella lo permita.");
                return;
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("room_ignore_mute") && Room.CheckMute(Session))
            {
                Session.SendWhisper("Oops, usted se encuentra silenciad@.");
                return;
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                int MuteTime;
                if (User.IncrementAndCheckFlood(out MuteTime))
                {
                    Session.SendMessage(new FloodControlComposer(MuteTime));
                    return;
                }
            }

            if (Session.GetHabbo().LastMessage == Message)
            {
                Session.GetHabbo().LastMessageCount++;
                if (Session.GetHabbo().LastMessageCount > 3)
                {
                    MoonEnvironment.GetGame().GetClientManager().RepeatAlert(new RoomInviteComposer(int.MinValue, "Repeat: " + Session.GetHabbo().Username + " / Frase: " + Message + " / Veces: " + Session.GetHabbo().LastMessageCount + "."));
                    Session.GetHabbo().LastMessageCount = 0;
                }
            }

            if (Message.Contains("&#1Âº;") || Message.Contains("&#1Âº") || Message.Contains("&#"))
            { Session.SendMessage(new MassEventComposer("habbopages/spammer.txt")); return; }

            Room.GetFilter().CheckMessage(Message);
            if (Room.GetWired().TriggerEvent(HabboHotel.Items.Wired.WiredBoxType.TriggerUserSays, Session.GetHabbo(), Message.ToLower()))
            {
                return;
            }

            if (Message.StartsWith(":", StringComparison.CurrentCulture) && MoonEnvironment.GetGame().GetChatManager().GetCommands().Parse(Session, Message))
                return;

            MoonEnvironment.GetGame().GetChatManager().GetLogs().StoreChatlog(new Moon.HabboHotel.Rooms.Chat.Logs.ChatlogEntry(Session.GetHabbo().Id, Room.Id, Message, UnixTimestamp.GetNow(), Session.GetHabbo(), Room));

            string word;
            if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override") &&
                MoonEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Message, out word))
            {
                Session.GetHabbo().BannedPhraseCount++;
                if (Session.GetHabbo().BannedPhraseCount >= 1)
                {
                    Session.SendWhisper("¡Has mencionado una palabra no apta para el código de " + MoonEnvironment.GetDBConfig().DBData["hotel.name"] + "! Aviso " + Session.GetHabbo().BannedPhraseCount + "/10");
                    
                    MoonEnvironment.GetGame().GetClientManager().StaffAlert1(new RoomInviteComposer(int.MinValue, "Spammer: " + Session.GetHabbo().Username + " / Frase: " + Message + " / Palabra: " + word.ToUpper() + " / Fase: " + Session.GetHabbo().BannedPhraseCount + " / 10."));
                    MoonEnvironment.GetGame().GetClientManager().StaffAlert2(new RoomNotificationComposer("Alerta de publicista:",
                    "<b><font color=\"#B40404\">Por favor, recuerda investigar bien antes de recurrir a una sanción.</font></b><br><br>Palabra: <b>" + word.ToUpper() + "</b>.<br><br><b>Frase:</b><br><i>" + Message +
                    "</i>.<br><br><b>Tipo:</b><br>Chat de sala.\r\n" + "<b>Usuario: " + Session.GetHabbo().Username + "</b><br><b>Secuencia:</b> " + Session.GetHabbo().BannedPhraseCount + "/10.", "foto", "Investigar", "event:navigator/goto/" +
                    Session.GetHabbo().CurrentRoomId));
                    return;

                }
                if (Session.GetHabbo().BannedPhraseCount >= 10)
                {
                    MoonEnvironment.GetGame().GetModerationManager().BanUser("System", HabboHotel.Moderation.ModerationBanType.USERNAME, Session.GetHabbo().Username, "Baneado por hacer spam con la frase (" + Message + ")", (MoonEnvironment.GetUnixTimestamp() + 78892200));
                    Session.Disconnect();
                    return;
                }
                Session.SendMessage(new ShoutComposer(User.VirtualId, "Mensaje Inapropiado", 0, Colour));
                return;
            }

            Session.GetHabbo().LastMessage = Message;
            MoonEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_CHAT);
       
            User.UnIdle();
            User.OnChat(User.LastBubble, Message, true);
        }
    }
}