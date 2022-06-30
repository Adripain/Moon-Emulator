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
    class ColourList : IChatCommand
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
            get { return "Informacão do "+Convert.ToString(MoonEnvironment.GetConfig().data["hotel.name"])+"."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Session.SendMessage(new RoomNotificationComposer("Lista de color:",
                 "<font color='#FF8000'><b>COLORES:</b>\n" +
                 "<font size=\"12\" color=\"#1C1C1C\">O comando :color te permitirá fixar uma cor que você deseja em chat, para poder selecionar a cor , deverá especificar depois de fazer o comando, como por exemplo:<br><i>:color red</i></font>" +
                 "<font size =\"13\" color=\"#0B4C5F\"><b>Stats:</b></font>\n" +
                 "<font size =\"11\" color=\"#1C1C1C\">  <b> · Users: </b> \r  <b> · Rooms: </b> \r  <b> · Uptime: </b>minutes.</font>\n\n" +
                 "", "quantum", ""));
        }
    }
}