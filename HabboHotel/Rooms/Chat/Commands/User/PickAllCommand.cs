using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Items;


using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class PickAllCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Recolhe todos os mobis do quarto."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
                return;

            Room.GetRoomItemHandler().RemoveItems(Session);
            Room.GetGameMap().GenerateMaps();

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `items` SET `room_id` = '0' WHERE `room_id` = @RoomId AND `user_id` = @UserId");
                dbClient.AddParameter("RoomId", Room.Id);
                dbClient.AddParameter("UserId", Session.GetHabbo().Id);
                dbClient.RunQuery();
            }

            List<Item> Items = Room.GetRoomItemHandler().GetWallAndFloor.ToList();
            if (Items.Count > 0)
                Session.SendWhisper("Há mais mobis nesta sala?, expulse-os manualmente digitando :ejectall !", 34);

            Session.SendMessage(new FurniListUpdateComposer());
        }
    }
}