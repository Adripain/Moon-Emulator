using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Items.Wired;

using Moon.Communication.Packets.Outgoing.Rooms.Engine;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.Communication.Packets.Outgoing.Rooms.Poll;
using Moon.HabboHotel.Items;
using System.Linq;
using Moon.HabboHotel.Rooms.Polls;
using Moon.Communication.Packets.Outgoing.Rooms.Polls;
using Moon.Communication.Packets.Outgoing.Rooms.Furni;
using System;
using System.Collections.Generic;
using Moon.HabboHotel.Rooms.AI;
using Moon.HabboHotel.Rooms.AI.Speech;

namespace Moon.Communication.Packets.Incoming.Rooms.Engine
{
    class GetRoomEntryDataEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            if (Session.GetHabbo().InRoom)
            {
                Room OldRoom;

                if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out OldRoom))
                    return;

                if (OldRoom.GetRoomUserManager() != null)
                    OldRoom.GetRoomUserManager().RemoveUserFromRoom(Session, false, false);
            }

            if (!Room.GetRoomUserManager().AddAvatarToRoom(Session))
            {
                Room.GetRoomUserManager().RemoveUserFromRoom(Session, false, false);
                return;
            }               
            

            Room.SendObjects(Session);

            //Status updating for messenger, do later as buggy.

            try
            {
                if (Session.GetHabbo().GetMessenger() != null)
                    Session.GetHabbo().GetMessenger().OnStatusChanged(true);
            }
            catch { }

            if (Session.GetHabbo().GetStats().QuestID > 0)
                MoonEnvironment.GetGame().GetQuestManager().QuestReminder(Session, Session.GetHabbo().GetStats().QuestID);

            Session.SendMessage(new RoomEntryInfoComposer(Room.RoomId, Room.CheckRights(Session, true)));
            Session.SendMessage(new RoomVisualizationSettingsComposer(Room.WallThickness, Room.FloorThickness, MoonEnvironment.EnumToBool(Room.Hidewall.ToString())));

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);

            if (ThisUser != null && Session.GetHabbo().PetId == 0)
                Room.SendMessage(new UserChangeComposer(ThisUser, false));

            if (!Session.GetHabbo().Effects().HasEffect(0) && Session.GetHabbo().Rank < 2)
                Session.GetHabbo().Effects().ApplyEffect(0);

            Session.SendMessage(new RoomEventComposer(Room.RoomData, Room.RoomData.Promotion));

            if (Session.GetHabbo().Rank > 3 && !Session.GetHabbo().StaffOk)
                Session.SendMessage(new GnomeBoxComposer(0));

            if (Room.poolQuestion != string.Empty)
            {
                Session.SendMessage(new QuickPollMessageComposer(Room.poolQuestion));

                if (Room.yesPoolAnswers.Contains(Session.GetHabbo().Id) || Room.noPoolAnswers.Contains(Session.GetHabbo().Id))
                    Session.SendMessage(new QuickPollResultsMessageComposer(Room.yesPoolAnswers.Count, Room.noPoolAnswers.Count));
            }

            if (Room.GetWired() != null)
                Room.GetWired().TriggerEvent(WiredBoxType.TriggerRoomEnter, Session.GetHabbo());

            if (Room.ForSale && Room.SalePrice > 0 && (Room.GetRoomUserManager().GetRoomUserByHabbo(Room.OwnerName) != null))
                Session.SendWhisper("Esta Sala esta en venta, en " + Room.SalePrice + " Duckets. Escribe :buyroom si deseas comprarla!");
            else if (Room.ForSale && Room.GetRoomUserManager().GetRoomUserByHabbo(Room.OwnerName) == null)
            {
                foreach (RoomUser _User in Room.GetRoomUserManager().GetRoomUsers())
                {
                    if (_User.GetClient() != null && _User.GetClient().GetHabbo() != null && _User.GetClient().GetHabbo().Id != Session.GetHabbo().Id)
                        _User.GetClient().SendWhisper("Esta Sala ya no se encuentra a la venta.");
                }
                Room.ForSale = false;
                Room.SalePrice = 0;
            }

            RoomPoll poll = null;
            
            if (MoonEnvironment.GetGame().GetPollManager().TryGetPollForRoom(Room.Id, out poll) && poll.Type == RoomPollType.Poll)
            {
                    if (!Session.GetHabbo().GetPolls().CompletedPolls.Contains(poll.Id))
                        Session.SendMessage(new PollOfferComposer(poll));
            }

            if (MoonEnvironment.GetUnixTimestamp() < Session.GetHabbo().FloodTime && Session.GetHabbo().FloodTime != 0)
                Session.SendMessage(new FloodControlComposer((int)Session.GetHabbo().FloodTime - (int)MoonEnvironment.GetUnixTimestamp()));

            if (Room.OwnerId == Session.GetHabbo().Id)
            {

                if (Session.GetHabbo()._NUX == true)
                {

                    List<RandomSpeech> BotSpeechList = new List<RandomSpeech>();
                    RoomUser BotUser = Room.GetRoomUserManager().DeployBot(new RoomBot(0, Session.GetHabbo().CurrentRoomId, "welcome", "freeroam", "Frank", "Manager del hotel", "hr-3194-38-36.hd-180-1.ch-220-1408.lg-285-73.sh-906-90.ha-3129-73.fa-1206-73.cc-3039-73", 0, 0, 0, 4, 0, 0, 0, 0, ref BotSpeechList, "", 0, 0, false, 0, false, 33), null);


                }
                else
                {
                    //User has already gotten today's prize :(
                }
            }
        }
    }
}