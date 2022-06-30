using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Users;
using Moon.HabboHotel.Users.Relationships;
using Moon.HabboHotel.Users.Messenger;
using Moon.HabboHotel.Groups;

namespace Moon.Communication.Packets.Outgoing.Messenger
{
    class FriendListUpdateComposer : ServerPacket
    {
        public FriendListUpdateComposer(int FriendId)
            : base(ServerPacketHeader.FriendListUpdateMessageComposer)
        {
            base.WriteInteger(1);//Category Count
            base.WriteInteger(1);
            base.WriteString("Grupos");
            base.WriteInteger(1);//Updates Count
            base.WriteInteger(-1);//Update
            base.WriteInteger(FriendId);
        }

        public FriendListUpdateComposer(Group Group, int State)
           : base(ServerPacketHeader.FriendListUpdateMessageComposer)
        {
            base.WriteInteger(1);//Category Count
            base.WriteInteger(1);
            base.WriteString("Grupos");
            base.WriteInteger(1);//Updates Count

            base.WriteInteger(State);//Update
            base.WriteInteger(-Group.Id);
            base.WriteString(Group.Name);
            base.WriteInteger(0);

            base.WriteBoolean(true);
            base.WriteBoolean(false);

            base.WriteString(Group.Badge);//Habbo.IsOnline ? Habbo.Look : "");
            base.WriteInteger(1); // categoryid
            base.WriteString("");
            base.WriteString(string.Empty); // Facebook username
            base.WriteString(string.Empty);
            base.WriteBoolean(true); // Allows offline messaging
            base.WriteBoolean(false); // ?
            base.WriteBoolean(false); // Uses phone
            base.WriteShort(0);
        }

        public FriendListUpdateComposer(GameClient Session, MessengerBuddy Buddy)
            : base(ServerPacketHeader.FriendListUpdateMessageComposer)
        {
            base.WriteInteger(1);//Category Count
            base.WriteInteger(1);
            base.WriteString("Grupos");
            base.WriteInteger(1);//Updates Count
            base.WriteInteger(0);//Update

            Relationship Relationship = Session.GetHabbo().Relationships.FirstOrDefault(x => x.Value.UserId == Convert.ToInt32(Buddy.UserId)).Value;
            int y = Relationship == null ? 0 : Relationship.Type;

            base.WriteInteger(Buddy.UserId);
            base.WriteString(Buddy.mUsername);
            base.WriteInteger(1);
            if (!Buddy.mAppearOffline || Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                base.WriteBoolean(Buddy.IsOnline);
            else
                base.WriteBoolean(false);

            if (!Buddy.mHideInroom || Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                base.WriteBoolean(Buddy.InRoom);
            else
                base.WriteBoolean(false);

            base.WriteString("");//Habbo.IsOnline ? Habbo.Look : "");
            base.WriteInteger(0); // categoryid
            base.WriteString(string.Empty);
            base.WriteString(string.Empty); // Facebook username
            base.WriteString(string.Empty);
            base.WriteBoolean(true); // Allows offline messaging
            base.WriteBoolean(false); // ?
            base.WriteBoolean(false); // Uses phone
            base.WriteShort(y);
        }
    }
}