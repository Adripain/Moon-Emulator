﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Items;
using Moon.HabboHotel.Quests;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;

using Moon.Database.Interfaces;


namespace Moon.Communication.Packets.Incoming.Rooms.Engine
{
    class ApplyDecorationEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = null;
            if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CheckRights(Session, true))
                return;

            Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(Packet.PopInt());
            if (Item == null)
                return;

            if (Item.GetBaseItem() == null)
                return;

            string DecorationKey = string.Empty;
            switch (Item.GetBaseItem().InteractionType)
            {
                case InteractionType.FLOOR:
                    DecorationKey = "floor";
                    break;

                case InteractionType.WALLPAPER:
                    DecorationKey = "wallpaper";
                    break;

                case InteractionType.LANDSCAPE:
                    DecorationKey = "landscape";
                    break;
            }

            switch (DecorationKey)
            {
                case "floor":
                    Room.Floor = Item.ExtraData;
                    Room.RoomData.Floor = Item.ExtraData;

                    MoonEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_DECORATION_FLOOR);
                    MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomDecoFloor", 1);
                    break;

                case "wallpaper":
                    Room.Wallpaper = Item.ExtraData;
                    Room.RoomData.Wallpaper = Item.ExtraData;

                    MoonEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_DECORATION_WALL);
                    MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomDecoWallpaper", 1);
                    break;

                case "landscape":
                    Room.Landscape = Item.ExtraData;
                    Room.RoomData.Landscape = Item.ExtraData;

                    MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomDecoLandscape", 1);
                    break;
            }

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `rooms` SET " + DecorationKey + " = @extradata WHERE `id` = '" + Room.RoomId + "' LIMIT 1");
                dbClient.AddParameter("extradata", Item.ExtraData);
                dbClient.RunQuery();

                dbClient.RunQuery("DELETE FROM items WHERE id=" + Item.Id + " LIMIT 1");
            }

            Session.GetHabbo().GetInventoryComponent().RemoveItem(Item.Id);
            Room.SendMessage(new RoomPropertyComposer(DecorationKey, Item.ExtraData));
        }
    }
}
