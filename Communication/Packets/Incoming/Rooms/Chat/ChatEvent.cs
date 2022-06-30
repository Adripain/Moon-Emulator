using System;

using Moon.Core;
using Moon.Communication.Packets.Incoming;
using Moon.Utilities;
using Moon.HabboHotel.Global;
using Moon.HabboHotel.Quests;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Rooms.Chat.Logs;
using Moon.Communication.Packets.Outgoing.Messenger;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.HabboHotel.Items.Data;

using Moon.HabboHotel.Rooms.Chat.Styles;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using System.Collections.Generic;

namespace Moon.Communication.Packets.Incoming.Rooms.Chat
{
    public class ChatEvent : IPacketEvent
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
            if (Session.GetHabbo().Rank > 3 && !Session.GetHabbo().StaffOk)
                return;

            string Message = StringCharFilter.Escape(Packet.PopString());
            if (Message.Length > 100)
                Message = Message.Substring(0, 100);

            int Colour = Packet.PopInt();

            if (Message.Contains("&#1Âº;") || Message.Contains("&#1Âº") || Message.Contains("&#"))
            { Session.SendMessage(new MassEventComposer("habbopages/spammer.txt")); return; }

            ChatStyle Style = null;
            if (!MoonEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Colour, out Style) || (Style.RequiredRight.Length > 0 && !Session.GetHabbo().GetPermissions().HasRight(Style.RequiredRight)))
                Colour = 0;

            User.UnIdle();

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
                Session.SendWhisper("Oops, usted se encuentra silenciad@");
                return;
            }

            User.LastBubble = Session.GetHabbo().CustomBubbleId == 0 ? Colour : Session.GetHabbo().CustomBubbleId;

            if (Room.GetWired().TriggerEvent(HabboHotel.Items.Wired.WiredBoxType.TriggerUserSays, Session.GetHabbo(), Message.ToLower()))
            {
                return;

            }
            else if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                int MuteTime;
                if (User.IncrementAndCheckFlood(out MuteTime))
                {
                    Session.SendMessage(new FloodControlComposer(MuteTime));
                    return;
                }
            }

            Room.GetFilter().CheckMessage(Message);

            if (Message.StartsWith(":", StringComparison.CurrentCulture) && MoonEnvironment.GetGame().GetChatManager().GetCommands().Parse(Session, Message))
                return;

            if(Session.GetHabbo().LastMessage == Message)
            {
                Session.GetHabbo().LastMessageCount++;
                if(Session.GetHabbo().LastMessageCount > 3)
                {
                    MoonEnvironment.GetGame().GetClientManager().RepeatAlert(new RoomInviteComposer(int.MinValue, "Repeat: " + Session.GetHabbo().Username + " / Frase: " + Message + " / Veces: " + Session.GetHabbo().LastMessageCount + "."));
                    Session.GetHabbo().LastMessageCount = 0;
                }
            }

            MoonEnvironment.GetGame().GetChatManager().GetLogs().StoreChatlog(new ChatlogEntry(Session.GetHabbo().Id, Room.Id, Message, UnixTimestamp.GetNow(), Session.GetHabbo(), Room));
            string word;
            if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override") &&
                MoonEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Message, out word))
            {
                    Session.GetHabbo().BannedPhraseCount++;

                if (Session.GetHabbo().BannedPhraseCount >= 1)
                {
                    Session.SendWhisper("¡Has mencionado una palabra no apta para el hotel! Aviso " + Session.GetHabbo().BannedPhraseCount + "/10");

                    DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    dtDateTime = dtDateTime.AddSeconds(MoonEnvironment.GetUnixTimestamp()).ToLocalTime();

                    MoonEnvironment.GetGame().GetClientManager().StaffAlert1(new RoomInviteComposer(int.MinValue, "Spammer: " + Session.GetHabbo().Username + " / Frase: " + Message + " / Palabra: " + word.ToUpper() + " / Fase: " + Session.GetHabbo().BannedPhraseCount + " / 10."));
                    MoonEnvironment.GetGame().GetClientManager().StaffAlert2(new RoomNotificationComposer("Alerta de publicista:",
                    "<b><font color=\"#B40404\">Por favor, recuerda investigar bien antes de recurrir a una sanción.</font></b><br><br>Palabra: <b>" + word.ToUpper() + "</b>.<br><br><b>Frase:</b><br><i>" + Message +
                    "</i>.<br><br><b>Tipo:</b><br>Chat de sala.\r\n" + "<b>Usuario: " + Session.GetHabbo().Username + "</b><br><b>Secuencia:</b> " + Session.GetHabbo().BannedPhraseCount + "/10.", "foto", "Investigar", "event:navigator/goto/" +
                    Session.GetHabbo().CurrentRoomId));
                    
                    if (Session.GetHabbo().BannedPhraseCount >= 10)
                    {
                        MoonEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", "El usuario " + Session.GetHabbo().Username + " ha sido baneado de manera automática por el sistema.", ""));

                        MoonEnvironment.GetGame().GetModerationManager().BanUser("System", HabboHotel.Moderation.ModerationBanType.USERNAME, Session.GetHabbo().Username, "Baneado por hacer Spam con la Frase (" + word + ")", (MoonEnvironment.GetUnixTimestamp() + 78892200));
                        Session.Disconnect();
                            return;
                    }
                    return;
                }

                Session.SendMessage(new ChatComposer(User.VirtualId, "Mensaje inapropiado.", 0, Colour));
                return;
            }

            if (Session.GetHabbo().MultiWhisper)
            {
                Session.SendMessage(new WhisperComposer(User.VirtualId, "@blue@ [MULTI] " + Message, 0, User.LastBubble));
                List<RoomUser> MultiW = Session.GetHabbo().MultiWhispers;
                if (MultiW.Count > 0)
                {
                    foreach (RoomUser user in MultiW)
                    {
                        if (user != null)
                        {
                            if (user.GetClient() != null && user.GetClient().GetHabbo() != null && !user.GetClient().GetHabbo().IgnorePublicWhispers)
                            {
                                user.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "@blue@ [MULTI] " + Message, 0, User.LastBubble));
                            }
                        }
                    }
                }
                return;
            }

            //if (Session.GetHabbo().IsBeingAsked == true && Message.ToLower().Contains("s"))
            //{
            //    Session.GetHabbo().SecureTradeEnabled = true;
            //    Session.GetHabbo().IsBeingAsked = false;
            //    Session.SendMessage(new WiredSmartAlertComposer("Acabas de activar el modo seguro de tradeo para dados."));
            //}
            //else if (Session.GetHabbo().IsBeingAsked == true && !Message.ToLower().Contains("s"))
            //{
            //    Session.GetHabbo().SecureTradeEnabled = false;
            //    Session.GetHabbo().IsBeingAsked = false;
            //    Session.SendMessage(new WiredSmartAlertComposer("Has dejado el tradeo en modo normal."));
            //}

            Session.GetHabbo().LastMessage = Message;

            MoonEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_CHAT);
            User.OnChat(User.LastBubble, Message, false);
        }
    }
}