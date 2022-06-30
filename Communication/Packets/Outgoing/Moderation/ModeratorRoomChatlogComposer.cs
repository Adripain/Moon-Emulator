using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;
using Moon.Database.Interfaces;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;

using Moon.Utilities;
using Moon.HabboHotel.Cache;

namespace Moon.Communication.Packets.Outgoing.Moderation
{
    class ModeratorRoomChatlogComposer : ServerPacket
    {
        public ModeratorRoomChatlogComposer(Room Room)
            : base(ServerPacketHeader.ModeratorRoomChatlogMessageComposer)
        {
            base.WriteByte(1);
            base.WriteShort(2);//Count
           base.WriteString("roomName");
            base.WriteByte(2);
           base.WriteString(Room.Name);
           base.WriteString("roomId");
            base.WriteByte(1);
            base.WriteInteger(Room.Id);

            DataTable Table = null;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `chatlogs` WHERE `room_id` = @rid ORDER BY `id` DESC LIMIT 250");
                dbClient.AddParameter("rid", Room.Id);
                Table = dbClient.getTable();
            }

            base.WriteShort(Table.Rows.Count);
            if (Table != null)
            {
                foreach (DataRow Row in Table.Rows)
                {
                    UserCache Habbo = MoonEnvironment.GetGame().GetCacheManager().GenerateUser(Convert.ToInt32(Row["user_id"]));

                    if (Habbo == null)
                    {
                        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        dtDateTime = dtDateTime.AddSeconds(Convert.ToInt32(Row["timestamp"])).ToLocalTime();

                        base.WriteString(dtDateTime.Hour + ":" + dtDateTime.Minute);
                        base.WriteInteger(-1);
                        base.WriteString("Unknown User");
                        base.WriteString(string.IsNullOrWhiteSpace(Convert.ToString(Row["message"])) ? "*user sent a blank message*" : Convert.ToString(Row["message"]));
                        base.WriteBoolean(false);
                    }
                    else
                    {
                        DateTime dDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        dDateTime = dDateTime.AddSeconds(Convert.ToInt32(Row["timestamp"])).ToLocalTime();

                        base.WriteString(dDateTime.Hour + ":" + dDateTime.Minute);
                        base.WriteInteger(Habbo.Id);
                        base.WriteString(Habbo.Username);
                        base.WriteString(string.IsNullOrWhiteSpace(Convert.ToString(Row["message"]) )? "*user sent a blank message*" : Convert.ToString(Row["message"]));
                        base.WriteBoolean(false);
                    }
                }
            }
        }
    }
}
