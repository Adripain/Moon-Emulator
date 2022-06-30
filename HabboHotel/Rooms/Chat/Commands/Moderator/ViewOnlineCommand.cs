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

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class ViewOnlineCommand : IChatCommand
    {
        public string PermissionRequired => "user_6";
        public string Parameters => "";
        public string Description => "Ver os usuários online.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Dictionary<Habbo, UInt32> clients = new Dictionary<Habbo, UInt32>();

            StringBuilder content = new StringBuilder();
            content.Append("- LISTA DE USUÁRIOS ONLINE -\r\n");

            foreach (var client in MoonEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                if (client == null)
                    continue;

                content.Append("¥ " + client.GetHabbo().Username + " » Se encontra no quarto: " + ((client.GetHabbo().CurrentRoom == null) ? "Em nenhum quarto." : client.GetHabbo().CurrentRoom.RoomData.Name) + "\r\n");
            }

            Session.SendMessage(new MOTDNotificationComposer(content.ToString()));
            return;
        }
    }
}
