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
    class CustomLegit : IChatCommand
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
            get { return "O que o destino nos prepara..."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Session.SendMessage(new NuxAlertComposer("helpBubble/add/CHAT_INPUT/Death awaits us..."));
            Session.SendMessage(new NuxAlertComposer("nux/lobbyoffer/hide"));
            MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_Login", 1);
        }
    }
}