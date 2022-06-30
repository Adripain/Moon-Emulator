using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;
using Moon.Database.Interfaces;
using System.Data;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;
using Moon.HabboHotel.Rooms;
using Moon.Communication.Packets.Outgoing.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class PrefixCommand2 : IChatCommand
    {
        public string PermissionRequired => "user_vip";
        public string Parameters => "%remove%";
        public string Description => "Deletar prefixo.";
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Diga ':prefixo remove' para deletar seu prefixo (a Tag)", 34);
                return;
            }

            if (Session.GetHabbo() == null)
                return;

            if (Params[1].ToLower() == "remove")
            {
                if (Session.GetHabbo().Rank >= 1)
                {
                    Session.GetHabbo()._tag = string.Empty;
                    UpdateDatabase(Session);
                }
            }

            Session.SendWhisper("Prefixo (a Tag) removido corretamente!", 34);
            return;
        }

        public void UpdateDatabase(GameClients.GameClient Session)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `users` SET `tag` = '" + Session.GetHabbo()._tag + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");

            }
        }
    }
}