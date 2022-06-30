using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Moon.Core;

using Moon.HabboHotel;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Users;
using Moon.HabboHotel.Users.UserDataManagement;

using Moon.Net;
using Moon.Utilities;
using log4net;

using System.Collections.Concurrent;
using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.Communication.Encryption.Keys;
using Moon.Communication.Encryption;

using Moon.Database.Interfaces;
using Moon.HabboHotel.Cache;
using Moon.Database;
using Moon.Communication.Packets.Outgoing.Notifications;
using System.Reflection;
using System.Diagnostics;

namespace Moon
{
    public static class MoonEnvironment
    {
        private static readonly ILog log = LogManager.GetLogger("Moon.MoonEnvironment");

        public const string PrettyVersion = "MOON Server";
        public const string PrettyBuild = "1.0";
        public const string Credits = "Desenvolvido por: KaioVDEV";
        public static string HotelName;

        private static ConfigurationData _configuration;
        private static Encoding _defaultEncoding;
        private static ConnectionHandling _connectionManager;
        private static Game _game;
        private static DatabaseManager _manager;
        public static ConfigData ConfigData;
        public static MusSocket MusSystem;
        public static CultureInfo CultureInfo;

        public static bool Event = false;
        public static DateTime lastEvent;
        public static DateTime ServerStarted;

        private static readonly List<char> Allowedchars = new List<char>(new[]
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',
                'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
                'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '.'
            });

        private static ConcurrentDictionary<int, Habbo> _usersCached = new ConcurrentDictionary<int, Habbo>();
        

        public static string SWFRevision = "";

        public static void Initialize()
        {
            string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
            ServerStarted = DateTime.Now;
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(@"          " + @"::::    ::::   ::::::::   ::::::::  ::::    :::");
            Console.WriteLine(@"          " + @"+:+:+: :+:+:+ :+:    :+: :+:    :+: :+:+:   :+:");
            Console.WriteLine(@"          " + @"+:+ +:+:+ +:+ +:+    +:+ +:+    +:+ :+:+:+  +:+");
            Console.WriteLine(@"          " + @"+#+  +:+  +#+ +#+    +:+ +#+    +:+ +#+ +:+ +#+       "+ Credits); 
            Console.WriteLine(@"          " + @"+#+       +#+ +#+    +#+ +#+    +#+ +#+  +#+#+#");
            Console.WriteLine(@"          " + @"#+#       #+# #+#    #+# #+#    #+# #+#   #+#+#"); 
            Console.WriteLine(@"          " + @"###       ###  ########   ########  ###    #### ");
            Console.WriteLine("");
            Console.WriteLine(@"-------------------------------------------------------------------------------");
            Console.WriteLine("");
            Console.Title = "Carregando MOON Server...";
            _defaultEncoding = Encoding.Default;

            Console.WriteLine("");
            Console.WriteLine("");

            CultureInfo = CultureInfo.CreateSpecificCulture("en-GB");

            try
            {

                _configuration = new ConfigurationData(Path.Combine(Application.StartupPath, @"config.ini"));

                var connectionString = new MySqlConnectionStringBuilder
                {
                    ConnectionTimeout = 10,
                    Database = GetConfig().data["db.name"],
                    DefaultCommandTimeout = 30,
                    Logging = false,
                    MaximumPoolSize = uint.Parse(GetConfig().data["db.pool.maxsize"]),
                    MinimumPoolSize = uint.Parse(GetConfig().data["db.pool.minsize"]),
                    Password = GetConfig().data["db.password"],
                    Pooling = true,
                    Port = uint.Parse(GetConfig().data["db.port"]),
                    Server = GetConfig().data["db.hostname"],
                    UserID = GetConfig().data["db.username"],
                    AllowZeroDateTime = true,
                    ConvertZeroDateTime = true,
                };

                _manager = new DatabaseManager(connectionString.ToString());

                if (!_manager.IsConnected())
                {
                    Console.WriteLine(CurrentTime + "» Houve um erro para se conectar ao banco de dados.");
                    Console.ReadKey(true);
                    Environment.Exit(1);
                    return;
                }
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(CurrentTime + "» Conectado ao banco de dados.");

                HotelName = Convert.ToString(GetConfig().data["hotel.name"]);

                //Reset our statistics first.
                using (IQueryAdapter dbClient = GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("TRUNCATE `catalog_marketplace_data`");
                    dbClient.RunQuery("UPDATE `rooms` SET `users_now` = '0' WHERE `users_now` > '0';");
                    dbClient.RunQuery("UPDATE `users` SET `online` = '0' WHERE `online` = '1'");
                    dbClient.RunQuery("UPDATE `server_status` SET `users_online` = '0', `loaded_rooms` = '0'");
                }

                //Get the configuration & Game set.
                ConfigData = new ConfigData();
                _game = new Game();

                //Have our encryption ready.
                HabboEncryptionV2.Initialize(new RSAKeys());

                //Make sure MUS is working.
                MusSystem = new MusSocket(GetConfig().data["mus.tcp.bindip"], int.Parse(GetConfig().data["mus.tcp.port"]), GetConfig().data["mus.tcp.allowedaddr"].Split(Convert.ToChar(";")), 0);

                //Accept connections.
                _connectionManager = new ConnectionHandling(int.Parse(GetConfig().data["game.tcp.port"]), int.Parse(GetConfig().data["game.tcp.conlimit"]), int.Parse(GetConfig().data["game.tcp.conperip"]), GetConfig().data["game.tcp.enablenagles"].ToLower() == "true");
                _connectionManager.init();

                _game.StartGameLoop();

                TimeSpan TimeUsed = DateTime.Now - ServerStarted;

                Console.WriteLine();

                Logging.WriteLine(">> MOON Server -> Pronto para jogar! (" + TimeUsed.Seconds + " s, " + TimeUsed.Milliseconds + " ms)", ConsoleColor.DarkGray);
            }
            catch (KeyNotFoundException e)
            {
                Logging.WriteLine("Please check your configuration file - some values appear to be missing.", ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...");
                Logging.WriteLine(e.ToString());
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
            catch (InvalidOperationException e)
            {
                Logging.WriteLine("Failed to initialize KeyEmulator: " + e.Message, ConsoleColor.Red);
                Logging.WriteLine("Press any key to shut down ...");
                Console.ReadKey(true);
                Environment.Exit(1);
                return;
            }
            catch (Exception e)
            {
                Logging.WriteLine("Fatal error during startup: " + e, ConsoleColor.Red);
                Logging.WriteLine("Press a key to exit");

                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        public static bool EnumToBool(string Enum)
        {
            return (Enum == "1");
        }

        public static string BoolToEnum(bool Bool)
        {
            return (Bool == true ? "1" : "0");
        }

        public static int GetRandomNumber(int Min, int Max)
        {
            return RandomNumber.GenerateNewRandom(Min, Max);
        }
        

        public static double GetUnixTimestamp()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return ts.TotalSeconds;
        }

        internal static int GetIUnixTimestamp()
        {
            var ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            var unixTime = ts.TotalSeconds;
            return Convert.ToInt32(unixTime);
        }

        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

        public static long Now()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            double unixTime = ts.TotalMilliseconds;
            return (long)unixTime;
        }

        public static string FilterFigure(string figure)
        {
            foreach (char character in figure)
            {
                if (!isValid(character))
                    return "sh-3338-93.ea-1406-62.hr-831-49.ha-3331-92.hd-180-7.ch-3334-93-1408.lg-3337-92.ca-1813-62";
            }

            return figure;
        }

        internal static bool IsNum(string Int)
        {
            double Num;
            bool isNum = double.TryParse(Int, out Num);
            if (isNum)
                return true;
            else
                return false;
        }

        private static bool isValid(char character)
        {
            return Allowedchars.Contains(character);
        }

        public static bool IsValidAlphaNumeric(string inputStr)
        {
            inputStr = inputStr.ToLower();
            if (string.IsNullOrEmpty(inputStr))
            {
                return false;
            }

            for (int i = 0; i < inputStr.Length; i++)
            {
                if (!isValid(inputStr[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetUsernameById(int UserId)
        {
            string Name = "Unknown User";

            GameClient Client = GetGame().GetClientManager().GetClientByUserID(UserId);
            if (Client != null && Client.GetHabbo() != null)
                return Client.GetHabbo().Username;

            UserCache User = MoonEnvironment.GetGame().GetCacheManager().GenerateUser(UserId);
            if (User != null)
                return User.Username;

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `username` FROM `users` WHERE id = @id LIMIT 1");
                dbClient.AddParameter("id", UserId);
                Name = dbClient.getString();
            }

            if (string.IsNullOrEmpty(Name))
                Name = "Unknown User";

            return Name;
        }

        public static string RainbowT()
        {
            int numColorst = 1000;
            var colorst = new List<string>();
            var randomt = new Random();
            for (int i = 0; i < numColorst; i++)
            {
                colorst.Add(String.Format("#{0:X6}", randomt.Next(0x1000000)));
            }

            int indext = 0;
            string rainbowt = colorst[indext];

            if (indext > numColorst)
                indext = 0;
            else
                indext++;

            return rainbowt;
        }

        public static Habbo GetHabboById(int UserId)
        {
            try
            {
                GameClient Client = GetGame().GetClientManager().GetClientByUserID(UserId);
                if (Client != null)
                {
                    Habbo User = Client.GetHabbo();
                    if (User != null && User.Id > 0)
                    {
                        if (_usersCached.ContainsKey(UserId))
                            _usersCached.TryRemove(UserId, out User);
                        return User;
                    }
                }
                else
                {
                    try
                    {
                        if (_usersCached.ContainsKey(UserId))
                            return _usersCached[UserId];
                        else
                        {
                            UserData data = UserDataFactory.GetUserData(UserId);
                            if (data != null)
                            {
                                Habbo Generated = data.user;
                                if (Generated != null)
                                {
                                    Generated.InitInformation(data);
                                    _usersCached.TryAdd(UserId, Generated);
                                    return Generated;
                                }
                            }
                        }
                    }
                    catch { return null; }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static Habbo GetHabboByUsername(String UserName)
        {
            try
            {
                using (IQueryAdapter dbClient = GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT `id` FROM `users` WHERE `username` = @user LIMIT 1");
                    dbClient.AddParameter("user", UserName);
                    int id = dbClient.getInteger();
                    if (id > 0)
                        return GetHabboById(Convert.ToInt32(id));
                }
                return null;
            }
            catch { return null; }
        }



        internal static void PerformShutDown()
        {
            PerformShutDown(false);
        }


        internal static void PerformRestart()
        {
            PerformShutDown(true);
        }

        public static void PerformShutDown(bool restart)
        {
            Console.Clear();
            log.Info("Moon Server --> Fechando");
            Console.Title = "Moon Server: Desligando!";

            GetGame().StopGameLoop();
            Thread.Sleep(2500);
            GetConnectionManager().Destroy();//Stop listening.
            GetGame().GetPacketManager().UnregisterAll();//Unregister the packets.
            GetGame().GetPacketManager().WaitForAllToComplete();
            GetGame().GetClientManager().CloseAll();//Close all connections
            GetGame().GetRoomManager().Dispose();//Stop the game loop.

            using (IQueryAdapter dbClient = _manager.GetQueryReactor())
            {
                dbClient.RunQuery("TRUNCATE `catalog_marketplace_data`");
                dbClient.RunQuery("UPDATE `users` SET online = '0'");
                dbClient.RunQuery("TRUNCATE `user_auth_ticket`");
                dbClient.RunQuery("UPDATE `rooms` SET `users_now` = '0' WHERE `users_now` > '0'");
                dbClient.RunQuery("UPDATE `server_status` SET `users_online` = '0', `loaded_rooms` = '0'");
            }

            log.Info("Moon sessão encerrada.");

            if (restart) Process.Start(Assembly.GetEntryAssembly().Location);
            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        public static ConfigurationData GetConfig()
        {
            return _configuration;
        }

        public static ConfigData GetDBConfig()
        {
            return ConfigData;
        }

        public static Encoding GetDefaultEncoding()
        {
            return _defaultEncoding;
        }

        public static ConnectionHandling GetConnectionManager()
        {
            return _connectionManager;
        }

        public static Game GetGame()
        {
            return _game;
        }

        public static DatabaseManager GetDatabaseManager()
        {
            return _manager;
        }

        public static ICollection<Habbo> GetUsersCached()
        {
            return _usersCached.Values;
        }

        public static bool RemoveFromCache(int Id, out Habbo Data)
        {
            return _usersCached.TryRemove(Id, out Data);
        }
    }
}