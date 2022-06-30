using System;
using System.Collections.Generic;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.Core;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Quests;
using Moon.HabboHotel.Rooms;
using Moon.Communication.Packets.Incoming;
using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.Utilities;
using Moon.HabboHotel.Rooms.Chat.Styles;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Messenger;

namespace Moon.Communication.Packets.Incoming.Rooms.Chat
{
    public class WhisperEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;
            if (Session.GetHabbo().Rank > 3 && !Session.GetHabbo().StaffOk)
                return;
            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool") && Room.CheckMute(Session))
            {
                Session.SendWhisper("Oops, usted se encuentra silenciad@");
                return;
            }

            if (MoonEnvironment.GetUnixTimestamp() < Session.GetHabbo().FloodTime && Session.GetHabbo().FloodTime != 0)
                return;

            string Params = Packet.PopString();
            string ToUser = Params.Split(' ')[0];
            string Message = Params.Substring(ToUser.Length + 1);
            int Colour = Packet.PopInt();

            if (Message.Contains("&#1Âº;") || Message.Contains("&#1Âº") || Message.Contains("&#"))
            { Session.SendMessage(new MassEventComposer("habbopages/spammer.txt")); return; }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            RoomUser User2 = Room.GetRoomUserManager().GetRoomUserByHabbo(ToUser);
            if (User2 == null)
                return;

            if (Session.GetHabbo().TimeMuted > 0)
            {
                Session.SendMessage(new MutedComposer(Session.GetHabbo().TimeMuted));
                return;
            }

            ChatStyle Style = null;
            if (!MoonEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Colour, out Style) || (Style.RequiredRight.Length > 0 && !Session.GetHabbo().GetPermissions().HasRight(Style.RequiredRight)))
                Colour = 0;

            User.LastBubble = Session.GetHabbo().CustomBubbleId == 0 ? Colour : Session.GetHabbo().CustomBubbleId;

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                int MuteTime;
                if (User.IncrementAndCheckFlood(out MuteTime))
                {
                    Session.SendMessage(new FloodControlComposer(MuteTime));
                    return;
                }
            }


            if (!User2.GetClient().GetHabbo().ReceiveWhispers && !Session.GetHabbo().GetPermissions().HasRight("room_whisper_override"))
            {
                Session.SendWhisper("Oops, este usuario no permite susurros.");
                return;
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

            Room.GetFilter().CheckMessage(Message);

            MoonEnvironment.GetGame().GetChatManager().GetLogs().StoreChatlog(new Moon.HabboHotel.Rooms.Chat.Logs.ChatlogEntry(Session.GetHabbo().Id, Room.Id, ": " + Message, UnixTimestamp.GetNow(), Session.GetHabbo(), Room));

            Room.AddChatlog(Session.GetHabbo().Id, ": " + Message);

            string word;
            if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override") && 
                MoonEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Message, out word))
            {
                Session.GetHabbo().BannedPhraseCount++;

                if (Session.GetHabbo().BannedPhraseCount >= 1)
                {
                    Session.SendWhisper("¡Has mencionado una palabra no apta para el hotel! Aviso " + Session.GetHabbo().BannedPhraseCount + "/10");

                    DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    dtDateTime = dtDateTime.AddSeconds(MoonEnvironment.GetUnixTimestamp()).ToLocalTime();

                    MoonEnvironment.GetGame().GetClientManager().StaffAlert1(new RoomInviteComposer(int.MinValue, "Spammer: " + Session.GetHabbo().Username + " / Frase: " + Message + " / Palabra: " + word.ToUpper() + " / Fase: " + Session.GetHabbo().BannedPhraseCount + " / 10."));
                    MoonEnvironment.GetGame().GetClientManager().StaffAlert2(new RoomNotificationComposer("Alerta de publicista:",
                    "<b><font color=\"#B40404\">Por favor, recuerda investigar bien antes de recurrir a una sanción.</font></b><br><br>Palabra: <b>" + word.ToUpper() + "</b>.<br><br><b>Frase:</b><br><i>" + Message +
                    "</i>.<br><br><b>Tipo:</b><br>Chat de sala.\r\n" + "<b>Usuario: " + Session.GetHabbo().Username + "</b><br><b>Secuencia:</b> " + Session.GetHabbo().BannedPhraseCount + "/10.", "foto", "Investigar", "event:navigator/goto/" +
                    Session.GetHabbo().CurrentRoomId));
                    return;
                }
                if (Session.GetHabbo().BannedPhraseCount >= 10)
                {
                    MoonEnvironment.GetGame().GetModerationManager().BanUser("System", HabboHotel.Moderation.ModerationBanType.USERNAME, Session.GetHabbo().Username, "Baneado por hacer Spam con la Frase (" + Message + ")", (MoonEnvironment.GetUnixTimestamp() + 78892200));
                    Session.Disconnect();
                    return;
                }

                Session.SendMessage(new WhisperComposer(User.VirtualId, "Mensaje inapropiado.", 0, User.LastBubble));
                return;
            }

            List<RoomUser> MultiW = Session.GetHabbo().MultiWhispers;
            if (MultiW.Count > 0)
            {
                foreach (RoomUser user in MultiW)
                {
                    if (user != null && user.HabboId != User2.HabboId && user.HabboId != User.HabboId)
                    {
                        if (user.GetClient() != null && user.GetClient().GetHabbo() != null && !user.GetClient().GetHabbo().IgnorePublicWhispers)
                        {
                            user.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "@blue@" + Message, 0, User.LastBubble));
                        }
                    }
                }
            }

            Session.GetHabbo().LastMessage = Message;
            MoonEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_CHAT);

            User.UnIdle();
            User.GetClient().SendMessage(new WhisperComposer(User.VirtualId, Message, 0, User.LastBubble));

            if (User2 != null && !User2.IsBot && User2.UserId != User.UserId)
            {
                if (!User2.GetClient().GetHabbo().MutedUsers.Contains(Session.GetHabbo().Id))
                {
                    User2.GetClient().SendMessage(new WhisperComposer(User.VirtualId, Message, 0, User.LastBubble));
                }
            }

            List<RoomUser> ToNotify = Room.GetRoomUserManager().GetRoomUserByRank(4);
            if (ToNotify.Count > 0)
            {
                foreach (RoomUser user in ToNotify)
                {
                    if (user != null && user.HabboId != User2.HabboId && user.HabboId != User.HabboId)
                    {
                        if (user.GetClient() != null && user.GetClient().GetHabbo() != null && !user.GetClient().GetHabbo().IgnorePublicWhispers)
                        {
                            user.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "@red@ [" + ToUser + "] " + Message, 0, User.LastBubble));
                        }
                    }
                }
            }
        }
    }
}