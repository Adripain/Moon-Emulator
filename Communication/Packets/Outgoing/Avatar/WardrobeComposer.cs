﻿using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;
using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;


namespace Moon.Communication.Packets.Outgoing.Avatar
{
    class WardrobeComposer : ServerPacket
    {
        public WardrobeComposer(GameClient Session)
            : base(ServerPacketHeader.WardrobeMessageComposer)
        {
            base.WriteInteger(1);
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `slot_id`,`look`,`gender` FROM `user_wardrobe` WHERE `user_id` = '" + Session.GetHabbo().Id + "'");
                DataTable WardrobeData = dbClient.getTable();

                if (WardrobeData == null)
                    base.WriteInteger(0);
                else
                {
                    base.WriteInteger(WardrobeData.Rows.Count);
                    foreach (DataRow Row in WardrobeData.Rows)
                    {
                        base.WriteInteger(Convert.ToInt32(Row["slot_id"]));
                       base.WriteString(Convert.ToString(Row["look"]));
                       base.WriteString(Row["gender"].ToString().ToUpper());
                    }
                }
            }
        }
    }
}
