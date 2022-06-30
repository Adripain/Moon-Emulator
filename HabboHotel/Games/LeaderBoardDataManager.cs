using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;

using Moon.Database.Interfaces;
using log4net;
using System.Globalization;

namespace Moon.HabboHotel.Games
{
    public class LeaderBoardDataManager
    {
        private static readonly ILog log = LogManager.GetLogger("Moon.HabboHotel.Games.GameDataManager");

        internal Dictionary<int, LeaderBoardData> _leaderboards;

        public LeaderBoardDataManager()
        {
            this._leaderboards = new Dictionary<int, LeaderBoardData>();

            this.Init();
        }

        public void Init()
        {
            if (_leaderboards.Count > 0)
                _leaderboards.Clear();

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable GetData = null;
                dbClient.SetQuery("SELECT * FROM `games_leaderboard`");
                GetData = dbClient.getTable();

                if (GetData != null)
                {
                    foreach (DataRow Row in GetData.Rows)
                    {
                        LeaderBoardData value = new LeaderBoardData(Convert.ToInt32(Row["game_id"]), Convert.ToInt32(Row["user_id"]), Convert.ToInt32(Row["points"]), Convert.ToInt32(Row["record"]), Convert.ToInt32(Row["week"]), Convert.ToInt32(Row["year"]));
                        this._leaderboards.Add(Convert.ToInt32(Row["id"]), value);
                    }
                }
            }

            log.Info(">> LeaderBoardData Manager -> Pronto!");
        }

        public bool TryGetLeaderBoardData(int GameId, out LeaderBoardData LeaderBoardData)
        {
            if (this._leaderboards.TryGetValue(GameId, out LeaderBoardData))
                return true;
            return false;
        }

        public bool TryGetLeaderBoardDataWithWeek(int GameId, int Week, out LeaderBoardData LeaderBoardData)
        {
            if (this._leaderboards.TryGetValue(Week, out LeaderBoardData) && this._leaderboards.TryGetValue(GameId, out LeaderBoardData))
                return true;
            return false;
        }

        public ICollection<LeaderBoardData> LeaderBoardData
        {
            get
            {
                return this._leaderboards.Values;
            }
        }

        public Dictionary<int, LeaderBoardData> getLeaderBoards()
        {
            return this._leaderboards;
        }
    }
}
