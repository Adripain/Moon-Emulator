using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;
using System.Drawing;
using System.Security.Cryptography;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;
using Moon.Utilities;
using System.Collections;

namespace Moon.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class MoveUserBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }

        public WiredBoxType Type
        {
            get { return WiredBoxType.EffectMoveUser; }
        }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }

        public int Delay
        {
            get { return this._delay; }
            set
            {
                this._delay = value;
                this.TickCount = value + 1;
            }
        }

        public int TickCount { get; set; }
        public string ItemsData { get; set; }
        private bool Requested;
        private int _delay = 0;
        private long _next = 0;

        private Queue _queue;

        public MoveUserBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();

            this._queue = new Queue();
            this.TickCount = Delay;
        }

        public void HandleSave(ClientPacket Packet)
        {
            if (this.SetItems.Count > 0)
                this.SetItems.Clear();

            int Unknown = Packet.PopInt();
            int Movement = Packet.PopInt();
            int Rotation = Packet.PopInt();

            string Unknown1 = Packet.PopString();

            int FurniCount = Packet.PopInt();
            for (int i = 0; i < FurniCount; i++)
            {
                Item SelectedItem = Instance.GetRoomItemHandler().GetItem(Packet.PopInt());

                if (SelectedItem != null && !Instance.GetWired().OtherBoxHasItem(this, SelectedItem.Id))
                    SetItems.TryAdd(SelectedItem.Id, SelectedItem);
            }

            this.StringData = Movement + ";" + Rotation;
            this.Delay = Packet.PopInt();
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

            Habbo Player = (Habbo)Params[0];

            if (Player == null)
                return false;

            Player.LastEffect = Player.Effects().CurrentEffect;

            if (Player.Effects() != null)
                Player.Effects().ApplyEffect(4);

            this._queue.Enqueue(Player);
            return true;
        }

        public bool OnCycle()
        {

            while (_queue.Count > 0)
            {
                Habbo Player = (Habbo)_queue.Dequeue();
                if (Player == null || Player.CurrentRoom != Instance)
                    continue;

                this.MoveUser(Player);
            }

            this.TickCount = Delay;
            return true;
        }

        private void MoveUser(Habbo Player)
        {
            if (Player == null)
                return;

            Room Room = Player.CurrentRoom;
            if (Room == null)
                return;

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
                return;

            if (Player.IsTeleporting || Player.IsHopping || Player.TeleporterId != 0)
                return;

            if (Room.GetGameMap() == null)
                return;

            Point Point = HandleMovement(Convert.ToInt32(this.StringData.Split(';')[0]), new Point(User.X, User.Y));
            int newRot = HandleRotation(Convert.ToInt32(this.StringData.Split(';')[1]), Item.Rotation, User.RotBody);

            User.MoveTo(Point);
            User.SetRot(newRot, false);

            if (Player.Effects() != null && Player.LastEffect == 0 || Player.LastEffect == 4)
                Player.Effects().ApplyEffect(0);
            else
                Player.Effects().ApplyEffect(Player.LastEffect);
        }

        private int HandleRotation(int mode, int rotation, int rotuser)
        {
            switch (mode)
            {
                case 1:
                    {
                        /*if (rotuser == 0)
                            rotation = 1;
                        if (rotuser == 1)
                            rotation = 2;
                        if (rotuser == 2)
                            rotation = 3;
                        if (rotuser == 3)
                            rotation = 4;
                        if (rotuser == 4)
                            rotation = 5;
                        if (rotuser == 5)
                            rotation = 6;
                        if (rotuser == 6)
                            rotation = 7;
                        if (rotuser == 7)
                            rotation = 0;*/
                        /*if (rotuser == 0 || rotuser == 1)
                            rotation = 2;
                        if (rotuser == 2 || rotuser == 3)
                            rotation = 4;
                        if (rotuser == 4 || rotuser == 5)
                            rotation = 6;
                        if (rotuser == 6 || rotuser == 7)
                            rotation = 0;*/
                        if (rotuser == 0 || rotuser == 7)
                            rotation = 2;
                        if (rotuser == 2 || rotuser == 1)
                            rotation = 4;
                        if (rotuser == 4 || rotuser == 3)
                            rotation = 6;
                        if (rotuser == 6 || rotuser == 5)
                            rotation = 0;
                        break;
                    }

                case 2:
                    {
                        /*if (rotuser == 0)
                             rotation = 7;
                         if (rotuser == 7)
                             rotation = 6;
                         if (rotuser == 6)
                             rotation = 5;
                         if (rotuser == 5)
                             rotation = 4;
                         if (rotuser == 4)
                             rotation = 3;
                         if (rotuser == 3)
                             rotation = 2;
                         if (rotuser == 2)
                             rotation = 1;
                         if (rotuser == 1)
                             rotation = 0;*/
                        /*if (rotuser == 0 || rotuser == 7)
                            rotation = 6;
                        if (rotuser == 6 || rotuser == 5)
                            rotation = 4;
                        if (rotuser == 4 || rotuser == 3)
                            rotation = 2;
                        if (rotuser == 2 || rotuser == 1)
                            rotation = 0;*/
                        if (rotuser == 0 || rotuser == 1)
                            rotation = 6;
                        if (rotuser == 6 || rotuser == 3)
                            rotation = 4;
                        if (rotuser == 4 || rotuser == 5)
                            rotation = 2;
                        if (rotuser == 2 || rotuser == 7)
                            rotation = 0;
                        break;
                    }

                case 3:

                    switch (RandomNumber.GenerateRandom(1, 8))
                    {
                        case 1:
                            rotation = 0;
                            break;
                        case 2:
                            rotation = 1;
                            break;
                        case 3:
                            rotation = 2;
                            break;
                        case 4:
                            rotation = 3;
                            break;
                        case 5:
                            rotation = 4;
                            break;
                        case 6:
                            rotation = 5;
                            break;
                        case 7:
                            rotation = 6;
                            break;
                        case 8:
                            rotation = 7;
                            break;
                    }
                    break;

            }
            return rotation;
        }

        private Point HandleMovement(int Mode, Point Position)
        {
            Point NewPos = new Point();
            switch (Mode)
            {
                case 0:
                    {
                        NewPos = Position;
                        break;
                    }
                case 1:
                    {
                        switch (RandomNumber.GenerateRandom(1, 8))
                        {
                            case 1:
                                NewPos = new Point(Position.X + 1, Position.Y);
                                break;
                            case 2:
                                NewPos = new Point(Position.X - 1, Position.Y);
                                break;
                            case 3:
                                NewPos = new Point(Position.X, Position.Y + 1);
                                break;
                            case 4:
                                NewPos = new Point(Position.X, Position.Y - 1);
                                break;
                            case 5:
                                NewPos = new Point(Position.X + 1, Position.Y + 1);
                                break;
                            case 6:
                                NewPos = new Point(Position.X - 1, Position.Y - 1);
                                break;
                            case 7:
                                NewPos = new Point(Position.X - 1, Position.Y + 1);
                                break;
                            case 8:
                                NewPos = new Point(Position.X + 1, Position.Y - 1);
                                break;
                        }
                        break;
                    }
                case 2:
                    {
                        if (RandomNumber.GenerateRandom(0, 2) == 1)
                        {
                            NewPos = new Point(Position.X - 1, Position.Y);
                        }
                        else
                        {
                            NewPos = new Point(Position.X + 1, Position.Y);
                        }
                        break;
                    }
                case 3:
                    {
                        if (RandomNumber.GenerateRandom(0, 2) == 1)
                        {
                            NewPos = new Point(Position.X, Position.Y - 1);
                        }
                        else
                        {
                            NewPos = new Point(Position.X, Position.Y + 1);
                        }
                        break;
                    }
                case 4:
                    {
                        NewPos = new Point(Position.X, Position.Y - 1);
                        break;
                    }
                case 5:
                    {
                        NewPos = new Point(Position.X + 1, Position.Y);
                        break;
                    }
                case 6:
                    {
                        NewPos = new Point(Position.X, Position.Y + 1);
                        break;
                    }
                case 7:
                    {
                        NewPos = new Point(Position.X - 1, Position.Y);
                        break;
                    }
            }

            return NewPos;
        }
    }
}