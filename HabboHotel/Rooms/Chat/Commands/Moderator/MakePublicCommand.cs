using Moon.HabboHotel.GameClients;
using System.Collections.Generic;
using System.Linq;
using Moon.Communication.Packets.Outgoing.Rooms.Session;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class MakePublicCommand : IChatCommand
    {
        public string PermissionRequired => "user_11";
        public string Parameters => "";
        public string Description => "Converter este quarto em público.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            var room = Session.GetHabbo().CurrentRoom;
            using (var queryReactor = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                queryReactor.runFastQuery(string.Format("UPDATE rooms SET roomtype = 'public' WHERE id = {0}",
                    room.RoomId));

            var roomId = Session.GetHabbo().CurrentRoom.RoomId;
            var users = new List<RoomUser>(Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUsers().ToList());

            MoonEnvironment.GetGame().GetRoomManager().UnloadRoom(Session.GetHabbo().CurrentRoom);

            RoomData Data = MoonEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);
            Session.GetHabbo().PrepareRoom(Session.GetHabbo().CurrentRoom.RoomId, "");

            MoonEnvironment.GetGame().GetRoomManager().LoadRoom(roomId);

            var data = new RoomForwardComposer(roomId);

            foreach (var user in users.Where(user => user != null && user.GetClient() != null))
                user.GetClient().SendMessage(data);
        }
    }
}
