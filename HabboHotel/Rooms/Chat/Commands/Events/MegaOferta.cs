using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moon.Utilities;
using Moon.HabboHotel.Users;
using Moon.HabboHotel.GameClients;
using Moon.Database.Interfaces;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.Core;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class MegaOferta : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_15"; }
        }

        public string Parameters
        {
            get { return "[ON] ó [OFF]"; }
        }

        public string Description
        {
            get { return "Cria e deleta uma MegaOferta."; ; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("advice", "Ops, você deve esrever assim: ':megaoferta on ou :megaoferta off'!", ""));
                return;
            }

            if (Params[1] == "on" || Params[1] == "ON")
            {
                // Comando editaveu abaixo mais cuidado pra não faze merda
                using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE targeted_offers SET active = 'true' WHERE active = 'false'");
                    dbClient.RunQuery("UPDATE users SET targeted_buy = '0'");
                }
                MoonEnvironment.GetGame().GetTargetedOffersManager().Initialize(MoonEnvironment.GetDatabaseManager().GetQueryReactor());
                MoonEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("volada", "Corre, uma nova MegaOferta foi lançada!", ""));
                Session.SendWhisper("Nova mega oferta iniciada!");
            }

            if (Params[1] == "off" || Params[1] == "OFF")
            {
                // Comando editaveu abaixo mais cuidado pra não faze merda
                using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE targeted_offers SET active = 'false' WHERE active = 'true'");
                    dbClient.RunQuery("UPDATE users SET targeted_buy = '0'");
                }
                MoonEnvironment.GetGame().GetTargetedOffersManager().Initialize(MoonEnvironment.GetDatabaseManager().GetQueryReactor());
                MoonEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("ADM", "Que pena, a MegaOferta foi removida!", ""));
                Session.SendWhisper("Mega oferta retirada!");
            }

            if (Params[1] != "on" || Params[1] != "off")
            {
                //Session.SendMessage(new RoomNotificationComposer("erro", "message", "Ops, usted debe teclear así: ':megaoferta on o :megaoferta off'!"));
                Session.SendMessage(RoomNotificationComposer.SendBubble("advice", "Ops, você deve escrever assim: ':megaoferta on o :megaoferta off'!", ""));

            }
        }
    }
}
