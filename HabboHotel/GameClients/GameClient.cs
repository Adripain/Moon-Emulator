using System;

using Moon.Net;
using System.Linq;
using Moon.Core;
using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;
using Moon.Communication.Interfaces;
using Moon.HabboHotel.Users.UserDataManagement;
using ConnectionManager;

using Moon.Communication.Packets.Outgoing.Sound;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.Communication.Packets.Outgoing.Handshake;
using Moon.Communication.Packets.Outgoing.Navigator;
using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Moon.Communication.Packets.Outgoing.Inventory.Achievements;


using Moon.Communication.Encryption.Crypto.Prng;
using Moon.HabboHotel.Users.Messenger.FriendBar;
using Moon.Communication.Packets.Outgoing.BuildersClub;
using Moon.HabboHotel.Moderation;

using Moon.Database.Interfaces;
using Moon.HabboHotel.Subscriptions;
using Moon.HabboHotel.Permissions;
using Moon.Communication.Packets.Outgoing;
using Moon.Communication.Packets.Outgoing.Nux;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using System.Collections.Generic;
using Moon.HabboHotel.Catalog;
using Moon.Communication.Packets.Outgoing.Talents;
using Moon.HabboHotel.Users.Messenger;
using System.Xml;
using System.Net;
using System.IO;
using Moon.Communication.Packets.Outgoing.Campaigns;
using Moon.Communication.Packets.Outgoing.Notifications;
using System.Threading;
using Moon.Communication.Packets.Outgoing.Messenger;
using Moon.Communication.Packets.Outgoing.Rooms.Furni;
using Moon.HabboHotel.Helpers;
using Moon.Communication.Packets.Outgoing.Help.Helpers;
using Moon.Utilities;
using System.Data;
using Moon.Communication.Packets.Incoming.LandingView;

namespace Moon.HabboHotel.GameClients
{
    public class GameClient
    {
        private readonly int _id;
        private Habbo _habbo;
        public string MachineId;
        private bool _disconnected;
        public string ssoTicket;
        public ARC4 RC4Client = null;
        private GamePacketParser _packetParser;
        private ConnectionInformation _connection;
        public int PingCount { get; set; }

        public Session wsSession;

        public GameClient(int ClientId, ConnectionInformation pConnection)
        {
            this._id = ClientId;
            this._connection = pConnection;
            this._packetParser = new GamePacketParser(this);
            this.PingCount = 0;
        }

        private void SwitchParserRequest()
        {
            _packetParser.SetConnection(_connection);
            _packetParser.onNewPacket += parser_onNewPacket;
            byte[] data = (_connection.parser as InitialPacketParser).currentData;
            _connection.parser.Dispose();
            _connection.parser = _packetParser;
            _connection.parser.handlePacketData(data);
        }

        private void parser_onNewPacket(ClientPacket Message)
        {
            try
            {
                MoonEnvironment.GetGame().GetPacketManager().TryExecutePacket(this, Message);
            }
            catch (Exception e)
            {
                Logging.LogPacketException(Message.ToString(), e.ToString());
            }
        }

        internal string SendNotification(string v1, string v2)
        {
            throw new NotImplementedException();
        }

        private void PolicyRequest()
        {
            _connection.SendData(MoonEnvironment.GetDefaultEncoding().GetBytes("<?xml version=\"1.0\"?>\r\n" +
                   "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
                   "<cross-domain-policy>\r\n" +
                   "<allow-access-from domain=\"*\" to-ports=\"1-31111\" />\r\n" +
                   "</cross-domain-policy>\x0"));
        }


        public void StartConnection()
        {
            if (_connection == null)
                return;

            this.PingCount = 0;

            (_connection.parser as InitialPacketParser).PolicyRequest += PolicyRequest;
            (_connection.parser as InitialPacketParser).SwitchParserRequest += SwitchParserRequest;
            _connection.startPacketProcessing();
        }

        public bool TryAuthenticate(string AuthTicket)
        {
            try
            {
                byte errorCode = 0;
                UserData userData = UserDataFactory.GetUserData(AuthTicket, out errorCode);
                if (errorCode == 1 || errorCode == 2)
                {
                    Disconnect();
                    return false;
                }

                #region Ban Checking
                //Let's have a quick search for a ban before we successfully authenticate..
                ModerationBan BanRecord = null;
                if (!string.IsNullOrEmpty(MachineId))
                {
                    if (MoonEnvironment.GetGame().GetModerationManager().IsBanned(MachineId, out BanRecord))
                    {
                        if (MoonEnvironment.GetGame().GetModerationManager().MachineBanCheck(MachineId))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }

                if (userData.user != null)
                {
                    //Now let us check for a username ban record..
                    BanRecord = null;
                    if (MoonEnvironment.GetGame().GetModerationManager().IsBanned(userData.user.Username, out BanRecord))
                    {
                        if (MoonEnvironment.GetGame().GetModerationManager().UsernameBanCheck(userData.user.Username))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }
                #endregion

                MoonEnvironment.GetGame().GetClientManager().RegisterClient(this, userData.userID, userData.user.Username);
                _habbo = userData.user;
                

                if (_habbo != null)
                {
                    this.ssoTicket = AuthTicket;
                    userData.user.Init(this, userData);

                    
                    SendMessage(new AuthenticationOKComposer());
                    SendMessage(new AvatarEffectsComposer(_habbo.Effects().GetAllEffects));
                    SendMessage(new NavigatorSettingsComposer(_habbo.HomeRoom));
                    SendMessage(new FavouritesComposer(userData.user.FavoriteRooms));
                    SendMessage(new FigureSetIdsComposer(_habbo.GetClothing().GetClothingAllParts));
                    SendMessage(new UserRightsComposer(_habbo));
                    SendMessage(new AvailabilityStatusComposer());
                    SendMessage(new AchievementScoreComposer(_habbo.GetStats().AchievementPoints));


                    var habboClubSubscription = new ServerPacket(ServerPacketHeader.HabboClubSubscriptionComposer);
                    habboClubSubscription.WriteString("club_habbo");
                    habboClubSubscription.WriteInteger(0);
                    habboClubSubscription.WriteInteger(0);
                    habboClubSubscription.WriteInteger(0);
                    habboClubSubscription.WriteInteger(2);
                    habboClubSubscription.WriteBoolean(false);
                    habboClubSubscription.WriteBoolean(false);
                    habboClubSubscription.WriteInteger(0);
                    habboClubSubscription.WriteInteger(0);
                    habboClubSubscription.WriteInteger(0);
                    SendMessage(habboClubSubscription);

                    SendMessage(new BuildersClubMembershipComposer());
                    SendMessage(new CfhTopicsInitComposer());

                    SendMessage(new BadgeDefinitionsComposer(MoonEnvironment.GetGame().GetAchievementManager()._achievements));
                    SendMessage(new SoundSettingsComposer(_habbo.ClientVolume, _habbo.ChatPreference, _habbo.AllowMessengerInvites, _habbo.FocusPreference, FriendBarStateUtility.GetInt(_habbo.FriendbarState)));
                    SendMessage(new TalentTrackLevelComposer());

                    if (GetHabbo().GetMessenger() != null)
                        GetHabbo().GetMessenger().OnStatusChanged(true);

                    if (_habbo.Rank < 2 && !MoonStaticGameSettings.HotelOpenForUsers)
                    {
                        SendMessage(new SendHotelAlertLinkEventComposer("Actualmente solo el Equipo Adminsitrativo puede entrar al hotel para comprobar que todo está bien antes de que los usuarios puedan entrar. Vuelve a intentarlo en unos minutos, podrás encontrar más información en nuestro Facebook.", MoonEnvironment.GetDBConfig().DBData["facebook_url"]));
                        Thread.Sleep(10000);
                        Disconnect();
                        return false;
                    }

                    GetHallOfFame.getInstance().Load();

                    if (!string.IsNullOrEmpty(MachineId))
                    {
                        if (this._habbo.MachineId != MachineId)
                        {
                            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `users` SET `machine_id` = @MachineId WHERE `id` = @id LIMIT 1");
                                dbClient.AddParameter("MachineId", MachineId);
                                dbClient.AddParameter("id", _habbo.Id);
                                dbClient.RunQuery();
                            }
                        }

                        _habbo.MachineId = MachineId;
                    }
                    PermissionGroup PermissionGroup = null;
                    if (MoonEnvironment.GetGame().GetPermissionManager().TryGetGroup(_habbo.Rank, out PermissionGroup))
                    {
                        if (!String.IsNullOrEmpty(PermissionGroup.Badge))
                            if (!_habbo.GetBadgeComponent().HasBadge(PermissionGroup.Badge))
                                _habbo.GetBadgeComponent().GiveBadge(PermissionGroup.Badge, true, this);
                    }

                    SubscriptionData SubData = null;
                    if (MoonEnvironment.GetGame().GetSubscriptionManager().TryGetSubscriptionData(this._habbo.VIPRank, out SubData))
                    {
                        if (!String.IsNullOrEmpty(SubData.Badge))
                        {
                            if (!_habbo.GetBadgeComponent().HasBadge(SubData.Badge))
                                _habbo.GetBadgeComponent().GiveBadge(SubData.Badge, true, this);
                        }
                    }

                    if (!MoonEnvironment.GetGame().GetCacheManager().ContainsUser(_habbo.Id))
                        MoonEnvironment.GetGame().GetCacheManager().GenerateUser(_habbo.Id);
                   
                    _habbo.InitProcess();

                    this.GetHabbo()._lastitems = new Dictionary<int, CatalogItem>();

                    if (MoonEnvironment.GetDBConfig().DBData["pin.system.enable"] == "0")
                        GetHabbo().StaffOk = true;

                    if(GetHabbo().StaffOk)
                    {
                        if (GetHabbo().GetPermissions().HasRight("mod_tickets"))
                        {
                              SendMessage(new ModeratorInitComposer(
                              MoonEnvironment.GetGame().GetModerationManager().UserMessagePresets,
                              MoonEnvironment.GetGame().GetModerationManager().RoomMessagePresets,
                              MoonEnvironment.GetGame().GetModerationManager().GetTickets));
                        }
                    }

                    if (GetHabbo().Rank >= 3)
                    {
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `ranks` WHERE id = '" + GetHabbo().Rank + "'");
                            DataRow Table = dbClient.getRow();

                            if (GetHabbo().GetBadgeComponent().HasBadge(Convert.ToString(Table["badgeid"])))
                            {

                            }
                            else
                            {

                                GetHabbo().GetBadgeComponent().GiveBadge(Convert.ToString(Table["badgeid"]), true, GetHabbo().GetClient());
                                SendMessage(RoomNotificationComposer.SendBubble("badge/" + Convert.ToString(Table["badgeid"]), "Você recebeu o emblema Staff!", "/inventory/open/badge"));
                            }
                        }
                    }

                    if (GetHabbo().Rank > 2 || GetHabbo()._guidelevel > 0)
                    {
                        HelperToolsManager.AddHelper(_habbo.GetClient(), false, true, true);
                        SendMessage(new HandleHelperToolComposer(true));
                    }

                    //SendMessage(new CampaignCalendarDataComposer(_habbo.calendarGift));
                    //if (int.Parse(MoonEnvironment.GetDBConfig().DBData["advent.calendar.enable"]) == 1) // Tk Custom By Whats
                    //    SendMessage(new MassEventComposer("openView/calendar"));

                    if (GetHabbo()._NUXROOM)
                    {
                        #region PREDESIGNED_ROOM BY KOMOK
                        if (1 > 0 && MoonEnvironment.GetGame().GetCatalog().GetPredesignedRooms().predesignedRoom.ContainsKey((uint)1))
                        {
                            #region SELECT ROOM AND CREATE NEW
                            var predesigned = MoonEnvironment.GetGame().GetCatalog().GetPredesignedRooms().predesignedRoom[(uint)1];
                            var decoration = predesigned.RoomDecoration;

                            var createRoom = MoonEnvironment.GetGame().GetRoomManager().CreateRoom(userData.user.GetClient(), "Territorio " + userData.user.GetClient().GetHabbo().Username, "¡Una Sala pre-decorada!", predesigned.RoomModel, 1, 25, 1);

                            createRoom.FloorThickness = int.Parse(decoration[0]);
                            createRoom.WallThickness = int.Parse(decoration[1]);
                            createRoom.Model.WallHeight = int.Parse(decoration[2]);
                            createRoom.Hidewall = ((decoration[3] == "True") ? 1 : 0);
                            createRoom.Wallpaper = decoration[4];
                            createRoom.Landscape = decoration[5];
                            createRoom.Floor = decoration[6];
                            var newRoom = MoonEnvironment.GetGame().GetRoomManager().LoadRoom(createRoom.Id);
                            #endregion

                            #region CREATE FLOOR ITEMS
                            if (predesigned.FloorItems != null)
                                foreach (var floorItems in predesigned.FloorItemData)
                                    using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                        dbClient.runFastQuery("INSERT INTO items (id, user_id, room_id, base_item, extra_data, x, y, z, rot, wall_pos, limited_number, limited_stack) VALUES (null, " + userData.user.GetClient().GetHabbo().Id + ", " + newRoom.RoomId + ", " + floorItems.BaseItem + ", '" + floorItems.ExtraData + "', " +
                                            floorItems.X + ", " + floorItems.Y + ", " + TextHandling.GetString(floorItems.Z) + ", " + floorItems.Rot + ", '', 0, 0);");
                            #endregion

                            #region CREATE WALL ITEMS
                            if (predesigned.WallItems != null)
                                foreach (var wallItems in predesigned.WallItemData)
                                    using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                        dbClient.runFastQuery("INSERT INTO items (id, user_id, room_id, base_item, extra_data, x, y, z, rot, wall_pos, limited_number, limited_stack) VALUES (null, " + userData.user.GetClient().GetHabbo().Id + ", " + newRoom.RoomId + ", " + wallItems.BaseItem + ", '" + wallItems.ExtraData +
                                            "', 0, 0, 0, 0, '" + wallItems.WallCoord + "', 0, 0);");
                            #endregion


                            #region GENERATE ROOM AND SEND PACKET
                            userData.user.GetClient().GetHabbo().GetInventoryComponent().UpdateItems(false);
                            MoonEnvironment.GetGame().GetRoomManager().LoadRoom(newRoom.Id).GetRoomItemHandler().LoadFurniture();
                            userData.user.GetClient().GetHabbo().HomeRoom = newRoom.Id;
                            var newFloorItems = newRoom.GetRoomItemHandler().GetFloor;
                            foreach (var roomItem in newFloorItems) newRoom.GetRoomItemHandler().SetFloorItem(roomItem, roomItem.GetX, roomItem.GetY, roomItem.GetZ);
                            var newWallItems = newRoom.GetRoomItemHandler().GetWall;
                            foreach (var roomItem in newWallItems) newRoom.GetRoomItemHandler().SetWallItem(userData.user.GetClient(), roomItem);
                            userData.user.GetClient().SendMessage(new FlatCreatedComposer(newRoom.Id, newRoom.Name));

                            Room Room = MoonEnvironment.GetGame().GetRoomManager().LoadRoom(newRoom.Id);
                            if (Room == null || !Room.CheckRights(userData.user.GetClient(), true))

                                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("UPDATE rooms SET state = 'open' WHERE `id` = '" + newRoom.Id + "' LIMIT 1");
                                    dbClient.RunQuery();
                                }
                            userData.user.GetClient().GetHabbo()._NUXROOM = false;
                            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE users SET nux_room = 'false' WHERE id = " + userData.user.GetClient().GetHabbo().Id + ";");
                                dbClient.RunQuery();
                            }

                            RoomAccess Access = RoomAccessUtility.ToRoomAccess("open");

                            Room.Access = Access;
                            Room.RoomData.Access = Access;
                            #endregion
                        }
                        #endregion
                    }

                    if (GetHabbo()._NUX)
                    {
                        var nuxStatus = new ServerPacket(ServerPacketHeader.NuxUserStatus);
                        nuxStatus.WriteInteger(2);
                        SendMessage(nuxStatus);

                    }

                    if (MoonEnvironment.GetGame().GetTargetedOffersManager().TargetedOffer != null)
                    {
                        MoonEnvironment.GetGame().GetTargetedOffersManager().Initialize(MoonEnvironment.GetDatabaseManager().GetQueryReactor());
                        TargetedOffers TargetedOffer = MoonEnvironment.GetGame().GetTargetedOffersManager().TargetedOffer;

                        if (TargetedOffer.Expire > MoonEnvironment.GetIUnixTimestamp())
                        {

                            if (TargetedOffer.Limit != GetHabbo()._TargetedBuy)
                            {

                                SendMessage(MoonEnvironment.GetGame().GetTargetedOffersManager().TargetedOffer.Serialize());
                            }
                        }
                        else if (TargetedOffer.Expire == -1)
                        {

                            if (TargetedOffer.Limit != GetHabbo()._TargetedBuy)
                            {

                                SendMessage(MoonEnvironment.GetGame().GetTargetedOffersManager().TargetedOffer.Serialize());
                            }
                        }
                        else
                        {
                            using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                dbClient.runFastQuery("UPDATE targeted_offers SET active = 'false'");
                            using (var dbClient2 = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                dbClient2.runFastQuery("UPDATE users SET targeted_buy = '0' WHERE targeted_buy > 0");
                        }
                    }

                    if (_habbo.MysticBoxes.Count == 0 && _habbo.MysticKeys.Count == 0)
                    {
                        var box = RandomNumber.GenerateRandom(1, 8);
                        string boxcolor = "";
                        switch (box)
                        {
                            case 1:
                                boxcolor = "purple";
                                break;
                            case 2:
                                boxcolor = "blue";
                                break;
                            case 3:
                                boxcolor = "green";
                                break;
                            case 4:
                                boxcolor = "yellow";
                                break;
                            case 5:
                                boxcolor = "lilac";
                                break;
                            case 6:
                                boxcolor = "orange";
                                break;
                            case 7:
                                boxcolor = "turquoise";
                                break;
                            case 8:
                                boxcolor = "red";
                                break;
                        }

                        var key = RandomNumber.GenerateRandom(1, 8);
                        string keycolor = "";
                        switch (key)
                        {
                            case 1:
                                keycolor = "purple";
                                break;
                            case 2:
                                keycolor = "blue";
                                break;
                            case 3:
                                keycolor = "green";
                                break;
                            case 4:
                                keycolor = "yellow";
                                break;
                            case 5:
                                keycolor = "lilac";
                                break;
                            case 6:
                                keycolor = "orange";
                                break;
                            case 7:
                                keycolor = "turquoise";
                                break;
                            case 8:
                                keycolor = "red";
                                break;
                        }

                        _habbo.MysticKeys.Add(keycolor);
                        _habbo.MysticBoxes.Add(boxcolor);

                        using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                            dbClient.runFastQuery("INSERT INTO user_mystic_data(user_id, mystic_keys, mystic_boxes) VALUES(" + GetHabbo().Id + ", '" + keycolor + "', '" + boxcolor +"');");
                    }

                    SendMessage(new MysteryBoxDataComposer(_habbo.GetClient()));

                    SendMessage(new HCGiftsAlertComposer());

                    MoonEnvironment.GetGame().GetRewardManager().CheckRewards(this);
                    MoonEnvironment.GetGame().GetAchievementManager().TryProgressHabboClubAchievements(this);
                    MoonEnvironment.GetGame().GetAchievementManager().TryProgressRegistrationAchievements(this);
                    MoonEnvironment.GetGame().GetAchievementManager().TryProgressLoginAchievements(this);

                    ICollection<MessengerBuddy> Friends = new List<MessengerBuddy>();
                    foreach (MessengerBuddy Buddy in this.GetHabbo().GetMessenger().GetFriends().ToList())
                    {
                        if (Buddy == null)
                            continue;

                        GameClient Friend = MoonEnvironment.GetGame().GetClientManager().GetClientByUserID(Buddy.Id);
                        if (Friend == null)
                            continue;
                        string figure = this.GetHabbo().Look;


                        Friend.SendMessage(RoomNotificationComposer.SendBubble("fig/" + figure, "Seu amig@ " + this.GetHabbo().Username + " entrou no hotel!", ""));
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                Logging.LogCriticalException("Bug during user login: " + e);
            }
            return false;
        }

        public void SendWhisper(string Message, int Colour = 0)
        {
            if (this == null || GetHabbo() == null || GetHabbo().CurrentRoom == null)
                return;

            RoomUser User = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Username);
            if (User == null)
                return;

            SendMessage(new WhisperComposer(User.VirtualId, Message, 0, (Colour == 0 ? User.LastBubble : Colour)));
        }

        public void SendChat(string Message, int Colour = 0)
        {
            if (this == null || GetHabbo() == null || GetHabbo().CurrentRoom == null)
                return;

            RoomUser User = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Username);
            if (User == null)
                return;

            SendMessage(new ChatComposer(User.VirtualId, Message, 0, (Colour == 0 ? User.LastBubble : Colour)));
        }

        public void SendNotification(string Message)
        {
            SendMessage(new BroadcastMessageAlertComposer(Message));
        }

        public void SendMessage(IServerPacket Message)
        {
            byte[] bytes = Message.GetBytes();

            if (Message == null)
                return;

            if (GetConnection() == null)
                return;

            GetConnection().SendData(bytes);
        }

        public void SendShout(string Message, int Colour = 0)
        {
            if (this == null || GetHabbo() == null || GetHabbo().CurrentRoom == null)
                return;

            RoomUser User = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Username);
            if (User == null)
                return;

            SendMessage(new ShoutComposer(User.VirtualId, Message, 0, (Colour == 0 ? User.LastBubble : Colour)));
        }

        public int ConnectionID
        {
            get { return _id; }
        }

        public ConnectionInformation GetConnection()
        {
            return _connection;
        }

        public Habbo GetHabbo()
        {
            return _habbo;
        }

        public void Disconnect()
        {
            try
            {
                if (GetHabbo() != null)
                {
                    ICollection<MessengerBuddy> Friends = new List<MessengerBuddy>();
                    foreach (MessengerBuddy Buddy in this.GetHabbo().GetMessenger().GetFriends().ToList())
                    {
                        if (Buddy == null)
                            continue;

                        GameClient Friend = MoonEnvironment.GetGame().GetClientManager().GetClientByUserID(Buddy.Id);
                        if (Friend == null)
                            continue;
                        string figure = this.GetHabbo().Look;


                        Friend.SendMessage(RoomNotificationComposer.SendBubble("fig/" + figure, "Seu amig@ " + this.GetHabbo().Username + " entrou no hotel!", ""));

                    }
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery(GetHabbo().GetQueryString);
                    }

                    GetHabbo().OnDisconnect();
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e.ToString());
            }


            if (!_disconnected)
            {
                if (_connection != null)
                    _connection.Dispose();
                _disconnected = true;
            }
        }

        public void Dispose()
        {
            if (GetHabbo() != null)
                GetHabbo().OnDisconnect();

            this.MachineId = string.Empty;
            this._disconnected = true;
            this._habbo = null;
            this._connection = null;
            this.RC4Client = null;
            this._packetParser = null;
        }

        public class Meteorologia
        {
            public string ApiVersion { get; set; }
            public Data Data { get; set; }
        }

        public class Data
        {
            public string Location { get; set; }
            public string Temperature { get; set; }
            public string Skytext { get; set; }
            public string Humidity { get; set; }
            public string Wind { get; set; }
            public string Date { get; set; }
            public string Day { get; set; }
        }

        public RoomUser GetRoomUser()
        {
            RoomUser RUser = null;
            try
            {
                if (this == null || GetHabbo() == null || GetHabbo().CurrentRoom == null)
                    return null;

                RUser = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Id);
            }
            catch
            {
                return RUser;
            }
            return RUser;
        }
    }
}