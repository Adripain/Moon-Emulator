using System;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;

namespace Moon.Communication.Packets.Incoming.Rooms.Chat
{
    public class StartTypingEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
            if (User == null)
                return;

            Session.GetHabbo().CurrentRoom.SendMessage(new UserTypingComposer(User.VirtualId, true));
        }
    }
}