using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class CarryCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return "[ID]"; }
        }

        public string Description
        {
            get { return "Permite carregar um item em sua mão."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            int ItemId = 0;
            if (!int.TryParse(Convert.ToString(Params[1]), out ItemId))
            {
                Session.SendWhisper("Por favor introduza um item valido", 34);
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            User.CarryItem(ItemId);
        }
    }
}
