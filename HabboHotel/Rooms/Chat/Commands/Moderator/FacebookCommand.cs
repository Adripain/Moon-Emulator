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

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class FacebookCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_13"; }
        }

        public string Parameters
        {
            get { return "[DE Q SE TRATA]"; }
        }

        public string Description
        {
            get { return "Novo concurso do fb!!"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 2)
            {
                Session.SendWhisper("Por favor insira a mensagem e o link para enviar..");
                return;
            }

            string URL = "https://www.facebook.com/"+Convert.ToString(MoonEnvironment.GetConfig().data["hotel.name"])+"hotel/";

            string Message = CommandManager.MergeParams(Params, 1);
            Session.SendMessage(new RoomNotificationComposer("Há um novo concurso no Facebook",
                 "   Do que se trata?\n\n\n" +
                 "  <font color=\"#a62984\"><b>" + Message +
                 "  <br><br></b></font><b>O concurso do Facebook é feito por: </b> <b><font color=\"#224CAD\">" + Session.GetHabbo().Username +
                 "  <br><br></b></font></b><b>Hora atual:</b> " + DateTime.Now + "\n\n" +
                 "Para acessar a página do Facebook, clique em Ir para o Facebook", "habbo_talent_show_stage", "Ir para o Facebook >>", URL));
            return;

        }
    }
}
