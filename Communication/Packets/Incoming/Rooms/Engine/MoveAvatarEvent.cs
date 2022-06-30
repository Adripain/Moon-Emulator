using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using System.Drawing;

namespace Moon.Communication.Packets.Incoming.Rooms.Engine
{
    class MoveAvatarEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null || !User.CanWalk)
                return;

            int MoveX = Packet.PopInt();
            int MoveY = Packet.PopInt();

            if (MoveX == User.X && MoveY == User.Y)
            {
                if (User.IsWalking)
                    User.PathCounter++;
                else
                    return;

                if (User.PathCounter == 4)
                    return;

                User.SamePath = true;
            }
            else
                User.SamePath = false;

            //    return;

            if (User.RidingHorse)
            {
                RoomUser Horse = Room.GetRoomUserManager().GetRoomUserByVirtualId(User.HorseID);
                if (Horse != null)
                    Horse.MoveTo(MoveX, MoveY);
            }
            User.DistancePath = 0;
            int a = Math.Abs((MoveX - User.X));
            int b = Math.Abs((MoveY - User.Y));
            int c = ((a * a) + (b * b));
            int distance = Convert.ToInt32(Math.Sqrt(c));
            if (User.X != MoveX && User.Y != MoveY)
            {
                distance = distance - 1;
                User.DiagMove = true;
            }
            else
            {
                User.DiagMove = false;
            }
            if (!User.IsWalking)
            {
                User.DistancePath = distance;
            }
            User.boolcount = 0;
            User.MoveTo(MoveX, MoveY);
        }
    }
}