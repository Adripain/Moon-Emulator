using System.Linq;
using System.Collections.Generic;
using Moon.Communication.Packets.Outgoing.Rooms.Session;


namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class Reloadcommand : IChatCommand
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
            get { return "Recarregue o quarto"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Session.GetHabbo().Id != Room.OwnerId && !Session.GetHabbo().GetPermissions().HasRight("room_any_owner"))
            {
                Session.SendWhisper("Lamentamos, este comando só está disponivel para o dono da sala");
                return;
            }

            List<RoomUser> UsersToReturn = Room.GetRoomUserManager().GetRoomUsers().ToList();

            MoonEnvironment.GetGame().GetRoomManager().UnloadRoom(Room);


            foreach (RoomUser User in UsersToReturn)
            {
                if (User == null || User.GetClient() == null)
                    continue;

                User.GetClient().SendMessage(new RoomForwardComposer(Room.Id));
            }


        }
    }
}
