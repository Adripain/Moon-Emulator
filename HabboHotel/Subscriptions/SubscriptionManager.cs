﻿using log4net;
using Moon.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Subscriptions
{
    public class SubscriptionManager
    {
        private static ILog log = LogManager.GetLogger("Moon.HabboHotel.Subscriptions.SubscriptionManager");

        private readonly Dictionary<int, SubscriptionData> _subscriptions = new Dictionary<int, SubscriptionData>();

        public SubscriptionManager()
        {
        }

        public void Init()
        {
            if (this._subscriptions.Count > 0)
                this._subscriptions.Clear();

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `subscriptions`;");
                DataTable GetSubscriptions = dbClient.getTable();

                if (GetSubscriptions != null)
                {
                    foreach (DataRow Row in GetSubscriptions.Rows)
                    {
                        if (!this._subscriptions.ContainsKey(Convert.ToInt32(Row["id"])))
                            this._subscriptions.Add(Convert.ToInt32(Row["id"]), new SubscriptionData(Convert.ToInt32(Row["id"]), Convert.ToString(Row["name"]), Convert.ToString(Row["badge_code"]), Convert.ToInt32(Row["credits"]), Convert.ToInt32(Row["duckets"]), Convert.ToInt32(Row["respects"])));
                    }
                }
            }

            //log.Info(">> Subscription Manager " + this._subscriptions.Count + " loaded.");
            log.Info(">> Subscription Manager -> Pronto!");
        }

        public bool TryGetSubscriptionData(int Id, out SubscriptionData Data)
        {
            return this._subscriptions.TryGetValue(Id, out Data);
        }
    }
}
