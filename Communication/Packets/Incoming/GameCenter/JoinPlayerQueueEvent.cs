using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Incoming;
using Moon.Communication.Packets;
using Moon.Communication.Packets.Outgoing.GameCenter;
using Moon.HabboHotel.Games;

namespace Moon.Communication.Packets.Incoming.GameCenter
{
    internal class JoinPlayerQueueEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if ((Session == null) || (Session.GetHabbo() == null))
                return;

            var GameId = Packet.PopInt();

            if (GameId == 1)
            {
                GameData GameData = null;
                if (MoonEnvironment.GetGame().GetGameDataManager().TryGetGame(GameId, out GameData))
                {
                    Session.SendMessage(new JoinQueueComposer(GameData.GameId));
                    var HabboID = Session.GetHabbo().Id;
                    using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        DataTable data;
                        dbClient.SetQuery("SELECT user_id FROM user_auth_ticket WHERE user_id = '" + HabboID + "'");
                        data = dbClient.getTable();
                        var count = 0;
                        foreach (DataRow row in data.Rows)
                        {
                            if (Convert.ToInt32(row["user_id"]) == HabboID)
                                count++;
                        }
                        if (count == 0)
                        {
                            var SSOTicket = "Fasfu-" + GenerateSSO(32) + "-" + Session.GetHabbo().Id;
                            dbClient.RunQuery("INSERT INTO user_auth_ticket(user_id, auth_ticket) VALUES ('" + HabboID +
                                              "', '" +
                                              SSOTicket + "')");
                            Session.SendMessage(new LoadGameComposer(GameData, SSOTicket, Session));
                        }
                        else
                        {
                            dbClient.SetQuery("SELECT user_id,auth_ticket FROM user_auth_ticket WHERE user_id = " + HabboID);
                            data = dbClient.getTable();
                            foreach (DataRow dRow in data.Rows)
                            {
                                var SSOTicket = dRow["auth_ticket"];
                                Session.SendMessage(new LoadGameComposer(GameData, (string)SSOTicket, Session));
                            }
                        }

                    }
                }
            }

            if (GameId == 2)
            {
                GameData GameData = null;
                if (MoonEnvironment.GetGame().GetGameDataManager().TryGetGame(GameId, out GameData))
                {
                    Session.SendMessage(new JoinQueueComposer(GameData.GameId));
                    var HabboID = Session.GetHabbo().Id;
                    using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        DataTable data;
                        dbClient.SetQuery("SELECT user_id FROM user_auth_ticket WHERE user_id = '" + HabboID + "'");
                        data = dbClient.getTable();
                        var count = 0;
                        foreach (DataRow row in data.Rows)
                        {
                            if (Convert.ToInt32(row["user_id"]) == HabboID)
                                count++;
                        }
                        if (count == 0)
                        {
                            var SSOTicket = "Snow-" + GenerateSSO(32) + "-" + Session.GetHabbo().Id;
                            dbClient.RunQuery("INSERT INTO user_auth_ticket(user_id, auth_ticket) VALUES ('" + HabboID +
                                              "', '" +
                                              SSOTicket + "')");
                            Session.SendMessage(new LoadGameComposer(GameData, SSOTicket, Session));
                        }
                        else
                        {
                            dbClient.SetQuery("SELECT user_id,auth_ticket FROM user_auth_ticket WHERE user_id = " + HabboID);
                            data = dbClient.getTable();
                            foreach (DataRow dRow in data.Rows)
                            {
                                var SSOTicket = dRow["auth_ticket"];
                                Session.SendMessage(new LoadGameComposer(GameData, (string)SSOTicket, Session));
                            }
                        }
                    }
                }
            }
        }

        private string GenerateSSO(int length)
        {
            var random = new Random();
            var characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var result = new StringBuilder(length);
            for (var i = 0; i < length; i++)
                result.Append(characters[random.Next(characters.Length)]);
            return result.ToString();
        }
    }
}