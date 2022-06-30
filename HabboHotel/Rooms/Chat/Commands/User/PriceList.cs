using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Users;
using Moon.Communication.Packets.Outgoing.Notifications;


using Moon.Communication.Packets.Outgoing.Handshake;
using Moon.Communication.Packets.Outgoing.Quests;
using Moon.HabboHotel.Items;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.HabboHotel.Quests;
using Moon.HabboHotel.Rooms;
using System.Threading;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Avatar;
using Moon.Communication.Packets.Outgoing.Pets;
using Moon.Communication.Packets.Outgoing.Messenger;
using Moon.HabboHotel.Users.Messenger;
using Moon.Communication.Packets.Outgoing.Rooms.Polls;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Availability;
using Moon.Communication.Packets.Outgoing;
using Moon.Communication.Packets.Outgoing.Nux;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class PriceList : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_info"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Vê a lista dos preços dos raros."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            StringBuilder List = new StringBuilder("");
            List.AppendLine("                          ¥ LISTA DE PREÇOS DO "+Convert.ToString(MoonEnvironment.GetConfig().data["hotel.name"])+"");
            List.AppendLine("   SOFÁ VIP: Duckets   »   SOFÁ VIP: Duckets   »   SOFÁ VIP: Duckets");
            List.AppendLine("   SOFÁ VIP: Duckets   »   SOFÁ VIP: Duckets   »   SOFÁ VIP: Duckets");
            List.AppendLine("   SOFÁ VIP: Duckets   »   SOFÁ VIP: Duckets   »   SOFÁ VIP: Duckets");
            List.AppendLine("   SOFÁ VIP: Duckets   »   SOFÁ VIP: Duckets   »   SOFÁ VIP: Duckets");
            List.AppendLine("   SOFÁ VIP: Duckets   »   SOFÁ VIP: Duckets   »   SOFÁ VIP: Duckets");
            List.AppendLine("   SOFÁ VIP: Duckets   »   SOFÁ VIP: Duckets   »   SOFÁ VIP: Duckets");
            List.AppendLine("Esta lista está em construção pelo Imperial, sua última atualização foi no día 23 de Agosto de 2018.");
            Session.SendMessage(new MOTDNotificationComposer(List.ToString()));


        }
    }
}