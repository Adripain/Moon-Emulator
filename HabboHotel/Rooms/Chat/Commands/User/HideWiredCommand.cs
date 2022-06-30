using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.HabboHotel.Global;
using System.Globalization;
using Moon.Database.Interfaces;
using Moon.Communication.Packets.Outgoing;
using Moon.HabboHotel.Items;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class HideWiredCommand : IChatCommand
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
            get { return "Esonde os Wireds do seu quarto."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            if (!Room.CheckRights(Session, false, false))
            {
                Session.SendWhisper("Não tens permissões nesta sala.", 34);
                return;
            }

            Room.HideWired = !Room.HideWired;
            if (Room.HideWired)
                Session.SendWhisper("Você escondeu todos os Wireds da sala.", 34);
            else
                Session.SendWhisper("Você mostrou todos os Wireds da sala.", 34);

            //using (IQueryAdapter con = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            //{
            //    con.SetQuery("UPDATE `rooms` SET `hide_wired` = @enum WHERE `id` = @id LIMIT 1");
            //    con.AddParameter("enum", MoonEnvironment.BoolToEnum(Room.HideWired));
            //    con.AddParameter("id", Room.Id);
            //    con.RunQuery();
            //}

            List<ServerPacket> list = new List<ServerPacket>();

            list = Room.HideWiredMessages(Room.HideWired);

            Room.SendMessage(list);


        }
    }
}
