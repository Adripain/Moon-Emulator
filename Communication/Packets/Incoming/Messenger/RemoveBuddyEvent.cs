﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.GameClients;

using Moon.Database.Interfaces;


namespace Moon.Communication.Packets.Incoming.Messenger
{
    class RemoveBuddyEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
                return;

            int Amount = Packet.PopInt();
            if (Amount > 100)
                Amount = 100;
            else if (Amount < 0)
                return;

            for (int i = 0; i < Amount; i++)
            {
                int Id = Packet.PopInt();

                if (Session.GetHabbo().Relationships.Where(x => x.Value.UserId == Id).ToList().Count > 0)
                {
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = @id AND `target` = @target OR `target` = @id AND `user_id` = @target");
                        dbClient.AddParameter("id", Session.GetHabbo().Id);
                        dbClient.AddParameter("target", Id);
                        dbClient.RunQuery();
                    }
                }


                if (Session.GetHabbo().Relationships.ContainsKey(Convert.ToInt32(Id)))
                    Session.GetHabbo().Relationships.Remove(Convert.ToInt32(Id));

                GameClient Target = MoonEnvironment.GetGame().GetClientManager().GetClientByUserID(Id);
                if (Target != null)
                {
                    if (Target.GetHabbo().Relationships.ContainsKey(Convert.ToInt32(Session.GetHabbo().Id)))
                        Target.GetHabbo().Relationships.Remove(Convert.ToInt32(Session.GetHabbo().Id));
                }

                Session.GetHabbo().GetMessenger().DestroyFriendship(Id);
            }
        }
    }
}