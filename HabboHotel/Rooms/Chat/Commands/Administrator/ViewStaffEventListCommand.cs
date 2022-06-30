using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Items;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Database.Interfaces;
using Moon.HabboHotel.Users;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class ViewStaffEventListCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_13"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Observa uma lista dos eventos abertos por os Staffs."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Dictionary<Habbo, UInt32> clients = new Dictionary<Habbo, UInt32>();

            StringBuilder content = new StringBuilder();
            content.Append("Lista de eventos abertos:\r\n");

            foreach (var client in MoonEnvironment.GetGame().GetClientManager()._clients.Values)
            {
                if (client != null && client.GetHabbo() != null && client.GetHabbo().Rank > 5)
                    clients.Add(client.GetHabbo(), (Convert.ToUInt16(client.GetHabbo().Rank)));
            }

            foreach (KeyValuePair<Habbo, UInt32> client in clients.OrderBy(key => key.Value))
            {
                if (client.Key == null)
                    continue;

                content.Append("¥ " + client.Key.Username + " [Rank: " + client.Key.Rank + "] - Abriu : " + client.Key._eventsopened + " eventos.\r\n");
            }

            Session.SendMessage(new MOTDNotificationComposer(content.ToString()));

            return;
        }
    }
}