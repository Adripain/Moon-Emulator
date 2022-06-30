using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Communication.Packets.Outgoing.Rooms.Avatar;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Items;

using Moon.Communication.Packets.Outgoing.Rooms.Chat;

using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Database.Interfaces;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class AfkCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Ficar ausente."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            User.IsAsleep = true;
            Room.SendMessage(new SleepComposer(User, true));

            Session.SendWhisper("Agora você está dormindo!", 34);
        }
    }
}
