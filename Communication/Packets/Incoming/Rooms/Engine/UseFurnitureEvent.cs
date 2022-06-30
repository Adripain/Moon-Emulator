using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Quests;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Items;
using Moon.HabboHotel.Items.Wired;
using Moon.Communication.Packets.Outgoing.Rooms.Furni;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;

using Moon.Database.Interfaces;



namespace Moon.Communication.Packets.Incoming.Rooms.Engine
{
    class UseFurnitureEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room;

            if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            int itemID = Packet.PopInt();
            Item Item = Room.GetRoomItemHandler().GetItem(itemID);
            if (Item == null)
                return;

            bool hasRights = false;
            if (Room.CheckRights(Session, false, true))
                hasRights = true;

            if (Item.GetBaseItem().InteractionType == InteractionType.banzaitele)
                return;
            if (Item.GetBaseItem().InteractionType == InteractionType.HCGATE)
                return;
            if (Item.GetBaseItem().InteractionType == InteractionType.VIPGATE)
                return;
            if (Item.GetBaseItem().InteractionType == InteractionType.idol_chair)
                return;
            if (Item.GetBaseItem().InteractionType == InteractionType.idol_counter)
                return;

            if (Item.GetBaseItem().InteractionType == InteractionType.TONER)
            {
                if (!Room.CheckRights(Session, true))
                    return;
                if (Room.TonerData.Enabled == 0)
                    Room.TonerData.Enabled = 1;
                else
                    Room.TonerData.Enabled = 0;

                Room.SendMessage(new ObjectUpdateComposer(Item, Room.OwnerId));

                Item.UpdateState();

                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `room_items_toner` SET `enabled` = '" + Room.TonerData.Enabled + "' LIMIT 1");
                }
                return;
            }

            if (Item.Data.InteractionType == InteractionType.GNOME_BOX && Item.UserID == Session.GetHabbo().Id)
            {
                Session.SendMessage(new GnomeBoxComposer(Item.Id));
            }

            Boolean Toggle = true;
            if (Item.GetBaseItem().InteractionType == InteractionType.WF_FLOOR_SWITCH_1 || Item.GetBaseItem().InteractionType == InteractionType.WF_FLOOR_SWITCH_2)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (User == null)
                    return;

                if (!Gamemap.TilesTouching(Item.GetX, Item.GetY, User.X, User.Y))
                {
                    Toggle = false;
                }
            }

            string oldData = Item.ExtraData;
            int request = Packet.PopInt();

            Item.Interactor.OnTrigger(Session, Item, request, hasRights);

            if (Toggle)
                Item.GetRoom().GetWired().TriggerEvent(WiredBoxType.TriggerStateChanges, Session.GetHabbo(), Item);

            MoonEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, Item.GetBaseItem().Id);

        }
    }
}
