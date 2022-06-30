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
    internal class SpecialEvent : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "user_13";
            }
        }
        public string Parameters
        {
            get { return "[EXPLICACION]"; }
        }
        public string Description
        {
            get
            {
                return "Manda um evento a todo o hotel.";
            }
        }
        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, digite uma mensagem para enviar.", 34);
                return;
            }
            else
            {
                string Message = CommandManager.MergeParams(Params, 1);

                MoonEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("O que está acontecendo em " + MoonEnvironment.HotelName + "...?",
                     Message, "event_image", "Hora da aventura!", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
            }

        }
    }
}

