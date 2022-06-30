using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Users;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Groups;
using Moon.Communication.Packets.Outgoing.Groups;
using Moon.Communication.Packets.Outgoing.Rooms.Permissions;
using Moon.HabboHotel.Cache;



namespace Moon.Communication.Packets.Incoming.Groups
{
    class TakeAdminRightsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            Group Group = null;
            if (!MoonEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            if (!Group.IsMember(UserId) || !Group.IsAdmin(Session.GetHabbo().Id))
            return;

            Habbo Habbo = MoonEnvironment.GetHabboById(UserId);
            if (Habbo == null)
            {
                Session.SendNotification("Oops, ocurrio un error mientras se realizaba la busqueda de este usuario.");
                return;
            }

            Group.TakeAdmin(UserId);

            Room Room = null;
            if (MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room))
            {
                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(UserId);
                if (User != null)
                {
                    if (User.Statusses.ContainsKey("flatctrl 3"))
                        User.RemoveStatus("flatctrl 3");
                    User.UpdateNeeded = true;
                    if (User.GetClient() != null)
                        User.GetClient().SendMessage(new YouAreControllerComposer(0));
                }
            }

            Session.SendMessage(new GroupMemberUpdatedComposer(GroupId, Habbo, 2));
        }
    }
}
