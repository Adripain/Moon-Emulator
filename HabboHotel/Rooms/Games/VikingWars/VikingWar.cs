//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using Moon.Communication.Packets.Outgoing.Rooms.Avatar;
//using Moon.HabboHotel.GameClients;
//using Moon.HabboHotel.Items;
//using Moon.Utilities.Enclosure;
//using System.Linq;
//using Moon.Communication.Packets.Outgoing.Rooms.Engine;
//using System.Collections.Concurrent;
//using Moon.HabboHotel.Pathfinding;
//using Moon.HabboHotel.Items.Wired;
//using Moon.HabboHotel.Rooms.Games.Teams;

//namespace Moon.HabboHotel.Rooms.Games.VikingWars
//{
//    public class VikingWar
//    {
//        private Room _room;
//        private byte[,] floorMap;
//        private double timestarted;
//        private bool warStarted;
//        private int RedTents;
//        private int BlueTents;
//        private GameField field;
//        private ConcurrentDictionary<int, Item> _banzaiTiles;

//        public VikingWar(Room room)
//        {
//            this._room = room;
//            warStarted = false;
//            timestarted = 0;
//            this._banzaiTiles = new ConcurrentDictionary<int, Item>();
//        }

//        public bool isBanzaiActive
//        {
//            get { return warStarted; }
//        }

//        public void AddTile(Item item, int itemID)
//        {
//            if (!_banzaiTiles.ContainsKey(itemID))
//                _banzaiTiles.TryAdd(itemID, item);
//        }

//        public void RemoveTile(int itemID)
//        {
//            Item Item = null;
//            _banzaiTiles.TryRemove(itemID, out Item);
//        }

//        public void OnUserWalk(RoomUser User)
//        {
//            if (User == null)
//                return;

//            // Zona de colisión - Acciones de Freezeo / + ideas (?)
//        }

//        private bool VerifyPuck(RoomUser user, int actualx, int actualy)
//        {
//            return Rotation.Calculate(user.X, user.Y, actualx, actualy) == user.RotBody;
//        }

//        public void StartWar()
//        {
//            if (warStarted)
//                return;

//            floorMap = new byte[_room.GetGameMap().Model.MapSizeY, _room.GetGameMap().Model.MapSizeX];
//            field = new GameField(floorMap, true);
//            timestarted = MoonEnvironment.GetUnixTimestamp();
//            _room.GetGameManager().LockGates();

//            for (int i = 1; i < 5; i++)
//            {
//                _room.GetGameManager().Points[i] = 0;
//            }

//            foreach (Item tile in _banzaiTiles.Values)
//            {
//                tile.ExtraData = "1";
//                tile.value = 0;
//                tile.team = TEAM.NONE;
//                tile.UpdateState();
//            }

//            this.ResetTiles();
//            warStarted = true;

//            _room.GetWired().TriggerEvent(WiredBoxType.TriggerGameStarts, null);

//            foreach (RoomUser user in _room.GetRoomUserManager().GetRoomUsers())
//            {
//                user.LockedTilesCount = 0;
//            }
//        }

//        public void ResetTiles()
//        {
//            foreach (Item item in _room.GetRoomItemHandler().GetFloor.ToList())
//            {
//                InteractionType type = item.GetBaseItem().InteractionType;

//                switch (type)
//                {
//                    case InteractionType.banzaiscoreblue:
//                    case InteractionType.banzaiscoregreen:
//                    case InteractionType.banzaiscorered:
//                    case InteractionType.banzaiscoreyellow:
//                        {
//                            item.ExtraData = "0";
//                            item.UpdateState();
//                            break;
//                        }
//                }
//            }
//        }

//        public void BanzaiEnd(bool userTriggered = false)
//        {
//            warStarted = false;
//            _room.GetGameManager().StopGame();
//            floorMap = null;

//            if (!userTriggered)
//                _room.GetWired().TriggerEvent(WiredBoxType.TriggerGameEnds, null);

//            TEAM winners = _room.GetGameManager().GetWinningTeam();
//            _room.GetGameManager().UnlockGates();
//            foreach (Item tile in _banzaiTiles.Values)
//            {
//                if (tile.team == winners)
//                {
//                    tile.interactionCount = 0;
//                    tile.interactionCountHelper = 0;
//                    tile.UpdateNeeded = true;
//                }
//                else if (tile.team == TEAM.NONE)
//                {
//                    tile.ExtraData = "0";
//                    tile.UpdateState();
//                }
//            }

//            if (winners != TEAM.NONE)
//            {
//                List<RoomUser> Winners = _room.GetRoomUserManager().GetRoomUsers();

//                foreach (RoomUser User in Winners.ToList())
//                {
//                    if (User.Team != TEAM.NONE)
//                    {
//                        if (MoonEnvironment.GetUnixTimestamp() - timestarted > 5)
//                        {
//                            MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_BattleBallTilesLocked", User.LockedTilesCount);
//                            MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_BattleBallPlayer", 1);
//                        }
//                    }
//                    if (winners == TEAM.BLUE)
//                    {
//                        if (User.CurrentEffect == 35)
//                        {
//                            if (MoonEnvironment.GetUnixTimestamp() - timestarted > 5)
//                                MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_BattleBallWinner", 1);
//                            _room.SendMessage(new ActionComposer(User.VirtualId, 1));
//                        }
//                    }
//                    else if (winners == TEAM.RED)
//                    {
//                        if (User.CurrentEffect == 33)
//                        {
//                            if (MoonEnvironment.GetUnixTimestamp() - timestarted > 5)
//                                MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_BattleBallWinner", 1);
//                            _room.SendMessage(new ActionComposer(User.VirtualId, 1));
//                        }
//                    }
//                    else if (winners == TEAM.GREEN)
//                    {
//                        if (User.CurrentEffect == 34)
//                        {
//                            if (MoonEnvironment.GetUnixTimestamp() - timestarted > 5)
//                                MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_BattleBallWinner", 1);
//                            _room.SendMessage(new ActionComposer(User.VirtualId, 1));
//                        }
//                    }
//                    else if (winners == TEAM.YELLOW)
//                    {
//                        if (User.CurrentEffect == 36)
//                        {
//                            if (MoonEnvironment.GetUnixTimestamp() - timestarted > 5)
//                                MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_BattleBallWinner", 1);
//                            _room.SendMessage(new ActionComposer(User.VirtualId, 1));
//                        }
//                    }
//                }
//                if (field != null)
//                    field.Dispose();
//            }
//        }

//        public void MovePuck(Item item, GameClient mover, int newX, int newY, TEAM team)
//        {
//            if (!_room.GetGameMap().itemCanBePlacedHere(newX, newY))
//                return;

//            Point oldRoomCoord = item.Coordinate;


//            if (oldRoomCoord.X == newX && oldRoomCoord.Y == newY)
//                return;

//            item.ExtraData = (Convert.ToInt32(team).ToString());
//            item.UpdateNeeded = true;
//            item.UpdateState();

//            Double NewZ = _room.GetGameMap().Model.SqFloorHeight[newX, newY];

//            _room.SendMessage(new SlideObjectBundleComposer(item.GetX, item.GetY, item.GetZ, newX, newY, NewZ, 0, 0, item.Id));

//            _room.GetRoomItemHandler().SetFloorItem(mover, item, newX, newY, item.Rotation, false, false, false, false);

//            if (mover == null || mover.GetHabbo() == null)
//                return;

//            RoomUser user = mover.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(mover.GetHabbo().Id);
//            if (warStarted)
//            {
//                HandleBanzaiTiles(new Point(newX, newY), team, user);
//            }

//        }

//        private void SetTile(Item item, TEAM team, RoomUser user)
//        {
//            if (item.team == team)
//            {
//                if (item.value < 3)
//                {
//                    item.value++;
//                    if (item.value == 3)
//                    {
//                        user.LockedTilesCount++;
//                        _room.GetGameManager().AddPointToTeam(item.team, 1);
//                        field.updateLocation(item.GetX, item.GetY, (byte)team);
//                        List<PointField> gfield = field.doUpdate();
//                        TEAM t;
//                        foreach (PointField gameField in gfield)
//                        {
//                            t = (TEAM)gameField.forValue;
//                            foreach (Point p in gameField.getPoints())
//                            {
//                                HandleMaxBanzaiTiles(new Point(p.X, p.Y), t);
//                                floorMap[p.Y, p.X] = gameField.forValue;
//                            }
//                        }
//                    }
//                }
//            }
//            else
//            {
//                if (item.value < 3)
//                {
//                    item.team = team;
//                    item.value = 1;
//                }
//            }


//            int newColor = item.value + (Convert.ToInt32(item.team) * 3) - 1;
//            item.ExtraData = newColor.ToString();
//        }

//        private void HandleBanzaiTiles(Point coord, TEAM team, RoomUser user)
//        {
//            if (team == TEAM.NONE)
//                return;

//            List<Item> items = _room.GetGameMap().GetCoordinatedItems(coord);
//            int i = 0;
//            foreach (Item _item in _banzaiTiles.Values.ToList())
//            {
//                if (_item == null)
//                    continue;

//                if (_item.GetBaseItem().InteractionType != InteractionType.banzaifloor)
//                {
//                    user.Team = TEAM.NONE;
//                    user.ApplyEffect(0);
//                    continue;
//                }

//                if (_item.ExtraData.Equals("5") || _item.ExtraData.Equals("8") || _item.ExtraData.Equals("11") ||
//                    _item.ExtraData.Equals("14"))
//                {
//                    i++;
//                    continue;
//                }

//                if (_item.GetX != coord.X || _item.GetY != coord.Y)
//                    continue;

//                SetTile(_item, team, user);
//                if (_item.ExtraData.Equals("5") || _item.ExtraData.Equals("8") || _item.ExtraData.Equals("11") ||
//                    _item.ExtraData.Equals("14"))
//                    i++;
//                _item.UpdateState(false, true);
//            }
//            if (i == _banzaiTiles.Count)
//                BanzaiEnd();
//        }

//        private void HandleMaxBanzaiTiles(Point coord, TEAM team)
//        {
//            if (team == TEAM.NONE)
//                return;

//            List<Item> items = _room.GetGameMap().GetCoordinatedItems(coord);

//            foreach (Item _item in _banzaiTiles.Values.ToList())
//            {
//                if (_item == null)
//                    continue;

//                if (_item.GetBaseItem().InteractionType != InteractionType.banzaifloor)
//                    continue;

//                if (_item.GetX != coord.X || _item.GetY != coord.Y)
//                    continue;

//                SetMaxForTile(_item, team);
//                _room.GetGameManager().AddPointToTeam(team, 1);
//                _item.UpdateState(false, true);
//            }
//        }

//        private static void SetMaxForTile(Item item, TEAM team)
//        {
//            if (item.value < 3)
//            {
//                item.value = 3;
//                item.team = team;
//            }

//            int newColor = item.value + (Convert.ToInt32(item.team) * 3) - 1;
//            item.ExtraData = newColor.ToString();
//        }

//        public void Dispose()
//        {
//            _banzaiTiles.Clear();

//            if (floorMap != null)
//                Array.Clear(floorMap, 0, floorMap.Length);

//            if (field != null)
//                field.Dispose();

//            _room = null;
//            _banzaiTiles = null;
//            floorMap = null;
//            field = null;
//        }
//    }
//}