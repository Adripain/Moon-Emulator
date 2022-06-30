
using Moon.Communication.Packets.Outgoing.Navigator;

using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;
using Moon.Communication.Packets.Incoming;

namespace Moon.Communication.Packets.Incoming.Navigator
{
    public class AddFavouriteRoomEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null)
                return;

            int RoomId = Packet.PopInt();

            RoomData Data = MoonEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);

            if (Data == null || Session.GetHabbo().FavoriteRooms.Count >= 30 || Session.GetHabbo().FavoriteRooms.Contains(RoomId))
            {
                // send packet that favourites is full.
                return;
            }

            Session.GetHabbo().FavoriteRooms.Add(RoomId);
            Session.SendMessage(new UpdateFavouriteRoomComposer(RoomId, true));

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("INSERT INTO user_favorites (user_id,room_id) VALUES (" + Session.GetHabbo().Id + "," + RoomId + ")");
            }
        }
    }
}