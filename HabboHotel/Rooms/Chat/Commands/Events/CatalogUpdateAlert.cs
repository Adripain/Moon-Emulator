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


namespace Moon.HabboHotel.Rooms.Chat.Commands.Events
{
    internal class CatalogUpdateAlert : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "command_addpredesigned";
            }
        }
        public string Parameters
        {
            get { return "%message%"; }
        }
        public string Description
        {
            get
            {
                return "Avisa uma atualização de Catalogo no hotel.";
            }
        }
        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            string Message = CommandManager.MergeParams(Params, 1);
            MoonEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("Atualização no Catálogo!",
              "O catálogo do <font color=\"#2E9AFE\"><b>"+Convert.ToString(MoonEnvironment.GetConfig().data["hotel.name"])+"</b></font> acaba de ser atualizado! Se quer conferir as <b>novidades</b> clique no botão abaixo.<br>", "cata", "ir para a páfgina", "event:catalog/open/" + Message));

            Session.SendWhisper("Catalogo atualizado com sucesso.");
        }
    }
}

