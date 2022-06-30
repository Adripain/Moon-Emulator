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
    internal class PubliAlert : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "command_publi_alert";
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
                return "Manda uma promoção a todo o Hotel!";
            }
        }
        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            string Message = CommandManager.MergeParams(Params, 1);
            MoonEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("Criou uma nova promoção..",
                 "Uma nova promoção foi lançada! Se quer ganhar <b>varias recompensas</b> por participar entre na sala da promoção.<br><br>Quem criou a notícia? <b> <font color=\"#58ACFA\">  "
                 + Session.GetHabbo().Username + "</font></b><br>Se você quer participar clique no botão abaixo<b>Ir a sala da promoção</b>, e poderás participar.<br><br>Do que se trata?<br><br><font color='#084B8A'><b>Trata de seguir as instruções para participar e ganhar seu premio!</b></font><br><br>Te esperamos!", "zpam", "Ir a sala da Promoção", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));

        }
    }
}

