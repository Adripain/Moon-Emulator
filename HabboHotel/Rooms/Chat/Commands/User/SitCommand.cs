using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class SitCommand : IChatCommand
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
            get { return "Senta no chão."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (User.Statusses.ContainsKey("lie") || User.isLying || User.RidingHorse || User.IsWalking)
                return;

            if (!User.Statusses.ContainsKey("sit"))
            {
                if ((User.RotBody % 2) == 0)
                {
                    if (User == null)
                        return;

                    try
                    {
                        User.Statusses.Add("sit", "1.0");
                        User.Z -= 0.35;
                        User.isSitting = true;
                        User.UpdateNeeded = true;
                    }
                    catch { }
                }
                else
                {
                    User.RotBody--;
                    User.Statusses.Add("sit", "1.0");
                    User.Z -= 0.35;
                    User.isSitting = true;
                    User.UpdateNeeded = true;
                }
            }
            else if (User.isSitting == true)
            {
                User.Z += 0.35;
                User.Statusses.Remove("sit");
                User.Statusses.Remove("1.0");
                User.isSitting = false;
                User.UpdateNeeded = true;
            }
        }
    }
}
