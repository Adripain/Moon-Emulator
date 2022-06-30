using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Moon.Communication.Packets.Incoming;

using Moon.HabboHotel.Items;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Pathfinding;

using Moon.Communication.Packets.Outgoing.Rooms.Engine;
using Moon.HabboHotel.Rooms.Games.Teams;
using Moon.HabboHotel.Items.Wired;
using System.Threading.Tasks;
using System.Threading;

namespace Moon.HabboHotel.Rooms.Games.Football
{
    public class Soccer
    {
        private Room _room;
        private Item[] gates;
        private ConcurrentDictionary<int, Item> _balls;
        private bool _gameStarted;

        public Soccer(Room room)
        {
            this._room = room;
            this.gates = new Item[4];
            this._balls = new ConcurrentDictionary<int, Item>();
            this._gameStarted = false;
        }
        public bool GameIsStarted
        {
            get { return this._gameStarted; }
        }
        public void StopGame(bool userTriggered = false)
        {
            this._gameStarted = false;

            if (!userTriggered)
                _room.GetWired().TriggerEvent(WiredBoxType.TriggerGameEnds, null);
        }

        public void StartGame()
        {
            this._gameStarted = true;
        }

        public void AddBall(Item item)
        {
            this._balls.TryAdd(item.Id, item);
        }

        public void RemoveBall(int itemID)
        {
            Item Item = null;
            this._balls.TryRemove(itemID, out Item);
        }
        public int ReverseDirection(int RotBody)
        {
            if (RotBody == 0)
                return 4;
            if (RotBody == 1)
                return 5;
            if (RotBody == 2)
                return 6;
            if (RotBody == 3)
                return 7;
            if (RotBody == 4)
                return 0;
            if (RotBody == 5)
                return 1;
            if (RotBody == 6)
                return 2;
            if (RotBody == 7)
                return 3;
            return -1;
        }
        public void MoveBallInverseBigShoot(RoomUser User, Item item, int chute)
        {
            int NewX = 0;
            int NewY = 0;
            User.UserOnBall = false;
            item.ballsteps = 0;
            item.ballstop = false;
            bool ignore = false;
            item.BallDireccion = ReverseDirection(User.RotBody);
            item.ejex = false;
            item.ejey = false;
            while (item.ballsteps < chute && !item.ballstop)
            {
                if (item.BallDireccion == 4)
                {
                    NewX = item.GetX;
                    NewY = item.GetY + 1;

                }
                else if (item.BallDireccion == 6)
                {
                    NewX = item.GetX - 1;
                    NewY = item.GetY;

                }
                else if (item.BallDireccion == 0)
                {
                    NewX = item.GetX;
                    NewY = item.GetY - 1;

                }
                else if (item.BallDireccion == 2)
                {
                    NewX = item.GetX + 1;
                    NewY = item.GetY;

                }
                else if (item.BallDireccion == 1)
                {
                    NewX = item.GetX + 1;
                    NewY = item.GetY - 1;

                }
                else if (item.BallDireccion == 7)
                {
                    NewX = item.GetX - 1;
                    NewY = item.GetY - 1;

                }
                else if (item.BallDireccion == 3)
                {
                    NewX = item.GetX + 1;
                    NewY = item.GetY + 1;

                }
                else if (item.BallDireccion == 5)
                {
                    NewX = item.GetX - 1;
                    NewY = item.GetY + 1;
                }
                if (_room.GetGameMap().SquareHasUser3(NewX, NewY, User))
                {
                    ignore = true;
                    if (item.ballsteps > 2)
                    {
                        item.ejex = false;
                        item.ejey = false;
                        item.ballstop = true;
                    }
                }
                if (item.GetRoom().GetGameMap().ValidTile(NewX, NewY, item) && !item.ballstop)
                {
                    MoveBall(item, NewX, NewY, User, true, ignore);
                }
                Task t = Task.Factory.StartNew(() => TaskWaiting());
                t.Wait();
                ignore = false;
                item.ballsteps = item.ballsteps + 1;
            }

        }
        public void MoveBallBigShoot(RoomUser User, Item item, int chute)
        {
            int NewX = 0;
            int NewY = 0;
            item.ballsteps = 0;
            item.ballstop = false;
            item.ejex = false;
            item.ejey = false;
            item.BallDireccion = User.RotBody;
            bool ignore = false;
            while (item.ballsteps < chute && !item.ballstop)
            {
                if (item.BallDireccion == 4)
                {
                    NewX = item.GetX;
                    NewY = item.GetY + 1;

                }
                else if (item.BallDireccion == 6)
                {
                    NewX = item.GetX - 1;
                    NewY = item.GetY;

                }
                else if (item.BallDireccion == 0)
                {
                    NewX = item.GetX;
                    NewY = item.GetY - 1;

                }
                else if (item.BallDireccion == 2)
                {
                    NewX = item.GetX + 1;
                    NewY = item.GetY;

                }
                else if (item.BallDireccion == 1)
                {
                    NewX = item.GetX + 1;
                    NewY = item.GetY - 1;

                }
                else if (item.BallDireccion == 7)
                {
                    NewX = item.GetX - 1;
                    NewY = item.GetY - 1;

                }
                else if (item.BallDireccion == 3)
                {
                    NewX = item.GetX + 1;
                    NewY = item.GetY + 1;

                }
                else if (item.BallDireccion == 5)
                {
                    NewX = item.GetX - 1;
                    NewY = item.GetY + 1;
                }
                if (_room.GetGameMap().SquareHasUser3(NewX, NewY, User))
                {
                    ignore = true;
                    if (item.ballsteps > 2)
                    {
                        item.ejex = false;
                        item.ejey = false;
                        item.ballstop = true;
                    }
                }
                if (item.GetRoom().GetGameMap().ValidTile(NewX, NewY, item) && !item.ballstop)
                {
                    MoveBall(item, NewX, NewY, User, true, ignore);
                }
                Task t = Task.Factory.StartNew(() => TaskWaiting());
                t.Wait();
                ignore = false;
                item.ballsteps = item.ballsteps + 1;
            }

        }
        public void MoveBallNormalShoot(RoomUser User, Item item, int chute)
        {
            int NewX = 0;
            int NewY = 0;


            if (User.RotBody == 4)
            {
                NewX = item.GetX;
                NewY = item.GetY + chute;

            }
            else if (User.RotBody == 6)
            {
                NewX = item.GetX - chute;
                NewY = item.GetY;

            }
            else if (User.RotBody == 0)
            {
                NewX = item.GetX;
                NewY = item.GetY - chute;

            }
            else if (User.RotBody == 2)
            {
                NewX = item.GetX + chute;
                NewY = item.GetY;

            }
            else if (User.RotBody == 1)
            {
                NewX = item.GetX + chute;
                NewY = item.GetY - chute;

            }
            else if (User.RotBody == 7)
            {
                NewX = item.GetX - chute;
                NewY = item.GetY - chute;

            }
            else if (User.RotBody == 3)
            {
                NewX = item.GetX + chute;
                NewY = item.GetY + chute;

            }
            else if (User.RotBody == 5)
            {
                NewX = item.GetX - chute;
                NewY = item.GetY + chute;
            }

            if (item.GetRoom().GetGameMap().ValidTile(NewX, NewY))
            {
                MoveBall(item, NewX, NewY, User, false);
            }

        }
        public void MoveBallInverseNormalShoot(RoomUser User, Item item, int chute)
        {
            int NewX = 0;
            int NewY = 0;
            User.UserOnBall = false;
            if (User.RotBody == 4)
            {
                NewX = User.X;
                NewY = item.GetY - chute;

            }
            else if (User.RotBody == 6)
            {
                NewX = item.GetX + chute;
                NewY = item.GetY;

            }
            else if (User.RotBody == 0)
            {
                NewX = item.GetX;
                NewY = item.GetY + chute;

            }
            else if (User.RotBody == 2)
            {
                NewX = item.GetX - chute;
                NewY = item.GetY;

            }
            else if (User.RotBody == 1)
            {
                NewX = item.GetX - chute;
                NewY = item.GetY + chute;

            }
            else if (User.RotBody == 7)
            {
                NewX = item.GetX + chute;
                NewY = item.GetY + chute;

            }
            else if (User.RotBody == 3)
            {
                NewX = item.GetX - chute;
                NewY = item.GetY - chute;

            }
            else if (User.RotBody == 5)
            {
                NewX = item.GetX + chute;
                NewY = item.GetY - chute;
            }
            if (item.GetRoom().GetGameMap().ValidTile(NewX, NewY))
            {
                MoveBall(item, NewX, NewY, User, false);
            }

        }
        public void OnUserWalk(RoomUser User)
        {
            if (User == null)
                return;

            foreach (Item item in this._balls.Values.ToList())
            {
                item.ballstop = false;
                bool goalxy = User.GoalX == item.GetX && User.GoalY == item.GetY;
                int chute = 1;
                int distancex = Math.Abs(User.X - item.GetX);
                int distancey = Math.Abs(User.Y - item.GetY);

                if (distancex > 1 || distancey > 1)
                    User.UserHandlingBall = false;

                if (goalxy && User.SetX == item.GetX && User.SetY == item.GetY && (User.RotBody != User.LastRotBody || !User.UserHandlingBall))//&& (!User.UserHandlingBall || User.RotBody != User.LastRotBody))
                {
                    chute = 6;
                    Task t1 = Task.Factory.StartNew(() => MoveBallBigShoot(User, item, chute));
                }
                else if (User.SetX == item.GetX && User.SetY == item.GetY && (User.DistancePath != 2 && User.Path.Count != 3))
                {
                    User.UserHandlingBall = true;
                    Task t2 = Task.Factory.StartNew(() => MoveBallNormalShoot(User, item, chute));
                }
                else if (User.SetX == item.GetX && User.SetY == item.GetY && User.Path.Count == 3)
                {
                    User.UserHandlingBall = true;
                    Task t5 = Task.Factory.StartNew(() => MoveBallNormalShoot(User, item, 1));
                }

                if (User.UserOnBall && User.X == item.GetX && User.Y == item.GetY)
                {
                    User.UserHandlingBall = true;
                    if (User.Path.Count == 2)
                    {
                        chute = 6;
                        Task t3 = Task.Factory.StartNew(() => MoveBallInverseBigShoot(User, item, chute));

                    }
                    else
                    {
                        Task t4 = Task.Factory.StartNew(() => MoveBallInverseNormalShoot(User, item, chute));
                    }
                }
            }
        }

        private bool VerifyBall(RoomUser user, int actualx, int actualy)
        {
            return Rotation.Calculate(user.X, user.Y, actualx, actualy) == user.RotBody;
        }

        public void RegisterGate(Item item)
        {
            if (gates[0] == null)
            {
                item.team = TEAM.BLUE;
                gates[0] = item;
            }
            else if (gates[1] == null)
            {
                item.team = TEAM.RED;
                gates[1] = item;
            }
            else if (gates[2] == null)
            {
                item.team = TEAM.GREEN;
                gates[2] = item;
            }
            else if (gates[3] == null)
            {
                item.team = TEAM.YELLOW;
                gates[3] = item;
            }
        }

        public void UnRegisterGate(Item item)
        {
            switch (item.team)
            {
                case TEAM.BLUE:
                    {
                        gates[0] = null;
                        break;
                    }
                case TEAM.RED:
                    {
                        gates[1] = null;
                        break;
                    }
                case TEAM.GREEN:
                    {
                        gates[2] = null;
                        break;
                    }
                case TEAM.YELLOW:
                    {
                        gates[3] = null;
                        break;
                    }
            }
        }

        public void onGateRemove(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.FOOTBALL_GOAL_RED:
                case InteractionType.footballcounterred:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.RED);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_GREEN:
                case InteractionType.footballcountergreen:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.GREEN);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_BLUE:
                case InteractionType.footballcounterblue:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.BLUE);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_YELLOW:
                case InteractionType.footballcounteryellow:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.YELLOW);
                        break;
                    }
            }
        }

        public void MoveBall(Item item, int newX, int newY, RoomUser user, bool bigshoot = false, bool ignore = false)
        {
            if (item == null || user == null)
                return;

            user.UserHandlingBall = true;

            if (!ignore)
            {
                if (!_room.GetGameMap().itemCanBePlacedHere(newX, newY, item, user))
                {
                    return;
                }
            }
            if (_room.GetGameMap().SquareHasUser3(newX, newY, user) && !bigshoot)
            {
                item.ejex = false;
                item.ejey = false;
                item.ballstop = true;
                return;
            }
            //if (rand == 0)
            //{

            //    if (_room.GetGameMap().SquareHasUser3(newX, newY, user))
            //    {
            //        item.ejex = false;
            //        item.ejey = false;
            //        item.ballstop = true;
            //        return;
            //    }
            //}

            Point oldRoomCoord = item.Coordinate;
            if (oldRoomCoord.X == newX && oldRoomCoord.Y == newY)
            {
                return;
            }

            double NewZ = _room.GetGameMap().Model.SqFloorHeight[newX, newY];

            _room.SendMessage(new SlideObjectBundleComposer(item.Coordinate.X, item.Coordinate.Y, item.GetZ, newX, newY, NewZ, item.Id, item.Id, item.Id));
            item.ExtraData = "55";
            item.UpdateNeeded = true;
            _room.GetRoomItemHandler().SetFloorItem_ball(user.GetClient(), item, newX, newY, item.Rotation, false, false, false, false, true); //aca

            this._room.OnUserShoot(user, item);
        }
        public void TaskWaiting()
        {
            Thread.Sleep(175);
        }

        public void Dispose()
        {
            Array.Clear(gates, 0, gates.Length);
            gates = null;
            _room = null;
            _balls.Clear();
            _balls = null;
        }
    }
}