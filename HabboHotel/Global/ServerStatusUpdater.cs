using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using log4net;
using Moon.Database.Interfaces;


namespace Moon.HabboHotel.Global
{
    public class ServerStatusUpdater : IDisposable
    {
        private static ILog log = LogManager.GetLogger("Mango.Global.ServerUpdater");

        private const int UPDATE_IN_SECS = 30;
        string HotelName = MoonEnvironment.GetConfig().data["hotel.name"];

        private Timer _timer;

        public ServerStatusUpdater()
        {
        }

        public void Init()
        {
            this._timer = new Timer(new TimerCallback(this.OnTick), null, TimeSpan.FromSeconds(UPDATE_IN_SECS), TimeSpan.FromSeconds(UPDATE_IN_SECS));

            Console.Title = "MOON Server 1.0 - [0] USUÁRIOS - [0] QUARTOS - [0] TEMPO DE ATIVIDADE";

            log.Info(">> Server Status -> Pronto!");
        }

        public void OnTick(object Obj)
        {
            this.UpdateOnlineUsers();
        }

        private void UpdateOnlineUsers()
        {
            TimeSpan Uptime = DateTime.Now - MoonEnvironment.ServerStarted;

            var clientCount = MoonEnvironment.GetGame().GetClientManager().Count;
            int UsersOnline = Convert.ToInt32(MoonEnvironment.GetGame().GetClientManager().Count);
            int RoomCount = MoonEnvironment.GetGame().GetRoomManager().Count;
            Game.SessionUserRecord = clientCount > Game.SessionUserRecord ? clientCount : Game.SessionUserRecord;
            Console.Title = "MOON Server 1.0 - [" + UsersOnline + "] USUÁRIOS - [" + RoomCount + "] SALAS - [" + Uptime.Days + "] DIAS [" + Uptime.Hours + "] HORAS";

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `server_status` SET `users_online` = @users, `loaded_rooms` = @loadedRooms LIMIT 1;");
                dbClient.AddParameter("users", UsersOnline);
                dbClient.AddParameter("loadedRooms", RoomCount);
                dbClient.RunQuery();
            }
        }


        public void Dispose()
        {
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `server_status` SET `users_online` = '0', `loaded_rooms` = '0'");
            }

            this._timer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
