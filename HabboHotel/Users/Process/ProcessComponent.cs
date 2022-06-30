﻿using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using log4net;

using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.HabboHotel.Items;
using Moon.Communication.Packets.Outgoing.Handshake;
using Moon.Database.Interfaces;

namespace Moon.HabboHotel.Users.Process
{
    sealed class ProcessComponent
    {
        private static readonly ILog log = LogManager.GetLogger("Moon.HabboHotel.Users.Process.ProcessComponent");

        /// <summary>
        /// Player to update, handle, change etc.
        /// </summary>
        private Habbo _player = null;

        /// <summary>
        /// ThreadPooled Timer.
        /// </summary>
        private Timer _timer = null;

        /// <summary>
        /// Prevents the timer from overlapping itself.
        /// </summary>
        private bool _timerRunning = false;

        /// <summary>
        /// Checks if the timer is lagging behind (server can't keep up).
        /// </summary>
        private bool _timerLagging = false;

        /// <summary>
        /// Enable/Disable the timer WITHOUT disabling the timer itself.
        /// </summary>
        private bool _disabled = false;

        /// <summary>
        /// Used for disposing the ProcessComponent safely.
        /// </summary>
        private AutoResetEvent _resetEvent = new AutoResetEvent(true);

        /// <summary>
        /// How often the timer should execute.
        /// </summary>
        private static int _runtimeInSec = 60;

        /// <summary>
        /// Default.
        /// </summary>
        public ProcessComponent()
        {
        }

        /// <summary>
        /// Initializes the ProcessComponent.
        /// </summary>
        /// <param name="Player">Player.</param>
        public bool Init(Habbo Player)
        {
            if (Player == null)
                return false;
            else if (this._player != null)
                return false;

            this._player = Player;
            this._timer = new Timer(new TimerCallback(Run), null, _runtimeInSec * 1000, _runtimeInSec * 1000);
            return true;
        }

        /// <summary>
        /// Called for each time the timer ticks.
        /// </summary>
        /// <param name="State"></param>
        public void Run(object State)
        {
            try
            {
                if (this._disabled)
                    return;

                if (this._timerRunning)
                {
                    this._timerLagging = true;
                    log.Warn("<Player " + this._player.Id + "> Server can't keep up, Player timer is lagging behind.");
                    return;
                }

                this._resetEvent.Reset();

                #region Muted Checks
                if (this._player.TimeMuted > 0)
                    this._player.TimeMuted -= 60;
                #endregion

                #region Console Checks
                if (this._player.MessengerSpamTime > 0)
                    this._player.MessengerSpamTime -= 60;
                if (this._player.MessengerSpamTime <= 0)
                    this._player.MessengerSpamCount = 0;
                #endregion

                this._player.TimeAFK += 1;

                #region Respect checking
                if (this._player.GetStats().RespectsTimestamp != DateTime.Today.ToString("MM/dd"))
                {
                    this._player.GetStats().RespectsTimestamp = DateTime.Today.ToString("MM/dd");
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `user_stats` SET `dailyRespectPoints` = '" + this._player.RespetosForbi + "', `dailyPetRespectPoints` = '" + this._player.RespetosForbi + "', `respectsTimestamp` = '" + DateTime.Today.ToString("MM/dd") + "' WHERE `id` = '" + this._player.Id + "' LIMIT 1");
                    }

                    this._player.GetStats().DailyRespectPoints = this._player.RespetosForbi;
                    this._player.GetStats().DailyPetRespectPoints = this._player.RespetosForbi;

                    //SISTEMA FORBI (VA CON LA CMS)
                    if (this._player.Rank == 1)
                    {
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery("UPDATE `users` SET `loto_clicks` = '1' WHERE `id` = '" + this._player.Id + "' LIMIT 1");
                        }
                    }else{
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery("UPDATE `users` SET `loto_clicks` = '2' WHERE `id` = '" + this._player.Id + "' LIMIT 1");
                        }
                    }
                    
                    if (this._player.GetClient() != null)
                    {
                        this._player.GetClient().SendMessage(new UserObjectComposer(this._player));
                    }
                }
                #endregion

                #region Reset Scripting Warnings
                if (this._player.GiftPurchasingWarnings < 15)
                    this._player.GiftPurchasingWarnings = 0;

                if (this._player.MottoUpdateWarnings < 15)
                    this._player.MottoUpdateWarnings = 0;

                if (this._player.ClothingUpdateWarnings < 15)
                    this._player.ClothingUpdateWarnings = 0;
                #endregion


                if (this._player.GetClient() != null)
                    MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(this._player.GetClient(), "ACH_AllTimeHotelPresence", 1);

                this._player.CheckCreditsTimer();
                this._player.CheckDiamondsTimer();
                this._player.CheckBonusTimer();
                this._player.CheckhofTimer();
                this._player.Effects().CheckEffectExpiry(this._player);

                this._timerRunning = false;
                this._timerLagging = false;

                this._resetEvent.Set();
            }
            catch { }
        }

        /// <summary>
        /// Stops the timer and disposes everything.
        /// </summary>
        public void Dispose()
        {
            // Wait until any processing is complete first.
            try
            {
                this._resetEvent.WaitOne(TimeSpan.FromMinutes(5));
            }
            catch { } // give up

            // Set the timer to disabled
            this._disabled = true;

            // Dispose the timer to disable it.
            try
            {
                if (this._timer != null)
                    this._timer.Dispose();
            }
            catch { }

            // Remove reference to the timer.
            this._timer = null;

            // Null the player so we don't reference it here anymore
            this._player = null;
        }
    }
}