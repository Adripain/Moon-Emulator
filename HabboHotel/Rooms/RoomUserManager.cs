﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;

using Moon.Communication.Packets.Outgoing.Rooms.Avatar;
using Moon.Core;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Items;
using Moon.HabboHotel.Global;
using Moon.HabboHotel.Pathfinding;
using Moon.HabboHotel.Rooms.AI;
using Moon.HabboHotel.Quests;
using Moon.HabboHotel.Rooms.Games;
using Moon.HabboHotel.Astar;
using Moon.HabboHotel.Users;
using Moon.HabboHotel.Users.Inventory;
using Moon.Communication.Packets.Incoming;

using Moon.Utilities;

using System.Data;
using Moon.Communication.Packets.Outgoing.Rooms.Session;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;
using Moon.Communication.Packets.Outgoing.Rooms.Permissions;
using Moon.Communication.Packets.Outgoing.Handshake;
using System.Text.RegularExpressions;
using Moon.HabboHotel.Rooms.Games.Teams;

using Moon.Database.Interfaces;
using System.Threading.Tasks;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms
{
    public class RoomUserManager
    {
        private Room _room;
        private ConcurrentDictionary<int, RoomUser> _users;
        private ConcurrentDictionary<int, RoomUser> _bots;
        private ConcurrentDictionary<int, RoomUser> _pets;

        private int primaryPrivateUserID;
        private int secondaryPrivateUserID;

        public int userCount;
        private int petCount;


        public RoomUserManager(Room room)
        {
            this._room = room;
            this._users = new ConcurrentDictionary<int, RoomUser>();
            this._pets = new ConcurrentDictionary<int, RoomUser>();
            this._bots = new ConcurrentDictionary<int, RoomUser>();

            this.primaryPrivateUserID = 0;
            this.secondaryPrivateUserID = 0;

            this.petCount = 0;
            this.userCount = 0;
        }

        public void Dispose()
        {
            this._users.Clear();
            this._pets.Clear();
            this._bots.Clear();

            this._users = null;
            this._pets = null;
            this._bots = null;
        }

        public RoomUser DeployBot(RoomBot Bot, Pet PetData)
        {
            var BotUser = new RoomUser(0, _room.RoomId, primaryPrivateUserID++, _room);
            Bot.VirtualId = primaryPrivateUserID;

            int PersonalID = secondaryPrivateUserID++;
            BotUser.InternalRoomID = PersonalID;
            _users.TryAdd(PersonalID, BotUser);

            DynamicRoomModel Model = _room.GetGameMap().Model;

            if ((Bot.X > 0 && Bot.Y > 0) && Bot.X < Model.MapSizeX && Bot.Y < Model.MapSizeY)
            {
                BotUser.SetPos(Bot.X, Bot.Y, Bot.Z);
                BotUser.SetRot(Bot.Rot, false);
            }
            else
            {
                Bot.X = Model.DoorX;
                Bot.Y = Model.DoorY;

                BotUser.SetPos(Model.DoorX, Model.DoorY, Model.DoorZ);
                BotUser.SetRot(Model.DoorOrientation, false);
            }

            BotUser.BotData = Bot;
            BotUser.BotAI = Bot.GenerateBotAI(BotUser.VirtualId);

            if (BotUser.IsPet)
            {
                BotUser.BotAI.Init(Bot.BotId, BotUser.VirtualId, _room.RoomId, BotUser, _room);
                BotUser.PetData = PetData;
                BotUser.PetData.VirtualId = BotUser.VirtualId;
            }
            else
                BotUser.BotAI.Init(Bot.BotId, BotUser.VirtualId, _room.RoomId, BotUser, _room);

            //UpdateUserStatus(BotUser, false);
            BotUser.UpdateNeeded = true;

            _room.SendMessage(new UsersComposer(BotUser));

            if (BotUser.IsPet)
            {
                if (_pets.ContainsKey(BotUser.PetData.PetId)) //Pet allready placed
                    _pets[BotUser.PetData.PetId] = BotUser;
                else
                    _pets.TryAdd(BotUser.PetData.PetId, BotUser);

                petCount++;
            }
            else if (BotUser.IsBot)
            {
                if (_bots.ContainsKey(BotUser.BotData.BotId))
                    _bots[BotUser.BotData.BotId] = BotUser;
                else
                    _bots.TryAdd(BotUser.BotData.Id, BotUser);
                _room.SendMessage(new DanceComposer(BotUser, BotUser.BotData.DanceId));
            }
            return BotUser;
        }

        public void RemoveBot(int VirtualId, bool Kicked)
        {
            RoomUser User = GetRoomUserByVirtualId(VirtualId);
            if (User == null || !User.IsBot)
                return;

            if (User.IsPet)
            {
                RoomUser PetRemoval = null;

                _pets.TryRemove(User.PetData.PetId, out PetRemoval);
                petCount--;
            }
            else
            {
                RoomUser BotRemoval = null;
                _bots.TryRemove(User.BotData.Id, out BotRemoval);
            }



            User.BotAI.OnSelfLeaveRoom(Kicked);

            _room.SendMessage(new UserRemoveComposer(User.VirtualId));

            RoomUser toRemove;

            if (_users != null)
                _users.TryRemove(User.InternalRoomID, out toRemove);

            onRemove(User);
        }

        public RoomUser GetUserForSquare(int x, int y)
        {
            return _room.GetGameMap().GetRoomUsers(new Point(x, y)).FirstOrDefault();
        }

        public bool AddAvatarToRoom(GameClient Session)
        {
            if (_room == null)
                return false;

            if (Session == null)
                return false;

            if (Session.GetHabbo().CurrentRoom == null)
                return false;

            RoomUser User = new RoomUser(Session.GetHabbo().Id, _room.RoomId, primaryPrivateUserID++, _room);

            if (User == null || User.GetClient() == null)
                return false;

            User.UserId = Session.GetHabbo().Id;
            Session.GetHabbo().TentId = 0;

            int PersonalID = secondaryPrivateUserID++;
            User.InternalRoomID = PersonalID;

            Session.GetHabbo().CurrentRoomId = _room.RoomId;

            if (!this._users.TryAdd(PersonalID, User))
                return false;

            DynamicRoomModel Model = _room.GetGameMap().Model;
            if (Model == null)
                return false;

            if (!_room.PetMorphsAllowed && Session.GetHabbo().PetId != 0)
                Session.GetHabbo().PetId = 0;

            if (!Session.GetHabbo().IsTeleporting && !Session.GetHabbo().IsHopping)
            {
                if (!Model.DoorIsValid())
                {
                    Point Square = _room.GetGameMap().getRandomWalkableSquare();
                    Model.DoorX = Square.X;
                    Model.DoorY = Square.Y;
                    Model.DoorZ = _room.GetGameMap().GetHeightForSquareFromData(Square);
                }

                User.SetPos(Model.DoorX, Model.DoorY, Model.DoorZ);
                User.SetRot(Model.DoorOrientation, false);
            }
            else if (!User.IsBot && (User.GetClient().GetHabbo().IsTeleporting || User.GetClient().GetHabbo().IsHopping))
            {
                Item Item = null;
                if (Session.GetHabbo().IsTeleporting)
                    Item = _room.GetRoomItemHandler().GetItem(Session.GetHabbo().TeleporterId);
                else if (Session.GetHabbo().IsHopping)
                    Item = _room.GetRoomItemHandler().GetItem(Session.GetHabbo().HopperId);

                if (Item != null)
                {
                    if (Session.GetHabbo().IsTeleporting)
                    {
                        Item.ExtraData = "2";
                        Item.UpdateState(false, true);
                        User.SetPos(Item.GetX, Item.GetY, Item.GetZ);
                        User.SetRot(Item.Rotation, false);
                        Item.InteractingUser2 = Session.GetHabbo().Id;
                        Item.ExtraData = "0";
                        Item.UpdateState(false, true);
                    }
                    else if (Session.GetHabbo().IsHopping)
                    {
                        Item.ExtraData = "1";
                        Item.UpdateState(false, true);
                        User.SetPos(Item.GetX, Item.GetY, Item.GetZ);
                        User.SetRot(Item.Rotation, false);
                        User.AllowOverride = false;
                        Item.InteractingUser2 = Session.GetHabbo().Id;
                        Item.ExtraData = "2";
                        Item.UpdateState(false, true);
                    }
                }
                else
                {
                    User.SetPos(Model.DoorX, Model.DoorY, Model.DoorZ - 1);
                    User.SetRot(Model.DoorOrientation, false);
                }
            }
            if (!Session.GetHabbo().Spectating) { _room.SendMessage(new UsersComposer(User)); }

            if (!Session.GetHabbo().Spectating)
            {
                if (_room.CheckRights(Session, true))
                {
                    User.SetStatus("flatctrl", "useradmin");
                    Session.SendMessage(new YouAreOwnerComposer());
                    Session.SendMessage(new YouAreControllerComposer(4));
                }
                else if (_room.CheckRights(Session, false) && _room.Group == null)
                {
                    User.SetStatus("flatctrl", "1");
                    Session.SendMessage(new YouAreControllerComposer(1));
                }
                else if (_room.Group != null && _room.CheckRights(Session, false, true))
                {
                    User.SetStatus("flatctrl", "3");
                    Session.SendMessage(new YouAreControllerComposer(3));
                }
                else
                    Session.SendMessage(new YouAreNotControllerComposer());

                User.UpdateNeeded = true;
            }

            if (Session.GetHabbo().GetPermissions().HasRight("autoeffect_102") && !Session.GetHabbo().DisableForcedEffects)
                Session.GetHabbo().Effects().ApplyEffect(102);
            if (Session.GetHabbo().GetPermissions().HasRight("autoeffect_178") && !Session.GetHabbo().DisableForcedEffects)
                Session.GetHabbo().Effects().ApplyEffect(178);
            if (Session.GetHabbo()._guidelevel == 1 && _room.Id == 72 && !Session.GetHabbo().DisableForcedEffects)
                Session.GetHabbo().Effects().ApplyEffect(597);
            if (Session.GetHabbo()._guidelevel == 2 && _room.Id == 72 && !Session.GetHabbo().DisableForcedEffects)
                Session.GetHabbo().Effects().ApplyEffect(595);

            if (_room.ForSale && _room.SalePrice > 0 && (_room.GetRoomUserManager().GetRoomUserByHabbo(_room.OwnerName) != null))
            {
                Session.SendWhisper("Esta Sala esta en venta, en  " + _room.SalePrice + " duckets. Escribe :buyroom si deseas comprarla!");
            }
            else if (_room.ForSale && _room.GetRoomUserManager().GetRoomUserByHabbo(_room.OwnerName) == null)
            {
                foreach (RoomUser _User in _room.GetRoomUserManager().GetRoomUsers())
                {
                    if (_User.GetClient() != null && _User.GetClient().GetHabbo() != null && _User.GetClient().GetHabbo().Id != Session.GetHabbo().Id)
                    {
                        _User.GetClient().SendWhisper("Esta Sala ya no se encuentra a la venta.");
                    }
                }
                _room.ForSale = false;
                _room.SalePrice = 0;
            }

            foreach (RoomUser Bot in this._bots.Values.ToList())
            {
                if (Bot == null || Bot.BotAI == null)
                    continue;

                Bot.BotAI.OnUserEnterRoom(User);

            }

            /*  if (User.GetClient().GetHabbo().Spectating)
              {
                  this.userCount--;
                  User.Dispose();
              }
              */
            return true;
        }

        public bool AddSpectatorToRoom(GameClient Session)
        {
            if (_room == null)
                return false;

            if (Session == null)
                return false;

            if (Session.GetHabbo().CurrentRoom == null)
                return false;

            RoomUser User = new RoomUser(Session.GetHabbo().Id, _room.RoomId, primaryPrivateUserID++, _room);

            if (User == null || User.GetClient() == null)
                return false;

            DynamicRoomModel Model = _room.GetGameMap().Model;
            if (Model == null)
                return false;


            foreach (RoomUser Bot in this._bots.Values.ToList())
            {
                if (Bot == null || Bot.BotAI == null)
                    continue;

                Bot.BotAI.OnUserEnterRoom(User);

            }

            return true;
        }

        public void RemoveUserFromRoom(GameClient Session, Boolean NotifyClient, Boolean NotifyKick = false)
        {
            try
            {
                if (_room == null)
                    return;

                if (Session == null || Session.GetHabbo() == null)
                    return;

                if (NotifyKick)
                    Session.SendMessage(new GenericErrorComposer(4008));

                if (NotifyClient)
                    Session.SendMessage(new CloseConnectionComposer());

                if (Session.GetHabbo().TentId > 0)
                    Session.GetHabbo().TentId = 0;

                RoomUser User = GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (User != null)
                {
                    if (User.RidingHorse)
                    {
                        User.RidingHorse = false;
                        RoomUser UserRiding = GetRoomUserByVirtualId(User.HorseID);
                        if (UserRiding != null)
                        {
                            UserRiding.RidingHorse = false;
                            UserRiding.HorseID = 0;
                        }
                    }

                    if (User.Team != TEAM.NONE)
                    {
                        TeamManager Team = this._room.GetTeamManagerForFreeze();
                        if (Team != null)
                        {
                            Team.OnUserLeave(User);

                            User.Team = TEAM.NONE;

                            if (User.GetClient().GetHabbo().Effects().CurrentEffect != 0)
                                User.GetClient().GetHabbo().Effects().ApplyEffect(0);
                        }
                    }


                    RemoveRoomUser(User);

                    if (User.CurrentItemEffect != ItemEffectType.NONE)
                    {
                        if (Session.GetHabbo().Effects() != null)
                            Session.GetHabbo().Effects().CurrentEffect = -1;
                    }

                    if (_room != null)
                    {
                        if (_room.HasActiveTrade(Session.GetHabbo().Id))
                            _room.TryStopTrade(Session.GetHabbo().Id);
                    }

                    //Session.GetHabbo().CurrentRoomId = 0;

                    if (Session.GetHabbo().GetMessenger() != null)
                        Session.GetHabbo().GetMessenger().OnStatusChanged(true);

                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.runFastQuery("UPDATE user_roomvisits SET exit_timestamp = '" + MoonEnvironment.GetUnixTimestamp() + "' WHERE room_id = '" + _room.RoomId + "' AND user_id = '" + Session.GetHabbo().Id + "' ORDER BY exit_timestamp DESC LIMIT 1");
                        dbClient.runFastQuery("UPDATE `rooms` SET `users_now` = '" + _room.UsersNow + "' WHERE `id` = '" + _room.RoomId + "' LIMIT 1");
                    }

                    if (User != null)
                        User.Dispose();
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e.ToString());
            }

            if (Session.GetHabbo() != null && Session.GetHabbo().Id == _room.OwnerId && _room.ForSale)
            {
                _room.ForSale = false;
                _room.SalePrice = 0;
                foreach (RoomUser m in GetRoomUsers())
                {
                    if (m != null && m.GetClient() != null && m.GetClient().GetHabbo() != null && m.GetClient().GetHabbo().Id != _room.OwnerId)
                    {
                        m.GetClient().SendWhisper("Esta sala ya no se encuentra a la venta.");
                    }
                }
            }
        }

        private void onRemove(RoomUser user)
        {
            try
            {

                GameClient session = user.GetClient();
                if (session == null)
                    return;

                List<RoomUser> Bots = new List<RoomUser>();

                try
                {
                    foreach (RoomUser roomUser in GetUserList().ToList())
                    {
                        if (roomUser == null)
                            continue;

                        if (roomUser.IsBot && !roomUser.IsPet)
                        {
                            if (!Bots.Contains(roomUser))
                                Bots.Add(roomUser);
                        }
                    }
                }
                catch { }

                List<RoomUser> PetsToRemove = new List<RoomUser>();
                foreach (RoomUser Bot in Bots.ToList())
                {
                    if (Bot == null || Bot.BotAI == null)
                        continue;

                    Bot.BotAI.OnUserLeaveRoom(session);

                    if (Bot.IsPet && Bot.PetData.OwnerId == user.UserId && !_room.CheckRights(session, true))
                    {
                        if (!PetsToRemove.Contains(Bot))
                            PetsToRemove.Add(Bot);
                    }
                }

                foreach (RoomUser toRemove in PetsToRemove.ToList())
                {
                    if (toRemove == null)
                        continue;

                    if (user.GetClient() == null || user.GetClient().GetHabbo() == null || user.GetClient().GetHabbo().GetInventoryComponent() == null)
                        continue;

                    user.GetClient().GetHabbo().GetInventoryComponent().TryAddPet(toRemove.PetData);
                    RemoveBot(toRemove.VirtualId, false);
                }

                _room.GetGameMap().RemoveUserFromMap(user, new Point(user.X, user.Y));
            }
            catch (Exception e)
            {
                Logging.LogCriticalException(e.ToString());
            }
        }

        private void RemoveRoomUser(RoomUser user)
        {
            if (user.SetStep)
                _room.GetGameMap().GameMap[user.SetX, user.SetY] = user.SqState;
            else
                _room.GetGameMap().GameMap[user.X, user.Y] = user.SqState;

            _room.GetGameMap().RemoveUserFromMap(user, new Point(user.X, user.Y));
            _room.SendMessage(new UserRemoveComposer(user.VirtualId));

            RoomUser toRemove = null;
            if (this._users.TryRemove(user.InternalRoomID, out toRemove))
            {
                //uhmm, could put the below stuff in but idk.
            }

            user.InternalRoomID = -1;
            onRemove(user);
        }

        public bool TryGetPet(int PetId, out RoomUser Pet)
        {
            return this._pets.TryGetValue(PetId, out Pet);
        }

        public bool TryGetBot(int BotId, out RoomUser Bot)
        {
            return this._bots.TryGetValue(BotId, out Bot);
        }

        public RoomUser GetBotByName(string Name)
        {
            bool FoundBot = this._bots.Where(x => x.Value.BotData != null && x.Value.BotData.Name.ToLower() == Name.ToLower()).ToList().Count() > 0;
            if (FoundBot)
            {
                int Id = this._bots.FirstOrDefault(x => x.Value.BotData != null && x.Value.BotData.Name.ToLower() == Name.ToLower()).Value.BotData.Id;

                return this._bots[Id];
            }

            return null;
        }

        public void UpdateUserCount(int count)
        {
            userCount = count;
            _room.RoomData.UsersNow = count;

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("UPDATE `rooms` SET `users_now` = '" + count + "' WHERE `id` = '" + _room.RoomId + "' LIMIT 1");
            }
        }

        public RoomUser GetRoomUserByVirtualId(int VirtualId)
        {
            RoomUser User = null;
            if (!_users.TryGetValue(VirtualId, out User))
                return null;
            return User;
        }

        public RoomUser GetRoomUserByHabbo(int Id)
        {
            if (this == null)
                return null;

            RoomUser User = this.GetUserList().Where(x => x != null && x.GetClient() != null && x.GetClient().GetHabbo() != null && x.GetClient().GetHabbo().Id == Id).FirstOrDefault();

            if (User != null)
                return User;

            return null;
        }

        public RoomUser GetRoomUserByUsername(string Username)
        {
            if (this == null)
                return null;

            RoomUser User = this.GetUserList().Where(x => x != null && x.GetClient() != null && x.GetClient().GetHabbo() != null && x.GetClient().GetHabbo().Username == Username).FirstOrDefault();

            if (User != null)
                return User;

            return null;
        }

        public List<RoomUser> GetRoomUsers()
        {
            List<RoomUser> List = new List<RoomUser>();

            List = this.GetUserList().Where(x => (!x.IsBot)).ToList();

            return List;
        }

        public List<RoomUser> GetRoomusersByChat(bool chat)
        {
            var returnList = new List<RoomUser>();
            foreach (RoomUser user in GetUserList().ToList())
            {
                if (user == null)
                    continue;

                if (!user.IsBot && user.GetClient() != null && user.GetClient().GetHabbo() != null && user.GetClient().GetHabbo().ChatPreference == false)
                    returnList.Add(user);
            }

            return returnList;
        }

        public List<RoomUser> GetRoomUserByRank(int minRank)
        {
            var returnList = new List<RoomUser>();
            foreach (RoomUser user in GetUserList().ToList())
            {
                if (user == null)
                    continue;

                if (!user.IsBot && user.GetClient() != null && user.GetClient().GetHabbo() != null && user.GetClient().GetHabbo().Rank >= minRank)
                    returnList.Add(user);
            }

            return returnList;
        }

        public RoomUser GetRoomUserByHabbo(string pName)
        {
            RoomUser User = this.GetUserList().Where(x => x != null && x.GetClient() != null && x.GetClient().GetHabbo() != null && x.GetClient().GetHabbo().Username.Equals(pName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (User != null)
                return User;

            return null;
        }

        public void UpdatePets()
        {
            foreach (Pet Pet in GetPets().ToList())
            {
                if (Pet == null)
                    continue;

                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    if (Pet.DBState == DatabaseUpdateState.NeedsInsert)
                    {
                        dbClient.SetQuery("INSERT INTO `bots` (`id`,`user_id`,`room_id`,`name`,`x`,`y`,`z`) VALUES ('" + Pet.PetId + "','" + Pet.OwnerId + "','" + Pet.RoomId + "',@name,'0','0','0')");
                        dbClient.AddParameter("name", Pet.Name);
                        dbClient.RunQuery();

                        dbClient.SetQuery("INSERT INTO `bots_petdata` (`type`,`race`,`color`,`experience`,`energy`,`createstamp`,`nutrition`,`respect`) VALUES ('" + Pet.Type + "',@race,@color,'0','100','" + Pet.CreationStamp + "','0','0')");
                        dbClient.AddParameter(Pet.PetId + "race", Pet.Race);
                        dbClient.AddParameter(Pet.PetId + "color", Pet.Color);
                        dbClient.RunQuery();
                    }
                    else if (Pet.DBState == DatabaseUpdateState.NeedsUpdate)
                    {
                        //Surely this can be *99 better?
                        RoomUser User = GetRoomUserByVirtualId(Pet.VirtualId);

                        dbClient.RunQuery("UPDATE `bots` SET room_id = " + Pet.RoomId + ", x = " + (User != null ? User.X : 0) + ", Y = " + (User != null ? User.Y : 0) + ", Z = " + (User != null ? User.Z : 0) + " WHERE `id` = '" + Pet.PetId + "' LIMIT 1");
                        dbClient.RunQuery("UPDATE `bots_petdata` SET `experience` = '" + Pet.experience + "', `energy` = '" + Pet.Energy + "', `nutrition` = '" + Pet.Nutrition + "', `respect` = '" + Pet.Respect + "' WHERE `id` = '" + Pet.PetId + "' LIMIT 1");
                    }

                    Pet.DBState = DatabaseUpdateState.Updated;
                }
            }
        }

        public List<Pet> GetPets()
        {
            List<Pet> Pets = new List<Pet>();
            foreach (RoomUser User in this._pets.Values.ToList())
            {
                if (User == null || !User.IsPet)
                    continue;

                Pets.Add(User.PetData);
            }

            return Pets;
        }

        public void SerializeStatusUpdates()
        {
            List<RoomUser> Users = new List<RoomUser>();
            ICollection<RoomUser> RoomUsers = GetUserList();

            if (RoomUsers == null)
                return;

            foreach (RoomUser User in RoomUsers.ToList())
            {
                if (User == null || !User.UpdateNeeded || Users.Contains(User))
                    continue;

                User.UpdateNeeded = false;
                Users.Add(User);
            }

            if (Users.Count > 0)
                _room.SendMessage(new UserUpdateComposer(Users));
        }

        public void UpdateUserStatusses()
        {
            foreach (RoomUser user in GetUserList().ToList())
            {
                if (user == null)
                    continue;

                UpdateUserStatus(user, false);
            }
        }

        private bool isValid(RoomUser user)
        {
            if (user == null)
                return false;
            if (user.IsBot)
                return true;
            if (user.GetClient() == null)
                return false;
            if (user.GetClient().GetHabbo() == null)
                return false;
            if (user.GetClient().GetHabbo().CurrentRoomId != _room.RoomId)
                return false;
            return true;
        }

        public void OnCycle()
        {
            int userCounter = 0;

            try
            {

                List<RoomUser> ToRemove = new List<RoomUser>();

                foreach (RoomUser User in GetUserList().ToList())
                {
                    if (User == null)
                        continue;

                    if (!isValid(User))
                    {
                        if (User.GetClient() != null)
                            RemoveUserFromRoom(User.GetClient(), false, false);
                        else
                            RemoveRoomUser(User);
                    }

                    if (User.NeedsAutokick && !ToRemove.Contains(User))
                    {
                        ToRemove.Add(User);
                        continue;
                    }

                    bool updated = false;
                    User.IdleTime++;
                    User.HandleSpamTicks();
                    if (!User.IsBot && !User.IsAsleep && User.IdleTime >= 1200)
                    {
                        User.IsAsleep = true;
                        _room.SendMessage(new SleepComposer(User, true));
                    }

                    if (User.CarryItemID > 0)
                    {
                        User.CarryTimer--;
                        if (User.CarryTimer <= 0)
                            User.CarryItem(0);
                    }

                    if (_room.GotFreeze())
                        _room.GetFreeze().CycleUser(User);

                    bool InvalidStep = false;

                    if (User.isRolling)
                    {
                        if (User.rollerDelay <= 0)
                        {
                            UpdateUserStatus(User, false);
                            User.isRolling = false;
                        }
                        else
                            User.rollerDelay--;
                    }

                    if (User.SetStep)
                    {
                        User.DiceTotal = 0;

                        if (this._room.GetGameMap().IsValidWalk(User, new Vector2D(User.X, User.Y), new Vector2D(User.SetX, User.SetY), User.AllowOverride) || User.RidingHorse)
                        {
                            if (!User.RidingHorse)
                                _room.GetGameMap().UpdateUserMovement(new Point(User.Coordinate.X, User.Coordinate.Y), new Point(User.SetX, User.SetY), User);

                            List<Item> items = _room.GetGameMap().GetCoordinatedItems(new Point(User.X, User.Y));
                            foreach (Item Item in items.ToList())
                            {
                                Item.UserWalksOffFurni(User);
                            }

                            if (!User.IsBot)
                            {
                                User.X = User.SetX;
                                User.Y = User.SetY;
                                User.Z = User.SetZ;
                            }
                            else if (User.IsBot && !User.RidingHorse)
                            {
                                User.X = User.SetX;
                                User.Y = User.SetY;
                                User.Z = User.SetZ;
                            }

                            if (!User.IsBot && User.RidingHorse)
                            {
                                RoomUser Horse = GetRoomUserByVirtualId(User.HorseID);
                                if (Horse != null)
                                {
                                    Horse.X = User.SetX;
                                    Horse.Y = User.SetY;
                                }
                            }

                            if (User.X == _room.GetGameMap().Model.DoorX && User.Y == _room.GetGameMap().Model.DoorY && !ToRemove.Contains(User) && !User.IsBot)
                            {
                                ToRemove.Add(User);
                                continue;
                            }

                            List<Item> Items = _room.GetGameMap().GetCoordinatedItems(new Point(User.X, User.Y));
                            foreach (Item Item in Items.ToList())
                            {
                                Item.UserWalksOnFurni(User);
                            }

                            UpdateUserStatus(User, true);
                        }
                        else
                            InvalidStep = true;
                        User.SetStep = false;
                    }
                    if (User.PathRecalcNeeded)
                    {
                        if (User.Path.Count > 1)
                            User.Path.Clear();

                        User.Path = PathFinder.FindPath(User, this._room.GetGameMap().DiagonalEnabled, this._room.GetGameMap(), new Vector2D(User.X, User.Y), new Vector2D(User.GoalX, User.GoalY));

                        if (User.Path.Count > 1)
                        {
                            User.PathStep = 1;
                            User.IsWalking = true;
                            User.PathRecalcNeeded = false;
                        }
                        else
                        {
                            User.PathRecalcNeeded = false;
                            if (User.Path.Count > 1)
                                User.Path.Clear();
                        }
                    }

                    if (User.IsWalking && !User.Freezed)
                    {
                        SquarePoint point = DreamPathfinder.GetNextStep(User, new Vector2D(User.X, User.Y), new Vector2D(User.GoalX, User.GoalY), this._room.GetGameMap());

                        if (InvalidStep || (User.PathStep >= User.Path.Count) || (User.GoalX == User.X && User.GoalY == User.Y)) //No path found, or reached goal (:
                        {
                            User.IsWalking = false;
                            User.RemoveStatus("mv");

                            if (User.Statusses.ContainsKey("sign"))
                                User.RemoveStatus("sign");

                            if (User.IsBot && User.BotData.TargetUser > 0)
                            {
                                if (User.CarryItemID > 0)
                                {
                                    RoomUser Target = _room.GetRoomUserManager().GetRoomUserByHabbo(User.BotData.TargetUser);

                                    if (Target != null && Gamemap.TilesTouching(User.X, User.Y, Target.X, Target.Y))
                                    {
                                        User.SetRot(Rotation.Calculate(User.X, User.Y, Target.X, Target.Y), false);
                                        Target.SetRot(Rotation.Calculate(Target.X, Target.Y, User.X, User.Y), false);
                                        Target.CarryItem(User.CarryItemID);
                                    }
                                }

                                User.CarryItem(0);
                                User.BotData.TargetUser = 0;
                            }

                            if (User.RidingHorse && User.IsPet == false && !User.IsBot)
                            {
                                RoomUser mascotaVinculada = GetRoomUserByVirtualId(User.HorseID);
                                if (mascotaVinculada != null)
                                {
                                    mascotaVinculada.IsWalking = false;
                                    mascotaVinculada.RemoveStatus("mv");
                                    mascotaVinculada.UpdateNeeded = true;
                                }
                            }
                        }
                        else
                        {
                            Vector2D NextStep = User.Path[(User.Path.Count - User.PathStep) - 1];
                            User.PathStep++;

                            if (User.FastWalking && User.PathStep < User.Path.Count)
                            {
                                int s2 = (User.Path.Count - User.PathStep) - 1;
                                NextStep = User.Path[s2];
                                User.PathStep++;
                            }

                            if (User.SuperFastWalking && User.PathStep < User.Path.Count)
                            {
                                int s2 = (User.Path.Count - User.PathStep) - 1;
                                NextStep = User.Path[s2];
                                User.PathStep++;
                                User.PathStep++;
                            }

                            int nextX = NextStep.X;
                            int nextY = NextStep.Y;
                            User.RemoveStatus("mv");


                            if (_room.GetGameMap().IsValidStep2(User, new Vector2D(User.X, User.Y), new Vector2D(nextX, nextY), (User.GoalX == nextX && User.GoalY == nextY), User.AllowOverride))
                            {
                                double nextZ = _room.GetGameMap().SqAbsoluteHeight(nextX, nextY);

                                if (!User.IsBot)
                                {
                                    if (User.isSitting)
                                    {
                                        User.Statusses.Remove("sit");
                                        User.Z += 0.35;
                                        User.isSitting = false;
                                        User.UpdateNeeded = true;
                                    }
                                    else if (User.isLying)
                                    {
                                        User.Statusses.Remove("sit");
                                        User.Z += 0.35;
                                        User.isLying = false;
                                        User.UpdateNeeded = true;
                                    }
                                }
                                if (!User.IsBot)
                                {
                                    User.Statusses.Remove("lay");
                                    User.Statusses.Remove("sit");
                                }

                                if (!User.IsBot && !User.IsPet && User.GetClient() != null)
                                {
                                    if (User.GetClient().GetHabbo().IsTeleporting)
                                    {
                                        User.GetClient().GetHabbo().IsTeleporting = false;
                                        User.GetClient().GetHabbo().TeleporterId = 0;
                                    }
                                    else if (User.GetClient().GetHabbo().IsHopping)
                                    {
                                        User.GetClient().GetHabbo().IsHopping = false;
                                        User.GetClient().GetHabbo().HopperId = 0;
                                    }
                                }

                                if (!User.IsBot && User.RidingHorse && User.IsPet == false)
                                {
                                    RoomUser Horse = GetRoomUserByVirtualId(User.HorseID);
                                    if (Horse != null)
                                        Horse.AddStatus("mv", nextX + "," + nextY + "," + TextHandling.GetString(nextZ));

                                    User.AddStatus("mv", +nextX + "," + nextY + "," + TextHandling.GetString(nextZ + 1));

                                    User.UpdateNeeded = true;
                                    Horse.UpdateNeeded = true;
                                }
                                else
                                    User.AddStatus("mv", nextX + "," + nextY + "," + TextHandling.GetString(nextZ));

                                int newRot = Rotation.Calculate(User.X, User.Y, nextX, nextY, User.MoonwalkEnabled);

                                User.RotBody = newRot;
                                User.RotHead = newRot;

                                User.SetStep = true;
                                User.SetX = nextX;
                                User.SetY = nextY;
                                User.SetZ = nextZ;
                                UpdateUserEffect(User, User.SetX, User.SetY);

                                updated = true;

                                if (User.RidingHorse && User.IsPet == false && !User.IsBot)
                                {
                                    RoomUser Horse = GetRoomUserByVirtualId(User.HorseID);
                                    if (Horse != null)
                                    {
                                        Horse.RotBody = newRot;
                                        Horse.RotHead = newRot;

                                        Horse.SetStep = true;
                                        Horse.SetX = nextX;
                                        Horse.SetY = nextY;
                                        Horse.SetZ = nextZ;
                                    }
                                }

                                _room.GetGameMap().GameMap[User.X, User.Y] = User.SqState; // REstore the old one
                                User.SqState = _room.GetGameMap().GameMap[User.SetX, User.SetY]; //Backup the new one

                                if (_room.RoomBlockingEnabled == 0)
                                {
                                    _room.GetGameMap().GameMap[nextX, nextY] = 0;
                                }
                                else
                                    _room.GetGameMap().GameMap[nextX, nextY] = 1;

                            }
                        }
                        if (!User.RidingHorse)
                            User.UpdateNeeded = true;

                        if (_room.GotSoccer())
                            _room.GetSoccer().OnUserWalk(User);
                    }
                    else
                    {
                        if (User.Statusses.ContainsKey("mv"))
                        {
                            User.RemoveStatus("mv");
                            User.UpdateNeeded = true;

                            if (User.RidingHorse)
                            {
                                RoomUser Horse = GetRoomUserByVirtualId(User.HorseID);
                                if (Horse != null)
                                {
                                    Horse.RemoveStatus("mv");
                                    Horse.UpdateNeeded = true;
                                }
                            }
                        }
                    }

                    if (User.RidingHorse)
                        User.ApplyEffect(77);

                    if (User.IsBot && User.BotAI != null)
                        User.BotAI.OnTimerTick();
                    else
                        userCounter++;

                    if (!updated)
                    {
                        UpdateUserEffect(User, User.X, User.Y);
                    }
                }

                foreach (RoomUser toRemove in ToRemove.ToList())
                {
                    GameClient client = MoonEnvironment.GetGame().GetClientManager().GetClientByUserID(toRemove.HabboId);
                    if (client != null)
                    {
                        RemoveUserFromRoom(client, true, false);
                    }
                    else
                        RemoveRoomUser(toRemove);
                }

                if (userCount != userCounter)
                    UpdateUserCount(userCounter);
            }
            catch (Exception e)
            {
                int rId = 0;
                if (_room != null)
                    rId = _room.Id;

                Logging.LogCriticalException("Affected Room - ID: " + rId + " - " + e.ToString());
            }
        }



        public void UpdateUserStatus(RoomUser User, bool cyclegameitems)
        {
            if (User == null)
                return;

            try
            {
                bool isBot = User.IsBot;
                if (isBot)
                    cyclegameitems = false;

                if (MoonEnvironment.GetUnixTimestamp() > MoonEnvironment.GetUnixTimestamp() + User.SignTime)
                {
                    if (User.Statusses.ContainsKey("sign"))
                    {
                        User.Statusses.Remove("sign");
                        User.UpdateNeeded = true;
                    }
                }

                if ((User.Statusses.ContainsKey("lay") && !User.isLying) || (User.Statusses.ContainsKey("sit") && !User.isSitting))
                {
                    if (User.Statusses.ContainsKey("lay"))
                        User.Statusses.Remove("lay");
                    if (User.Statusses.ContainsKey("sit"))
                        User.Statusses.Remove("sit");
                    User.UpdateNeeded = true;
                }
                else if (User.isLying || User.isSitting)
                    return;

                double newZ;
                List<Item> ItemsOnSquare = _room.GetGameMap().GetAllRoomItemForSquare(User.X, User.Y);
                if (ItemsOnSquare != null || ItemsOnSquare.Count != 0)
                {
                    if (User.RidingHorse && User.IsPet == false)
                        newZ = _room.GetGameMap().SqAbsoluteHeight(User.X, User.Y, ItemsOnSquare.ToList()) + 1;
                    else
                        newZ = _room.GetGameMap().SqAbsoluteHeight(User.X, User.Y, ItemsOnSquare.ToList());
                }
                else
                {
                    newZ = 1;
                }

                if (newZ != User.Z)
                {
                    User.Z = newZ;
                    User.UpdateNeeded = true;
                }

                DynamicRoomModel Model = _room.GetGameMap().Model;
                if (Model.SqState[User.X, User.Y] == SquareState.SEAT)
                {
                    if (!User.Statusses.ContainsKey("sit"))
                        User.Statusses.Add("sit", "1.0");
                    User.Z = Model.SqFloorHeight[User.X, User.Y];
                    User.RotHead = Model.SqSeatRot[User.X, User.Y];
                    User.RotBody = Model.SqSeatRot[User.X, User.Y];

                    User.UpdateNeeded = true;
                }


                if (ItemsOnSquare.Count == 0)
                    User.LastItem = null;


                foreach (Item Item in ItemsOnSquare.ToList())
                {
                    if (Item == null)
                        continue;

                    if (Item.GetBaseItem().IsSeat)
                    {
                        if (!User.Statusses.ContainsKey("sit"))
                        {
                            if (!User.Statusses.ContainsKey("sit"))
                                User.Statusses.Add("sit", TextHandling.GetString(Item.GetBaseItem().Height));
                        }

                        User.Z = Item.GetZ;
                        User.RotHead = Item.Rotation;
                        User.RotBody = Item.Rotation;
                        User.UpdateNeeded = true;

                    }

                    switch (Item.GetBaseItem().InteractionType)
                    {
                        #region Beds & Tents
                        case InteractionType.BED:
                        case InteractionType.TENT_SMALL:
                            {
                                if (!User.Statusses.ContainsKey("lay"))
                                    User.Statusses.Add("lay", TextHandling.GetString(Item.GetBaseItem().Height) + " null");

                                User.Z = Item.GetZ;
                                User.RotHead = Item.Rotation;
                                User.RotBody = Item.Rotation;

                                User.UpdateNeeded = true;
                                break;
                            }
                        #endregion

                        #region Banzai Gates
                        case InteractionType.banzaigategreen:
                        case InteractionType.banzaigateblue:
                        case InteractionType.banzaigatered:
                        case InteractionType.banzaigateyellow:
                            {
                                if (cyclegameitems)
                                {
                                    int effectID = Convert.ToInt32(Item.team + 32);
                                    TeamManager t = User.GetClient().GetHabbo().CurrentRoom.GetTeamManagerForBanzai();

                                    if (User.Team == TEAM.NONE)
                                    {
                                        if (t.CanEnterOnTeam(Item.team))
                                        {
                                            if (User.Team != TEAM.NONE)
                                                t.OnUserLeave(User);
                                            User.Team = Item.team;

                                            t.AddUser(User);

                                            if (User.GetClient().GetHabbo().Effects().CurrentEffect != effectID)
                                                User.GetClient().GetHabbo().Effects().ApplyEffect(effectID);
                                        }
                                    }
                                    else if (User.Team != TEAM.NONE && User.Team != Item.team)
                                    {
                                        t.OnUserLeave(User);
                                        User.Team = TEAM.NONE;
                                        User.GetClient().GetHabbo().Effects().ApplyEffect(0);
                                    }
                                    else
                                    {
                                        //usersOnTeam--;
                                        t.OnUserLeave(User);
                                        if (User.GetClient().GetHabbo().Effects().CurrentEffect == effectID)
                                            User.GetClient().GetHabbo().Effects().ApplyEffect(0);
                                        User.Team = TEAM.NONE;
                                    }
                                    //Item.ExtraData = usersOnTeam.ToString();
                                    //Item.UpdateState(false, true);
                                }
                                break;
                            }
                        #endregion

                        #region Freeze Gates
                        case InteractionType.FREEZE_YELLOW_GATE:
                        case InteractionType.FREEZE_RED_GATE:
                        case InteractionType.FREEZE_GREEN_GATE:
                        case InteractionType.FREEZE_BLUE_GATE:
                            {
                                if (cyclegameitems)
                                {
                                    int effectID = Convert.ToInt32(Item.team + 39);
                                    TeamManager t = User.GetClient().GetHabbo().CurrentRoom.GetTeamManagerForFreeze();

                                    if (User.Team == TEAM.NONE)
                                    {
                                        if (t.CanEnterOnTeam(Item.team))
                                        {
                                            if (User.Team != TEAM.NONE)
                                                t.OnUserLeave(User);
                                            User.Team = Item.team;
                                            t.AddUser(User);

                                            if (User.GetClient().GetHabbo().Effects().CurrentEffect != effectID)
                                                User.GetClient().GetHabbo().Effects().ApplyEffect(effectID);
                                        }
                                    }
                                    else if (User.Team != TEAM.NONE && User.Team != Item.team)
                                    {
                                        t.OnUserLeave(User);
                                        User.Team = TEAM.NONE;
                                        User.GetClient().GetHabbo().Effects().ApplyEffect(0);
                                    }
                                    else
                                    {
                                        //usersOnTeam--;
                                        t.OnUserLeave(User);
                                        if (User.GetClient().GetHabbo().Effects().CurrentEffect == effectID)
                                            User.GetClient().GetHabbo().Effects().ApplyEffect(0);
                                        User.Team = TEAM.NONE;
                                    }
                                    //Item.ExtraData = usersOnTeam.ToString();
                                    //Item.UpdateState(false, true);
                                }
                                break;
                            }
                        #endregion

                        #region Banzai Teles
                        case InteractionType.banzaitele:
                            {
                                if (User.Statusses.ContainsKey("mv"))
                                    _room.GetGameItemHandler().onTeleportRoomUserEnter(User, Item);
                                break;
                            }
                        #endregion

                        #region Football Gate

                        #endregion

                        #region HI PROVIDER
                        case InteractionType.HI_PROVIDER:
                            {
                                if (User == null)
                                    return;

                                if (!User.IsBot)
                                {
                                    var handitem = int.Parse(Item.ExtraData);
                                    User.CarryItem(handitem);
                                    Item.UpdateState(false, true);
                                    Item.RequestUpdate(2, true);
                                }
                                break;
                            }
                        #endregion

                        #region CHESSCHAIR
                        case InteractionType.chesschair:
                        case InteractionType.SILLAGUIA:
                            {
                                if (User == null)
                                    return;

                                if (!User.IsBot)
                                {
                                    RoomUser ThisUser = User.GetClient().GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(User.GetClient().GetHabbo().Id);
                                    User.GetClient().GetHabbo().PlayingChess = true;
                                    User.GetClient().SendMessage(RoomNotificationComposer.SendBubble("playing_chess", "Acabas de activar el modo ajedrez, disfruta del juego y recuerda jugar limpio.", ""));
                                    User.GetClient().SendMessage(new MassEventComposer("habbopages/chess.txt"));
                                }
                                break;
                            }
                        #endregion

                        #region DA PROVIDER
                        case InteractionType.DA_PROVIDER:
                            {
                                if (User == null)
                                    return;

                                if (!User.IsBot)
                                {
                                    RoomUser ThisUser = User.GetClient().GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(User.GetClient().GetHabbo().Id);
                                    var dance = int.Parse(Item.ExtraData);
                                    User.GetClient().GetHabbo().CurrentRoom.SendMessage(new DanceComposer(ThisUser, dance));
                                    Item.UpdateState(false, true);
                                    Item.RequestUpdate(2, true);
                                }
                                break;
                            }
                        #endregion

                        #region Effects
                        case InteractionType.FX_PROVIDER:
                            {
                                {
                                    if (User == null)
                                        return;

                                    if (!User.IsBot)
                                    {
                                        var effect = int.Parse(Item.ExtraData);
                                        User.GetClient().GetHabbo().Effects().ApplyEffect(effect);
                                        Item.UpdateState(false, true);
                                        Item.RequestUpdate(4, true);
                                    }
                                    break;
                                }
                            }
                        #endregion

                        #region InfoLink
                        case InteractionType.ROOM_PROVIDER:
                            {
                                {
                                    if (User == null)
                                        return;

                                    if (!User.IsBot)
                                    {
                                        var room = Item.ExtraData;
                                        User.GetClient().SendMessage(new MassEventComposer("habbopages/" + room + ".txt"));
                                    }
                                    break;
                                }
                            }
                        #endregion

                        #region Arrows
                        case InteractionType.ARROW:
                            {
                                if (User.GoalX == Item.GetX && User.GoalY == Item.GetY)
                                {
                                    if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                                        continue;

                                    Room Room;

                                    if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(User.GetClient().GetHabbo().CurrentRoomId, out Room))
                                        return;

                                    if (!ItemTeleporterFinder.IsTeleLinked(Item.Id, Room))
                                        User.UnlockWalking();
                                    else
                                    {
                                        int LinkedTele = ItemTeleporterFinder.GetLinkedTele(Item.Id, Room);
                                        int TeleRoomId = ItemTeleporterFinder.GetTeleRoomId(LinkedTele, Room);

                                        if (TeleRoomId == Room.RoomId)
                                        {
                                            Item TargetItem = Room.GetRoomItemHandler().GetItem(LinkedTele);
                                            if (TargetItem == null)
                                            {
                                                if (User.GetClient() != null)
                                                    User.GetClient().SendWhisper("Ei, algum erro aconteceu avise um administrador!");
                                                return;
                                            }
                                            else
                                            {
                                                Room.GetGameMap().TeleportToItem(User, TargetItem);
                                            }
                                        }
                                        else if (TeleRoomId != Room.RoomId)
                                        {
                                            if (User != null && !User.IsBot && User.GetClient() != null && User.GetClient().GetHabbo() != null)
                                            {
                                                User.GetClient().GetHabbo().IsTeleporting = true;
                                                User.GetClient().GetHabbo().TeleportingRoomID = TeleRoomId;
                                                User.GetClient().GetHabbo().TeleporterId = LinkedTele;

                                                User.GetClient().GetHabbo().PrepareRoom(TeleRoomId, "");
                                            }
                                        }
                                        else if (this._room.GetRoomItemHandler().GetItem(LinkedTele) != null)
                                        {
                                            User.SetPos(Item.GetX, Item.GetY, Item.GetZ);
                                            User.SetRot(Item.Rotation, false);
                                        }
                                        else
                                            User.UnlockWalking();
                                    }
                                }
                                break;
                            }
                        #endregion

                        #region Pinatas
                        case InteractionType.PINATA:
                            {
                                if (User.IsWalking && Item.ExtraData.Length > 0)
                                {
                                    var givenHits = int.Parse(Item.ExtraData);
                                    if (givenHits < 1 && User.CurrentEffect == 158)
                                    {
                                        givenHits++;
                                        Item.ExtraData = givenHits.ToString();
                                        Item.UpdateState();

                                        if (givenHits == 1) MoonEnvironment.GetGame().GetPinataManager().ReceiveCrackableReward(User, _room, Item);

                                        #region Achievements
                                        MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_PinataWhacker", 1);
                                        MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_PinataBreaker", 1);
                                        #endregion

                                    }
                                }
                                break;
                            }
                        #endregion

                        #region Plantas
                        case InteractionType.PLANT_SEED:
                            {
                                if (User.IsWalking && Item.ExtraData.Length > 0)
                                {
                                    var givenHits = int.Parse(Item.ExtraData);
                                    if (givenHits < 1 && User.CurrentEffect == 192)
                                    {
                                        givenHits++;
                                        Item.ExtraData = givenHits.ToString();
                                        Item.UpdateState();

                                        if (givenHits > 5) MoonEnvironment.GetGame().GetPinataManager().ReceiveCrackableReward(User, _room, Item);
                                        {
                                            givenHits = 0;
                                            Item.ExtraData = givenHits.ToString();
                                            Item.UpdateState();
                                            MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_PinataWhacker", 1);
                                        }
                                    }
                                }
                                break;
                            }
                        #endregion

                        case InteractionType.FOOTBALL_GATE:
                            {
                                Room Room = User.GetClient().GetHabbo().CurrentRoom;
                                if (User.GetClient().GetHabbo().LastMovFGate && User.GetClient().GetHabbo().BackupGender == User.GetClient().GetHabbo().Gender)
                                {
                                    User.GetClient().GetHabbo().LastMovFGate = false;
                                    User.GetClient().GetHabbo().Look = User.GetClient().GetHabbo().BackupLook;
                                }
                                else
                                {
                                    // mini Fix
                                    string _gateLook = ((User.GetClient().GetHabbo().Gender.ToUpper() == "M") ? Item.ExtraData.Split(',')[0] : Item.ExtraData.Split(',')[1]);
                                    string gateLook = "";
                                    foreach (string part in _gateLook.Split('.'))
                                    {
                                        if (part.StartsWith("hd"))
                                            continue;
                                        gateLook += part + ".";
                                    }
                                    gateLook = gateLook.Substring(0, gateLook.Length - 1);

                                    var Parts = User.GetClient().GetHabbo().Look.Split('.');
                                    var NewLook = "";
                                    foreach (var Part in Parts)
                                    {
                                        if (/*Part.StartsWith("hd") || */Part.StartsWith("sh") || Part.StartsWith("cp") || Part.StartsWith("cc") || Part.StartsWith("ch") || Part.StartsWith("lg") || Part.StartsWith("ca") || Part.StartsWith("wa"))
                                            continue;
                                        NewLook += Part + ".";
                                    }
                                    NewLook += gateLook;

                                    User.GetClient().GetHabbo().BackupLook = User.GetClient().GetHabbo().Look;
                                    User.GetClient().GetHabbo().BackupGender = User.GetClient().GetHabbo().Gender;
                                    User.GetClient().GetHabbo().Look = NewLook;
                                    User.GetClient().GetHabbo().LastMovFGate = true;
                                }

                                Room.SendMessage(new UsersComposer(User));

                                if (User.GetClient().GetHabbo().InRoom)
                                {
                                    Room.SendMessage(new UsersComposer(User));
                                }

                                break;
                            }
                    }
                }

                if (User.PathRecalcNeeded)
                {
                    if (User.Path.Count > 1)
                        User.Path.Clear();

                    User.Path = PathFinder.FindPath(User, this._room.GetGameMap().DiagonalEnabled, this._room.GetGameMap(), new Vector2D(User.X, User.Y), new Vector2D(User.GoalX, User.GoalY));

                    if (User.Path.Count > 1)
                    {
                        User.PathStep = 1;
                        User.IsWalking = true;
                        User.PathRecalcNeeded = false;
                    }
                    else
                    {
                        User.PathRecalcNeeded = false;
                        if (User.Path.Count > 1)
                            User.Path.Clear();
                    }
                }

                if (User.isSitting && User.TeleportEnabled)
                {
                    User.Z -= 0.35;
                    User.UpdateNeeded = true;
                }

                if (cyclegameitems)
                {
                    if (_room.GotSoccer())
                        _room.GetSoccer().OnUserWalk(User);

                    if (_room.GotBanzai())
                        _room.GetBanzai().OnUserWalk(User);

                    if (_room.GotFreeze())
                        _room.GetFreeze().OnUserWalk(User);
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e.ToString());
            }
        }

        private void UpdateUserEffect(RoomUser User, int x, int y)
        {
            if (User == null || User.IsBot || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                return;

            try
            {
                byte NewCurrentUserItemEffect = _room.GetGameMap().EffectMap[x, y];
                if (NewCurrentUserItemEffect > 0)
                {
                    if (User.GetClient().GetHabbo().Effects().CurrentEffect == 0)
                        User.CurrentItemEffect = ItemEffectType.NONE;

                    ItemEffectType Type = ByteToItemEffectEnum.Parse(NewCurrentUserItemEffect);
                    if (Type != User.CurrentItemEffect)
                    {
                        switch (Type)
                        {
                            case ItemEffectType.Iceskates:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(User.GetClient().GetHabbo().Gender == "M" ? 38 : 39);
                                    User.CurrentItemEffect = ItemEffectType.Iceskates;
                                    break;
                                }

                            case ItemEffectType.Normalskates:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(User.GetClient().GetHabbo().Gender == "M" ? 55 : 56);
                                    User.CurrentItemEffect = Type;
                                    break;
                                }
                            case ItemEffectType.SWIM:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(29);
                                    User.CurrentItemEffect = Type;
                                    break;
                                }
                            case ItemEffectType.SwimLow:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(30);
                                    User.CurrentItemEffect = Type;
                                    break;
                                }
                            case ItemEffectType.SwimHalloween:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(37);
                                    User.CurrentItemEffect = Type;
                                    break;
                                }

                            case ItemEffectType.SillaGuia:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(187);
                                    User.CurrentItemEffect = Type;
                                    break;
                                }

                            case ItemEffectType.SillonVIP:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(187);
                                    User.CurrentItemEffect = Type;
                                    User.GetClient().SendMessage(new MassEventComposer("habbopages/vip.txt"));
                                    break;
                                }



                            case ItemEffectType.NONE:
                                {
                                    User.GetClient().GetHabbo().Effects().ApplyEffect(-1);
                                    User.CurrentItemEffect = Type;
                                    break;
                                }
                        }
                    }
                }
                else if (User.CurrentItemEffect != ItemEffectType.NONE && NewCurrentUserItemEffect == 0)
                {
                    User.GetClient().GetHabbo().Effects().ApplyEffect(-1);
                    User.CurrentItemEffect = ItemEffectType.NONE;
                }
            }
            catch
            {
            }
        }

        public int PetCount
        {
            get { return petCount; }
        }

        public ICollection<RoomUser> GetUserList()
        {
            return this._users.Values;
        }
    }
}
