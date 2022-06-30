using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Moon.Core;
using Moon.HabboHotel.Catalog;
using Moon.HabboHotel.Groups;
using Moon.HabboHotel.Items;
using Moon.HabboHotel.Rooms.AI;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users.Badges;
using Moon.HabboHotel.Achievements;
using Moon.HabboHotel.Users.Inventory;
using Moon.HabboHotel.Users.Messenger;
using Moon.HabboHotel.Users.Relationships;
using Moon.HabboHotel.Users.Authenticator;

using Moon.Database.Interfaces;
using Plus.HabboHotel.Club;

namespace Moon.HabboHotel.Users.UserDataManagement
{
    public class UserDataFactory
    {
        public static UserData GetUserData(string SessionTicket, out byte errorCode)
        {
            int UserId;
            DataRow dUserInfo = null;
            DataTable dAchievements = null;
            DataTable dFavouriteRooms = null;
            DataTable dTags = null;
            DataTable dMystic = null;
            DataTable dIgnores = null;
            DataTable dBadges = null;
            DataTable dEffects = null;
            DataTable dFriends = null;
            DataTable dRequests = null;
            DataTable dRooms = null;
            DataTable dQuests = null;
            DataTable dRelations = null;
            DataTable talentsTable = null;
            DataRow UserInfo = null;
            DataTable Subscriptions = null;

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `users` WHERE `auth_ticket` = @sso LIMIT 1");
                dbClient.AddParameter("sso", SessionTicket);
                dUserInfo = dbClient.getRow();

                if (dUserInfo == null)
                {
                    errorCode = 1;
                    return null;
                }

                UserId = Convert.ToInt32(dUserInfo["id"]);
                if (MoonEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId) != null)
                {
                    errorCode = 2;
                    MoonEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId).Disconnect();
                    return null;
                }

                dbClient.SetQuery("SELECT `group`,`level`,`progress` FROM `user_achievements` WHERE `userid` = '" + UserId + "'");
                dAchievements = dbClient.getTable();

                dbClient.SetQuery("SELECT room_id FROM user_favorites WHERE `user_id` = '" + UserId + "'");
                dFavouriteRooms = dbClient.getTable();

                dbClient.SetQuery("SELECT ignore_id FROM user_ignores WHERE `user_id` = '" + UserId + "'");
                dIgnores = dbClient.getTable();

                dbClient.SetQuery("SELECT `tag_name` FROM user_tags WHERE `user_id` = '" + UserId + "'");
                dTags = dbClient.getTable();

                dbClient.SetQuery("SELECT * FROM user_mystic_data WHERE `user_id` = '" + UserId + "'");
                dMystic = dbClient.getTable();

                dbClient.SetQuery("SELECT `badge_id`,`badge_slot` FROM user_badges WHERE `user_id` = '" + UserId + "'");
                dBadges = dbClient.getTable();

                dbClient.SetQuery("SELECT `effect_id`,`total_duration`,`is_activated`,`activated_stamp` FROM user_effects WHERE `user_id` = '" + UserId + "'");
                dEffects = dbClient.getTable();

                dbClient.SetQuery(
                    "SELECT users.id,users.username,users.motto,users.look,users.last_online,users.hide_inroom,users.hide_online " +
                    "FROM users " +
                    "JOIN messenger_friendships " +
                    "ON users.id = messenger_friendships.user_one_id " +
                    "WHERE messenger_friendships.user_two_id = " + UserId + " " +
                    "UNION ALL " +
                    "SELECT users.id,users.username,users.motto,users.look,users.last_online,users.hide_inroom,users.hide_online " +
                    "FROM users " +
                    "JOIN messenger_friendships " +
                    "ON users.id = messenger_friendships.user_two_id " +
                    "WHERE messenger_friendships.user_one_id = " + UserId);
                dFriends = dbClient.getTable();

                dbClient.SetQuery("SELECT messenger_requests.from_id,messenger_requests.to_id,users.username FROM users JOIN messenger_requests ON users.id = messenger_requests.from_id WHERE messenger_requests.to_id = " + UserId);
                dRequests = dbClient.getTable();

                dbClient.SetQuery("SELECT * FROM rooms WHERE `owner` = '" + UserId + "' LIMIT 150");
                dRooms = dbClient.getTable();

                dbClient.SetQuery("SELECT * FROM users_talents WHERE userid = '" + UserId + "'");
                talentsTable = dbClient.getTable();

                dbClient.SetQuery("SELECT `quest_id`,`progress` FROM user_quests WHERE `user_id` = '" + UserId + "'");
                dQuests = dbClient.getTable();

                dbClient.SetQuery("SELECT * FROM `user_relationships` WHERE `user_id` = @id");
                dbClient.AddParameter("id", UserId);
                dRelations = dbClient.getTable();

                dbClient.SetQuery("SELECT * FROM user_subscriptions WHERE user_id = '" + UserId + "'");
                Subscriptions = dbClient.getTable();

                dbClient.SetQuery("SELECT * FROM `user_info` WHERE `user_id` = '" + UserId + "' LIMIT 1");
                UserInfo = dbClient.getRow();
                if (UserInfo == null)
                {
                    dbClient.RunQuery("INSERT INTO `user_info` (`user_id`) VALUES ('" + UserId + "')");

                    dbClient.SetQuery("SELECT * FROM `user_info` WHERE `user_id` = '" + UserId + "' LIMIT 1");
                    UserInfo = dbClient.getRow();
                }

                dbClient.runFastQuery("UPDATE users SET online='1' WHERE id=" + UserId + " LIMIT 1");
                dbClient.RunQuery("DELETE FROM `user_auth_ticket` WHERE `user_id` = '" + UserId + "' LIMIT 1");

            }

            ConcurrentDictionary<string, UserAchievement> Achievements = new ConcurrentDictionary<string, UserAchievement>();
            foreach (DataRow dRow in dAchievements.Rows)
            {
                Achievements.TryAdd(Convert.ToString(dRow["group"]), new UserAchievement(Convert.ToString(dRow["group"]), Convert.ToInt32(dRow["level"]), Convert.ToInt32(dRow["progress"])));
            }

            List<int> favouritedRooms = new List<int>();
            foreach (DataRow dRow in dFavouriteRooms.Rows)
            {
                favouritedRooms.Add(Convert.ToInt32(dRow["room_id"]));
            }

            List<int> ignores = new List<int>();
            foreach (DataRow dRow in dIgnores.Rows)
            {
                ignores.Add(Convert.ToInt32(dRow["ignore_id"]));
            }

            List<string> tags = new List<string>();
            foreach (DataRow dRow in dTags.Rows)
            {
                tags.Add(Convert.ToString(dRow["tag_name"]));
            }

            List<string> mystic_keys = new List<string>();
            foreach (DataRow dRow in dMystic.Rows)
            {
                mystic_keys.Add(Convert.ToString(dRow["mystic_keys"]));
            }

            List<string> mystic_boxes = new List<string>();
            foreach (DataRow dRow in dMystic.Rows)
            {
                mystic_boxes.Add(Convert.ToString(dRow["mystic_boxes"]));
            }

            List<Badge> badges = new List<Badge>();
            foreach (DataRow dRow in dBadges.Rows)
            {
                badges.Add(new Badge(Convert.ToString(dRow["badge_id"]), Convert.ToInt32(dRow["badge_slot"])));
            }

            Dictionary<int, MessengerBuddy> friends = new Dictionary<int, MessengerBuddy>();
            foreach (DataRow dRow in dFriends.Rows)
            {
                int friendID = Convert.ToInt32(dRow["id"]);
                string friendName = Convert.ToString(dRow["username"]);
                string friendLook = Convert.ToString(dRow["look"]);
                string friendMotto = Convert.ToString(dRow["motto"]);
                int friendLastOnline = Convert.ToInt32(dRow["last_online"]);
                bool friendHideOnline = MoonEnvironment.EnumToBool(dRow["hide_online"].ToString());
                bool friendHideRoom = MoonEnvironment.EnumToBool(dRow["hide_inroom"].ToString());

                if (friendID == UserId)
                    continue;

                if (!friends.ContainsKey(friendID))
                    friends.Add(friendID, new MessengerBuddy(friendID, friendName, friendLook, friendMotto, friendLastOnline, friendHideOnline, friendHideRoom));
            }

            Dictionary<int, MessengerRequest> requests = new Dictionary<int, MessengerRequest>();
            foreach (DataRow dRow in dRequests.Rows)
            {
                int receiverID = Convert.ToInt32(dRow["from_id"]);
                int senderID = Convert.ToInt32(dRow["to_id"]);

                string requestUsername = Convert.ToString(dRow["username"]);

                if (receiverID != UserId)
                {
                    if (!requests.ContainsKey(receiverID))
                        requests.Add(receiverID, new MessengerRequest(UserId, receiverID, requestUsername));
                }
                else
                {
                    if (!requests.ContainsKey(senderID))
                        requests.Add(senderID, new MessengerRequest(UserId, senderID, requestUsername));
                }
            }

            List<RoomData> rooms = new List<RoomData>();
            foreach (DataRow dRow in dRooms.Rows)
            {
                rooms.Add(MoonEnvironment.GetGame().GetRoomManager().FetchRoomData(Convert.ToInt32(dRow["id"]), dRow));
            }

            Dictionary<int, int> quests = new Dictionary<int, int>();
            foreach (DataRow dRow in dQuests.Rows)
            {
                int questId = Convert.ToInt32(dRow["quest_id"]);

                if (quests.ContainsKey(questId))
                    quests.Remove(questId);

                quests.Add(questId, Convert.ToInt32(dRow["progress"]));
            }

            Dictionary<int, Relationship> Relationships = new Dictionary<int, Relationship>();
            foreach (DataRow Row in dRelations.Rows)
            {
                if (friends.ContainsKey(Convert.ToInt32(Row[2])))
                    Relationships.Add(Convert.ToInt32(Row[2]), new Relationship(Convert.ToInt32(Row[0]), Convert.ToInt32(Row[2]), Convert.ToInt32(Row[3].ToString())));
            }

            Dictionary<int, UserTalent> talents = new Dictionary<int, UserTalent>();
            if (talentsTable != null)
            {
                foreach (DataRow row in talentsTable.Rows)
                {
                    int num2 = (int)row["talent_id"];
                    int state = (int)row["talent_state"];

                    talents.Add(num2, new UserTalent(num2, state));
                }
            }

            Dictionary<string, Subscription> subscriptions = new Dictionary<string, Subscription>();
            foreach (DataRow dataRow in Subscriptions.Rows)
            {
                string str = (string)dataRow["subscription_id"];
                int TimeExpire = (int)dataRow["timestamp_expire"];

                subscriptions.Add(str, new Subscription(str, TimeExpire));
            }


            Habbo user = HabboFactory.GenerateHabbo(dUserInfo, UserInfo);

            dUserInfo = null;
            dAchievements = null;
            dFavouriteRooms = null;
            dIgnores = null;
            dBadges = null;
            dEffects = null;
            dFriends = null;
            dRequests = null;
            dRooms = null;
            dRelations = null;
            dTags = null;
            dMystic = null;

            errorCode = 0;
            return new UserData(UserId, Achievements, favouritedRooms, tags, mystic_boxes, mystic_keys, ignores, badges, friends, requests, rooms, quests, user, Relationships, talents, subscriptions);
        }

        public static UserData GetUserData(int UserId)
        {
            DataRow dUserInfo = null;
            DataRow UserInfo = null;
            DataTable dRelations = null;
            DataTable dGroups = null;

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `users` WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("id", UserId);
                dUserInfo = dbClient.getRow();

                MoonEnvironment.GetGame().GetClientManager().LogClonesOut(Convert.ToInt32(UserId));

                if (dUserInfo == null)
                    return null;

                if (MoonEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId) != null)
                    return null;


                dbClient.SetQuery("SELECT * FROM `user_info` WHERE `user_id` = '" + UserId + "' LIMIT 1");
                UserInfo = dbClient.getRow();
                if (UserInfo == null)
                {
                    dbClient.RunQuery("INSERT INTO `user_info` (`user_id`) VALUES ('" + UserId + "')");

                    dbClient.SetQuery("SELECT * FROM `user_info` WHERE `user_id` = '" + UserId + "' LIMIT 1");
                    UserInfo = dbClient.getRow();
                }

                dbClient.SetQuery("SELECT group_id,rank FROM group_memberships WHERE user_id=@id");
                dbClient.AddParameter("id", UserId);
                dGroups = dbClient.getTable();

                dbClient.SetQuery("SELECT `id`,`target`,`type` FROM user_relationships WHERE user_id=@id");
                dbClient.AddParameter("id", UserId);
                dRelations = dbClient.getTable();
            }

            ConcurrentDictionary<string, UserAchievement> Achievements = new ConcurrentDictionary<string, UserAchievement>();
            Dictionary<int, UserTalent> talents = new Dictionary<int, UserTalent>();
            List<int> FavouritedRooms = new List<int>();
            List<string> Tags = new List<string>();
            List<string> MysticBoxes = new List<string>();
            List<string> MysticKeys = new List<string>();
            List<int> Ignores = new List<int>();
            List<Badge> Badges = new List<Badge>();
            Dictionary<int, MessengerBuddy> Friends = new Dictionary<int, MessengerBuddy>();
            Dictionary<int, MessengerRequest> FriendRequests = new Dictionary<int, MessengerRequest>();
            List<RoomData> Rooms = new List<RoomData>();
            Dictionary<int, int> Quests = new Dictionary<int, int>();
            Dictionary<string, Subscription> subscriptions = new Dictionary<string, Subscription>();

            Dictionary<int, Relationship> Relationships = new Dictionary<int, Relationship>();
            foreach (DataRow Row in dRelations.Rows)
            {
                if (!Relationships.ContainsKey(Convert.ToInt32(Row["id"])))
                {
                    Relationships.Add(Convert.ToInt32(Row["target"]), new Relationship(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["target"]), Convert.ToInt32(Row["type"].ToString())));
                }
            }

            Habbo user = HabboFactory.GenerateHabbo(dUserInfo, UserInfo);
            return new UserData(UserId, Achievements, FavouritedRooms, Tags, MysticBoxes, MysticKeys, Ignores, Badges, Friends, FriendRequests, Rooms, Quests, user, Relationships, talents, subscriptions);
        }
    }
}