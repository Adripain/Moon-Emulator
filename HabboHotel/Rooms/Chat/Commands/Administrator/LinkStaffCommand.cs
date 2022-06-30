using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    internal class LinkStaffCommand : IChatCommand
    {
        public string PermissionRequired { get  { return "user_13";   }  }
        public string Parameters  { get { return "%message%"; }  }
        public string Description { get { return "Envia um link ao quarto"; } }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            string Link = Params[1];
            string Message = CommandManager.MergeParams(Params, 2);

            RoomUser actor = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            Room.SendMessage(new ChatComposer(actor.VirtualId, "<font color=\"#2E9AFE\"><a href='" + Link + "' target='_blank'><b>" + Message + "</b></a></font>", 0, 2));
        }
    }
}

