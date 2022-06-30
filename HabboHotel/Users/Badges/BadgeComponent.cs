using System;
using System.Collections;
using System.Collections.Generic;

using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Users.UserDataManagement;
using Moon.Communication.Packets.Incoming;

using Moon.Communication.Packets.Outgoing.Inventory.Badges;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;

using Moon.Database.Interfaces;
using Moon.HabboHotel.Badges;

namespace Moon.HabboHotel.Users.Badges
{
    public class BadgeComponent
    {
        private readonly Habbo _player;
        private readonly Dictionary<string, Badge> _badges;

        public BadgeComponent(Habbo Player, UserData data)
        {
            this._player = Player;
            this._badges = new Dictionary<string, Badge>();

            foreach (Badge badge in data.badges)
            {

                if (!this._badges.ContainsKey(badge.Code))
                    this._badges.Add(badge.Code, badge);
            }
        }

        public int Count
        {
            get { return _badges.Count; }
        }

        public int EquippedCount
        {
            get
            {
                int i = 0;

                foreach (Badge Badge in _badges.Values)
                {
                    if (Badge.Slot <= 0)
                    {
                        continue;
                    }

                    i++;
                }

                return i;
            }
        }

        public ICollection<Badge> GetBadges()
        {
            return this._badges.Values;
        }

        public Badge GetBadge(string Badge)
        {
            if (_badges.ContainsKey(Badge))
                return (Badge)_badges[Badge];

            return null;
        }

        public bool TryGetBadge(string BadgeCode, out Badge Badge)
        {
            return this._badges.TryGetValue(BadgeCode, out Badge);
        }

        public bool HasBadge(string Badge)
        {
            return _badges.ContainsKey(Badge);
        }

        public bool HasBadgeList(List<string> Badge)
        {
            foreach (string str in Badge)
            {
                if (!this._badges.ContainsKey(str))
                {
                    return false;
                }
            }
            return true;
        }

        public void GiveBadge(string Badge, Boolean InDatabase, GameClient Session)
        {
            if (HasBadge(Badge))
                return;



            if (InDatabase)
            {
                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO user_badges (user_id,badge_id,badge_slot) VALUES (" + _player.Id + ",@badge," + 0 + ")");
                    dbClient.AddParameter("badge", Badge);
                    dbClient.RunQuery();
                }
            }

            _badges.Add(Badge, new Badge(Badge, 0));

            if (Session != null)
            {
                Session.SendMessage(new BadgesComposer(Session));
                Session.SendMessage(new FurniListNotificationComposer(1, 4));
            }
        }

        public void ResetSlots()
        {
            foreach (Badge Badge in _badges.Values)
            {
                Badge.Slot = 0;
            }
        }

        public void RemoveBadge(string Badge, GameClient Session)
        {
            if (!HasBadge(Badge))
            {
                return;
            }

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM user_badges WHERE badge_id = @badge AND user_id = " + _player.Id + " LIMIT 1");
                dbClient.AddParameter("badge", Badge);
                dbClient.RunQuery();
            }

            if (_badges.ContainsKey(Badge))
                _badges.Remove(Badge);
        }

        public void RemoveBadge(string Badge)
        {
            if (!HasBadge(Badge))
            {
                return;
            }

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM user_badges WHERE badge_id = @badge AND user_id = " + _player.Id + " LIMIT 1");
                dbClient.AddParameter("badge", Badge);
                dbClient.RunQuery();
            }

            if (_badges.ContainsKey(Badge))
                _badges.Remove(Badge);
        }
    }
}