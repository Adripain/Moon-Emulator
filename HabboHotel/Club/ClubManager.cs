using Plus.HabboHotel.Club;
using Moon.Communication.Packets.Outgoing.Handshake;
using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Users.UserDataManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moon.HabboHotel.Club
{
    internal class ClubManager
    {
        private readonly int UserId;
        private readonly Dictionary<string, Subscription> Subscriptions;

        internal ClubManager(int userID, UserData userData)
        {
            this.UserId = userID;
            this.Subscriptions = userData.subscriptions;
        }

        internal void Clear()
        {
            this.Subscriptions.Clear();
        }

        internal Subscription GetSubscription(string SubscriptionId)
        {
            if (this.Subscriptions.ContainsKey(SubscriptionId))
            {
                return this.Subscriptions[SubscriptionId];
            }
            else
            {
                return (Subscription)null;
            }
        }

        internal bool HasSubscription(string SubscriptionId)
        {
            if (!this.Subscriptions.ContainsKey(SubscriptionId))
            {
                return false;
            }

            Subscription subscription = this.Subscriptions[SubscriptionId];
            return subscription.IsValid();
        }

        internal void AddOrExtendSubscription(string SubscriptionId, int DurationSeconds, GameClient Session)
        {
            SubscriptionId = SubscriptionId.ToLower();


            if (this.Subscriptions.ContainsKey(SubscriptionId))
            {
                Subscription subscription = this.Subscriptions[SubscriptionId];

                if (subscription.IsValid())
                {
                    subscription.ExtendSubscription(DurationSeconds);
                }
                else
                {
                    subscription.SetEndTime((int)MoonEnvironment.GetUnixTimestamp() + DurationSeconds);
                }

                using (IQueryAdapter adapter = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    adapter.SetQuery(string.Concat(new object[] { "UPDATE user_subscriptions SET timestamp_expire = ", subscription.ExpireTime, " WHERE user_id = ", this.UserId, " AND subscription_id = '", subscription.SubscriptionId, "'" }));
                    adapter.RunQuery();
                }
            }
            else
            {
                int unixTimestamp = (int)MoonEnvironment.GetUnixTimestamp();
                int timeExpire = (int)MoonEnvironment.GetUnixTimestamp() + DurationSeconds;
                string SubscriptionType = SubscriptionId;
                Subscription subscription2 = new Subscription(SubscriptionId, timeExpire);

                using (IQueryAdapter adapter = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    adapter.SetQuery(string.Concat(new object[] { "INSERT INTO user_subscriptions (user_id,subscription_id,timestamp_activated,timestamp_expire) VALUES (", this.UserId, ",'", SubscriptionType, "',", unixTimestamp, ",", timeExpire, ")" }));
                    adapter.RunQuery();
                }

                this.Subscriptions.Add(subscription2.SubscriptionId.ToLower(), subscription2);
            }
        }


        internal void ReloadSubscription(GameClient Session)
        {
            Session.SendMessage(new UserRightsComposer(Session.GetHabbo()));
        }
    }
}