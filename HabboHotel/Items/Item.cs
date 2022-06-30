using System;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Collections.Generic;

using Moon.Core;
using Moon.HabboHotel.Groups;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Pathfinding;
using Moon.HabboHotel.Rooms.Games;
using Moon.HabboHotel.Items.Interactor;


using Moon.Utilities;
using Moon.HabboHotel.Users;
using Moon.HabboHotel.Rooms.Games.Freeze;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;
using Moon.Communication.Packets.Outgoing.Rooms.Session;


using Moon.HabboHotel.Rooms.Games.Teams;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Football;
using Moon.HabboHotel.Items.Wired.Util;

namespace Moon.HabboHotel.Items
{

    public class Item
    {

        public long NextCommand = 0;
        public int Id;
        public int ballsteps = 0;
        public bool ballstop = false;
        public int BallDireccion;
        public bool ejex;
        public bool ejey;
        private ItemData _data;
        public int BaseItem;
        public string ExtraData;
        public string Figure;
        public string Gender;
        public int GroupId;
        public bool BallIsMoving;
        public int BallValue;
        public int InteractingUser;
        public int InteractingUser2;
        public int LimitedNo;
        public int LimitedTot;
        public bool MagicRemove = false;
        public int RoomId;
        public int Rotation;
        public int UpdateCounter;
        public string FoundBy;
        public int UserID;
        public string Username;
        public int interactingBallUser;
        public byte interactionCount;
        public byte interactionCountHelper;



        private int _coordX;
        private int _coordY;
        private double _coordZ;

        public TEAM team;
        public bool pendingReset = false;
        public FreezePowerUp freezePowerUp;


        public int value;
        public string wallCoord;
        private bool updateNeeded;

        private Room _room;
        private static Random _random = new Random();
        private Dictionary<int, ThreeDCoord> _affectedPoints;

        private readonly bool mIsRoller;
        private readonly bool mIsWallItem;
        private readonly bool mIsFloorItem;

        internal int BallTryGoThrough;
        internal GameClient ballMover;
        internal bool ballIsMoving;
        internal bool Shoot = false;
        internal IComeDirection comeDirection;
        public MovementDirection MoveToDirMovement = MovementDirection.NONE;

        public bool isBCitem;

        //public int ExtradataInt
        //{
        //    get
        //    {
        //        int i;
        //        if (!int.TryParse(ExtraData, out i))
        //            return 0;
        //        return i;
        //    }
        //}

        public int ExtradataInt
        {
            get
            {
                int num;
                bool flag = !int.TryParse(this.ExtraData, out num);
                int result;
                if (flag)
                {
                    result = 0;
                }
                else
                {
                    result = num;
                }
                return result;
            }
        }

        public Item(int Id, int RoomId, int BaseItem, string ExtraData, int X, int Y, Double Z, int Rot, int Userid, int Group, int limitedNumber, int limitedStack, string wallCoord, Room Room = null)
        {
            ItemData Data = null;
            if (MoonEnvironment.GetGame().GetItemManager().GetItem(BaseItem, out Data))
            {
                this.Id = Id;
                this.RoomId = RoomId;
                this._room = Room;
                this._data = Data;
                this.BaseItem = BaseItem;
                this.ExtraData = ExtraData;
                this.GroupId = Group;

                this._coordX = X;
                this._coordY = Y;
                if (!double.IsInfinity(Z))
                    this._coordZ = Z;
                this.Rotation = Rot;
                this.UpdateNeeded = false;
                this.UpdateCounter = 0;
                this.InteractingUser = 0;
                this.InteractingUser2 = 0;
                this.interactingBallUser = 0;
                this.interactionCount = 0;
                this.value = 0;

                this.UserID = Userid;
                this.Username = MoonEnvironment.GetUsernameById(Userid);


                this.LimitedNo = limitedNumber;
                this.LimitedTot = limitedStack;

                

                switch (GetBaseItem().InteractionType)
                {
                    case InteractionType.TELEPORT:
                        RequestUpdate(0, true);
                        break;

                    case InteractionType.HOPPER:
                        RequestUpdate(0, true);
                        break;

                    case InteractionType.ROLLER:
                        mIsRoller = true;
                        if (RoomId > 0)
                        {
                            GetRoom().GetRoomItemHandler().GotRollers = true;
                        }
                        break;

                    case InteractionType.banzaiscoreblue:
                    case InteractionType.footballcounterblue:
                    case InteractionType.banzaigateblue:
                    case InteractionType.FREEZE_BLUE_GATE:
                    case InteractionType.freezebluecounter:
                        team = TEAM.BLUE;
                        break;

                    case InteractionType.banzaiscoregreen:
                    case InteractionType.footballcountergreen:
                    case InteractionType.banzaigategreen:
                    case InteractionType.freezegreencounter:
                    case InteractionType.FREEZE_GREEN_GATE:
                        team = TEAM.GREEN;
                        break;

                    case InteractionType.banzaiscorered:
                    case InteractionType.footballcounterred:
                    case InteractionType.banzaigatered:
                    case InteractionType.freezeredcounter:
                    case InteractionType.FREEZE_RED_GATE:
                        team = TEAM.RED;
                        break;

                    case InteractionType.banzaiscoreyellow:
                    case InteractionType.footballcounteryellow:
                    case InteractionType.banzaigateyellow:
                    case InteractionType.freezeyellowcounter:
                    case InteractionType.FREEZE_YELLOW_GATE:
                        team = TEAM.YELLOW;
                        break;

                    case InteractionType.banzaitele:
                        {
                            this.ExtraData = "";
                            break;
                        }
                }

                this.mIsWallItem = (GetBaseItem().Type.ToString().ToLower() == "i");
                this.mIsFloorItem = (GetBaseItem().Type.ToString().ToLower() == "s");

                if (this.mIsFloorItem)
                {
                    this._affectedPoints = Gamemap.GetAffectedTiles(GetBaseItem().Length, GetBaseItem().Width, GetX, GetY, Rot);
                }
                else if (this.mIsWallItem)
                {
                    this.wallCoord = wallCoord;
                    this.mIsWallItem = true;
                    this.mIsFloorItem = false;
                    this._affectedPoints = new Dictionary<int, ThreeDCoord>();
                }
            }
        }

        public ItemData Data
        {
            get { return this._data; }
            set { this._data = value; }
        }

        public Dictionary<int, ThreeDCoord> GetAffectedTiles
        {
            get { return this._affectedPoints; }
        }

        public int GetX
        {
            get { return _coordX; }
            set { this._coordX = value; }
        }

        public int GetY
        {
            get { return _coordY; }
            set { this._coordY = value; }
        }

        public double GetZ
        {
            get { return _coordZ; }
            set { this._coordZ = value; }
        }

        public bool UpdateNeeded
        {
            get { return updateNeeded; }
            set
            {
                if (value && GetRoom() != null)
                    GetRoom().GetRoomItemHandler().QueueRoomItemUpdate(this);
                updateNeeded = value;
            }
        }

        public bool IsRoller
        {
            get { return mIsRoller; }
        }

        public Point Coordinate
        {
            get { return new Point(GetX, GetY); }
        }

        public List<Point> GetCoords
        {
            get
            {
                var toReturn = new List<Point>();
                toReturn.Add(Coordinate);

                foreach (ThreeDCoord tile in _affectedPoints.Values)
                {
                    toReturn.Add(new Point(tile.X, tile.Y));
                }

                return toReturn;
            }
        }

        public List<Point> GetSides()
        {
            var toReturn = new List<Point>();
            toReturn.Add(SquareBehind);
            toReturn.Add(SquareInFront);
            toReturn.Add(SquareLeft);
            toReturn.Add(SquareRight);
            toReturn.Add(Coordinate);
            return toReturn;
        }

        public double TotalHeight
        {
            get
            {
                double CurHeight = 0.0;
                int num2;

                if (this.GetBaseItem().AdjustableHeights.Count > 1)
                {
                    if (int.TryParse(this.ExtraData, out num2) && (this.GetBaseItem().AdjustableHeights.Count) - 1 >= num2)
                        CurHeight = this.GetZ + this.GetBaseItem().AdjustableHeights[num2];
                }

                if (CurHeight <= 0.0)
                    CurHeight = this.GetZ + this.GetBaseItem().Height;

                return CurHeight;
            }
        }

        public bool IsWallItem
        {
            get { return mIsWallItem; }
        }

        public bool IsFloorItem
        {
            get { return mIsFloorItem; }
        }
        public bool IsBuilderItem
        {
            get { return isBCitem; }
        }

        public Point SquareInFront
        {
            get
            {
                var Sq = new Point(GetX, GetY);

                if (Rotation == 0)
                {
                    Sq.Y--;
                }
                else if (Rotation == 2)
                {
                    Sq.X++;
                }
                else if (Rotation == 4)
                {
                    Sq.Y++;
                }
                else if (Rotation == 6)
                {
                    Sq.X--;
                }

                return Sq;
            }
        }

        public Point SquareBehind
        {
            get
            {
                var Sq = new Point(GetX, GetY);

                if (Rotation == 0)
                {
                    Sq.Y++;
                }
                else if (Rotation == 2)
                {
                    Sq.X--;
                }
                else if (Rotation == 4)
                {
                    Sq.Y--;
                }
                else if (Rotation == 6)
                {
                    Sq.X++;
                }

                return Sq;
            }
        }

        public Point SquareLeft
        {
            get
            {
                var Sq = new Point(GetX, GetY);

                if (Rotation == 0)
                {
                    Sq.X++;
                }
                else if (Rotation == 2)
                {
                    Sq.Y--;
                }
                else if (Rotation == 4)
                {
                    Sq.X--;
                }
                else if (Rotation == 6)
                {
                    Sq.Y++;
                }

                return Sq;
            }
        }

        public Point SquareRight
        {
            get
            {
                var Sq = new Point(GetX, GetY);

                if (Rotation == 0)
                {
                    Sq.X--;
                }
                else if (Rotation == 2)
                {
                    Sq.Y++;
                }
                else if (Rotation == 4)
                {
                    Sq.X++;
                }
                else if (Rotation == 6)
                {
                    Sq.Y--;
                }
                return Sq;
            }
        }

        public IFurniInteractor Interactor
        {
            get
            {
                if (IsWired)
                {
                    return new InteractorWired();
                }

                switch (GetBaseItem().InteractionType)
                {
                    case InteractionType.JUKEBOX: // Added
                        return new InteractorJukebox();

                    case InteractionType.CRACKABLE: // Added
                        return new InteractorCrackable();

                    case InteractionType.CAJAFORBI: // Added
                        return new InteractorCajaForbi();

                    case InteractionType.GATE:
                        return new InteractorGate();

                    case InteractionType.TELEPORT:
                        return new InteractorTeleport();

                    case InteractionType.HOPPER:
                        return new InteractorHopper();

                    case InteractionType.BOTTLE:
                        return new InteractorSpinningBottle();

                    case InteractionType.CRAFTING:
                        return new InteractorCrafting();

                    case InteractionType.DICE:
                        return new InteractorDice();

                    case InteractionType.DICE2:
                        return new InteractorDice();

                    case InteractionType.HABBO_WHEEL:
                        return new InteractorHabboWheel();

                    case InteractionType.RENTABLE_SPACE:
                        return new InteractorRentableSpace();

                    case InteractionType.LOVE_SHUFFLER:
                        return new InteractorLoveShuffler();

                    case InteractionType.WMTOTEM:
                        return new InteractorWmTotem();

                    case InteractionType.ONE_WAY_GATE:
                        return new InteractorOneWayGate();

                    case InteractionType.ALERT:
                        return new InteractorAlert();

                    case InteractionType.VENDING_MACHINE:
                        return new InteractorVendor();

                    case InteractionType.idol_counter:
                        return new IdolCounter();

                    case InteractionType.SCOREBOARD:
                        return new InteractorScoreboard();

                    case InteractionType.PUZZLE_BOX:
                        return new InteractorPuzzleBox();

                    case InteractionType.MANNEQUIN:
                        return new InteractorMannequin();

                    case InteractionType.MYSTICBOX:
                        return new InteractorMysticBox();

                    case InteractionType.banzaicounter:
                        return new InteractorBanzaiTimer();

                    case InteractionType.EASTEREGG:
                        return new InteractorEasterEgg();

                    case InteractionType.freezetimer:
                        return new InteractorFreezeTimer();

                    case InteractionType.FREEZE_TILE_BLOCK:
                    case InteractionType.FREEZE_TILE:
                        return new InteractorFreezeTile();

                    //case InteractionType.FOOTBALL:
                      //  return new InteractorFootball();

                    case InteractionType.footballcounterblue:
                    case InteractionType.footballcountergreen:
                    case InteractionType.footballcounterred:
                    case InteractionType.footballcounteryellow:
                        return new InteractorScoreCounter();

                    case InteractionType.banzaiscoreblue:
                    case InteractionType.banzaiscoregreen:
                    case InteractionType.banzaiscorered:
                    case InteractionType.banzaiscoreyellow:
                        return new InteractorBanzaiScoreCounter();

                    case InteractionType.WF_FLOOR_SWITCH_1:
                    case InteractionType.WF_FLOOR_SWITCH_2:
                        return new InteractorSwitch();

                    case InteractionType.LOVELOCK:
                        return new InteractorLoveLock();


                    case InteractionType.PRESSURE_PAD:
                        return new InteractorPressurePad();

                    case InteractionType.ROOM_PROVIDER:
                        return new InteractorRoomProvider();

                    case InteractionType.CANNON:
                        return new InteractorCannon();

                    case InteractionType.PINATATRIGGERED:
                        return new InteractorPinata();

                    case InteractionType.COUNTER:
                        return new InteractorCounter();

                    case InteractionType.alert_furniture:
                        return new InteractorAlertFurniture();

                    case InteractionType.MUTESIGNAL:
                        return new InteractorMuteSignal();

                    case InteractionType.vikingtent:
                        return new InteractorViking();

                    case InteractionType.SLOT_MACHINE:
                        return new InteractorSlotmachine();

                    case InteractionType.PLANT_SEED:
                        return new InteractorPlantSeed();

                    case InteractionType.MAGICEGG:
                        return new InteractorMagicEgg();

                    case InteractionType.NONE:
                    default:
                        return new InteractorGenericSwitch();
                }
            }
        }

        public bool IsWired
        {
            get
            {
                switch (GetBaseItem().InteractionType)
                {
                    case InteractionType.WIRED_EFFECT:
                    case InteractionType.WIRED_TRIGGER:
                    case InteractionType.WIRED_CONDITION:
                        return true;
                }

                return false;
            }
        }

        public void RegenerateBlock(string NewMode, Gamemap Tile)
        {
            try
            {
                List<RoomUser> list = new List<RoomUser>();
                int CurrentMode = 0;

                if (!int.TryParse(NewMode, out CurrentMode))
                {
                }

                if (CurrentMode <= 0)
                {
                    foreach (RoomUser user in _room.GetGameMap().GetRoomUsers(new Point(this.GetX, this.GetY)))
                    {
                        user.SqState = 0;
                    }
                    _room.GetGameMap().GameMap[this.GetX, this.GetY] = 0;
                }
            }
            catch
            {
            }
        }

        public void SetState(int pX, int pY, Double pZ, Dictionary<int, ThreeDCoord> Tiles)
        {
            GetX = pX;
            GetY = pY;
            if (!double.IsInfinity(pZ))
            {
                _coordZ = pZ;
            }
            _affectedPoints = Tiles;
        }

        public void Destroy()
        {
            _affectedPoints.Clear();
        }

        public void ProcessUpdates()
        {
            if (this == null)
                return;

            try
            {
                UpdateCounter--;

                if (UpdateCounter <= 0)
                {
                    UpdateNeeded = false;
                    UpdateCounter = 0;

                    RoomUser User = null;
                    RoomUser User2 = null;

                    switch (GetBaseItem().InteractionType)
                    {
                        #region Group Gates
                        case InteractionType.GUILD_GATE:
                            {
                                if (ExtraData == "1")
                                {
                                    if (GetRoom().GetRoomUserManager().GetUserForSquare(GetX, GetY) == null)
                                    {
                                        ExtraData = "0";
                                        UpdateState(false, true);
                                    }
                                    else
                                    {
                                        RequestUpdate(2, false);
                                    }
                                }
                                break;
                            }
                        #endregion

                        #region HC Gate
                        case InteractionType.HCGATE:
                        case InteractionType.VIPGATE:
                            {
                                if (ExtraData == "1")
                                {
                                    if (GetRoom().GetRoomUserManager().GetUserForSquare(GetX, GetY) == null)
                                    {
                                        ExtraData = "0";
                                        UpdateState(false, true);
                                    }
                                    else
                                    {
                                        RequestUpdate(2, false);
                                    }
                                }
                                break;
                            }
                        #endregion


                        #region Item Effects
                        //case InteractionType.FX_PROVIDER:
                        //    {
                        //        if (ExtraData == "1")
                        //        {
                        //            if (GetRoom().GetRoomUserManager().GetUserForSquare(GetX, GetY) == null)
                        //            {
                        //                ExtraData = "0";
                        //                UpdateState(false, true);
                        //            }
                        //            else
                        //            {
                        //                RequestUpdate(2, false);
                        //            }
                        //        }
                        //        break;
                        //    }
                        #endregion

                        #region One way gates
                        case InteractionType.ONE_WAY_GATE:

                            User = null;

                            if (InteractingUser > 0)
                            {
                                User = GetRoom().GetRoomUserManager().GetRoomUserByHabbo(InteractingUser);
                            }

                            if (User != null && User.X == GetX && User.Y == GetY)
                            {
                                ExtraData = "1";

                                User.MoveTo(SquareBehind);
                                User.InteractingGate = false;
                                User.GateId = 0;
                                RequestUpdate(1, false);
                                UpdateState(false, true);
                            }
                            else if (User != null && User.Coordinate == SquareBehind)
                            {
                                User.UnlockWalking();

                                ExtraData = "0";
                                InteractingUser = 0;
                                User.InteractingGate = false;
                                User.GateId = 0;
                                UpdateState(false, true);
                            }
                            else if (ExtraData == "1")
                            {
                                ExtraData = "0";
                                UpdateState(false, true);
                            }

                            if (User == null)
                            {
                                InteractingUser = 0;
                            }

                            break;
                        #endregion

                        #region VIP Gate
                        case InteractionType.GATE_VIP:

                            User = null;


                            if (InteractingUser > 0)
                            {
                                User = GetRoom().GetRoomUserManager().GetRoomUserByHabbo(InteractingUser);
                            }

                            int NewY = 0;
                            int NewX = 0;

                            if (User != null && User.X == GetX && User.Y == GetY)
                            {
                                if (User.RotBody == 4)
                                {
                                    NewY = 1;
                                }
                                else if (User.RotBody == 0)
                                {
                                    NewY = -1;
                                }
                                else if (User.RotBody == 6)
                                {
                                    NewX = -1;
                                }
                                else if (User.RotBody == 2)
                                {
                                    NewX = 1;
                                }


                                User.MoveTo(User.X + NewX, User.Y + NewY);
                                RequestUpdate(1, false);
                            }
                            else if (User != null && (User.Coordinate == SquareBehind || User.Coordinate == SquareInFront))
                            {
                                User.UnlockWalking();

                                ExtraData = "0";
                                InteractingUser = 0;
                                UpdateState(false, true);
                            }
                            else if (ExtraData == "1")
                            {
                                ExtraData = "0";
                                UpdateState(false, true);
                            }

                            if (User == null)
                            {
                                InteractingUser = 0;
                            }

                            break;
                        #endregion

                        #region Hopper
                        case InteractionType.HOPPER:
                            {
                                User = null;
                                User2 = null;
                                bool showHopperEffect = false;
                                bool keepDoorOpen = false;
                                int Pause = 0;


                                // Do we have a primary user that wants to go somewhere?
                                if (InteractingUser > 0)
                                {
                                    User = GetRoom().GetRoomUserManager().GetRoomUserByHabbo(InteractingUser);

                                    // Is this user okay?
                                    if (User != null)
                                    {
                                        // Is he in the tele?
                                        if (User.Coordinate == Coordinate)
                                        {
                                            //Remove the user from the square
                                            User.AllowOverride = false;
                                            if (User.TeleDelay == 0)
                                            {
                                                int RoomHopId = ItemHopperFinder.GetAHopper(User.RoomId);
                                                int NextHopperId = ItemHopperFinder.GetHopperId(RoomHopId);

                                                if (!User.IsBot && User != null && User.GetClient() != null &&
                                                    User.GetClient().GetHabbo() != null)
                                                {
                                                    User.GetClient().GetHabbo().IsHopping = true;
                                                    User.GetClient().GetHabbo().HopperId = NextHopperId;
                                                    User.GetClient().GetHabbo().PrepareRoom(RoomHopId, "");
                                                    //User.GetClient().SendMessage(new RoomForwardComposer(RoomHopId));
                                                    InteractingUser = 0;
                                                }
                                            }
                                            else
                                            {
                                                User.TeleDelay--;
                                                showHopperEffect = true;
                                            }
                                        }

                                        else if (User.Coordinate == SquareInFront)
                                        {
                                            User.AllowOverride = true;
                                            keepDoorOpen = true;


                                            if (User.IsWalking && (User.GoalX != GetX || User.GoalY != GetY))
                                            {
                                                User.ClearMovement(true);
                                            }

                                            User.CanWalk = false;
                                            User.AllowOverride = true;


                                            User.MoveTo(Coordinate.X, Coordinate.Y, true);
                                        }

                                        else
                                        {
                                            InteractingUser = 0;
                                        }
                                    }
                                    else
                                    {

                                        InteractingUser = 0;
                                    }
                                }

                                if (InteractingUser2 > 0)
                                {
                                    User2 = GetRoom().GetRoomUserManager().GetRoomUserByHabbo(InteractingUser2);


                                    if (User2 != null)
                                    {

                                        keepDoorOpen = true;
                                        User2.UnlockWalking();
                                        User2.MoveTo(SquareInFront);
                                    }

                                    InteractingUser2 = 0;
                                }


                                if (keepDoorOpen)
                                {
                                    if (ExtraData != "1")
                                    {
                                        ExtraData = "1";
                                        UpdateState(false, true);
                                    }
                                }
                                else if (showHopperEffect)
                                {
                                    if (ExtraData != "2")
                                    {
                                        ExtraData = "2";
                                        UpdateState(false, true);
                                    }
                                }
                                else
                                {
                                    if (ExtraData != "0")
                                    {
                                        if (Pause == 0)
                                        {
                                            ExtraData = "0";
                                            UpdateState(false, true);
                                            Pause = 2;
                                        }
                                        else
                                        {
                                            Pause--;
                                        }
                                    }
                                }


                                RequestUpdate(1, false);
                                break;
                            }
                        #endregion

                        #region Teleports
                        case InteractionType.TELEPORT:
                            {
                                User = null;
                                User2 = null;

                                bool keepDoorOpen = false;
                                bool showTeleEffect = false;


                                if (InteractingUser > 0)
                                {
                                    User = GetRoom().GetRoomUserManager().GetRoomUserByHabbo(InteractingUser);


                                    if (User != null)
                                    {

                                        if (User.Coordinate == Coordinate)
                                        {

                                            User.AllowOverride = false;

                                            if (ItemTeleporterFinder.IsTeleLinked(Id, GetRoom()))
                                            {
                                                showTeleEffect = true;

                                                if (true)
                                                {

                                                    int TeleId = ItemTeleporterFinder.GetLinkedTele(Id, GetRoom());
                                                    int RoomId = ItemTeleporterFinder.GetTeleRoomId(TeleId, GetRoom());


                                                    if (RoomId == this.RoomId)
                                                    {
                                                        Item Item = GetRoom().GetRoomItemHandler().GetItem(TeleId);

                                                        if (Item == null)
                                                        {
                                                            User.UnlockWalking();
                                                        }
                                                        else
                                                        {

                                                            User.SetPos(Item.GetX, Item.GetY, Item.GetZ);
                                                            User.SetRot(Item.Rotation, false);


                                                            Item.ExtraData = "2";
                                                            Item.UpdateState(false, true);


                                                            Item.InteractingUser2 = InteractingUser;
                                                            GetRoom().GetGameMap().RemoveUserFromMap(User, new Point(GetX, GetY));

                                                            InteractingUser = 0;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (User.TeleDelay == 0)
                                                        {

                                                            if (!User.IsBot && User != null && User.GetClient() != null &&
                                                                User.GetClient().GetHabbo() != null)
                                                            {
                                                                User.GetClient().GetHabbo().IsTeleporting = true;
                                                                User.GetClient().GetHabbo().TeleportingRoomID = RoomId;
                                                                User.GetClient().GetHabbo().TeleporterId = TeleId;
                                                                User.GetClient().GetHabbo().PrepareRoom(RoomId, "");
                                                                //User.GetClient().SendMessage(new RoomForwardComposer(RoomId));
                                                                InteractingUser = 0;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            User.TeleDelay--;
                                                            showTeleEffect = true;
                                                        }
                                                        //MoonEnvironment.GetGame().GetRoomManager().AddTeleAction(new TeleUserData(User.GetClient().GetMessageHandler(), User.GetClient().GetHabbo(), RoomId, TeleId));
                                                    }
                                                    GetRoom().GetGameMap().GenerateMaps();

                                                }
                                                else
                                                {

                                                }
                                            }
                                            else
                                            {

                                                User.UnlockWalking();
                                                InteractingUser = 0;
                                            }
                                        }

                                        else if (User.Coordinate == SquareInFront)
                                        {
                                            User.AllowOverride = true;

                                            keepDoorOpen = true;


                                            if (User.IsWalking && (User.GoalX != GetX || User.GoalY != GetY))
                                            {
                                                User.ClearMovement(true);
                                            }

                                            User.CanWalk = false;
                                            User.AllowOverride = true;

                                            User.MoveTo(Coordinate.X, Coordinate.Y, true);

                                        }

                                        else
                                        {
                                            InteractingUser = 0;
                                        }
                                    }
                                    else
                                    {

                                        InteractingUser = 0;
                                    }
                                }


                                if (InteractingUser2 > 0)
                                {
                                    User2 = GetRoom().GetRoomUserManager().GetRoomUserByHabbo(InteractingUser2);


                                    if (User2 != null)
                                    {

                                        keepDoorOpen = true;
                                        User2.UnlockWalking();
                                        User2.MoveTo(SquareInFront);
                                    }


                                    InteractingUser2 = 0;
                                }


                                if (showTeleEffect)
                                {
                                    if (ExtraData != "2")
                                    {
                                        ExtraData = "2";
                                        UpdateState(false, true);
                                    }
                                }
                                else if (keepDoorOpen)
                                {
                                    if (ExtraData != "1")
                                    {
                                        ExtraData = "1";
                                        UpdateState(false, true);
                                    }
                                }
                                else
                                {
                                    if (ExtraData != "0")
                                    {
                                        ExtraData = "0";
                                        UpdateState(false, true);
                                    }
                                }


                                RequestUpdate(1, false);
                                break;
                            }
                        #endregion

                        #region Bottle
                        case InteractionType.BOTTLE:
                            ExtraData = RandomNumber.GenerateNewRandom(0, 7).ToString();
                            UpdateState();
                            break;
                        #endregion

                        #region Dice
                        case InteractionType.DICE:
                            {
                                string[] numbers = new string[] { "1", "2", "3", "4", "5", "6" };
                                if (ExtraData == "-1")
                                    ExtraData = RandomizeStrings(numbers)[0];
                                UpdateState();
                                //left off here
                                MoonEnvironment.GetHabboById(InteractingUser).casinoEvent(ExtraData);

                            }
                            break;
                        #endregion


                        #region Dice 2
                        case InteractionType.DICE2:
                            {

                                if (ExtraData == "-1")
                                {
                                    var num = RandomNumber.GenerateRandom(1, 6);
                                    ExtraData = num.ToString();
                                }

                                UpdateState();
                            }
                            break;
                        #endregion

                        #region Habbo Wheel
                        case InteractionType.HABBO_WHEEL:
                            ExtraData = RandomNumber.GenerateRandom(1, 10).ToString();
                            UpdateState();
                            break;
                        #endregion

                        #region Love Shuffler
                        case InteractionType.LOVE_SHUFFLER:

                            if (ExtraData == "0")
                            {
                                ExtraData = RandomNumber.GenerateNewRandom(1, 4).ToString();
                                RequestUpdate(20, false);
                            }
                            else if (ExtraData != "-1")
                            {
                                ExtraData = "-1";
                            }

                            UpdateState(false, true);
                            break;
                        #endregion

                        #region Alert
                        case InteractionType.ALERT:
                            if (ExtraData == "1")
                            {
                                ExtraData = "0";
                                UpdateState(false, true);
                            }
                            break;
                        #endregion

                        #region Vending Machine
                        case InteractionType.VENDING_MACHINE:

                            if (ExtraData == "1")
                            {
                                User = GetRoom().GetRoomUserManager().GetRoomUserByHabbo(InteractingUser);
                                if (User == null)
                                    break;
                                User.UnlockWalking();
                                if (GetBaseItem().VendingIds.Count > 0)
                                {
                                    int randomDrink = GetBaseItem().VendingIds[RandomNumber.GenerateRandom(0, (GetBaseItem().VendingIds.Count - 1))];
                                    User.CarryItem(randomDrink);
                                }


                                InteractingUser = 0;
                                ExtraData = "0";

                                UpdateState(false, true);
                            }
                            break;
                        #endregion

                        #region Scoreboard
                        case InteractionType.SCOREBOARD:
                            {
                                if (string.IsNullOrEmpty(ExtraData))
                                    break;


                                int seconds = 0;

                                try
                                {
                                    seconds = int.Parse(ExtraData);
                                }
                                catch
                                {
                                }

                                if (seconds > 0)
                                {
                                    if (interactionCountHelper == 1)
                                    {
                                        seconds--;
                                        interactionCountHelper = 0;

                                        ExtraData = seconds.ToString();
                                        UpdateState();
                                    }
                                    else
                                        interactionCountHelper++;

                                    UpdateCounter = 1;
                                }
                                else
                                    UpdateCounter = 0;

                                break;
                            }
                        #endregion

                        #region Banzai Counter
                        case InteractionType.banzaicounter:
                            {
                                if (string.IsNullOrEmpty(ExtraData))
                                    break;

                                int seconds = 0;

                                try
                                {
                                    seconds = int.Parse(ExtraData);
                                }
                                catch
                                {
                                }

                                if (seconds > 0)
                                {
                                    if (interactionCountHelper == 1)
                                    {
                                        seconds--;
                                        interactionCountHelper = 0;

                                        if (GetRoom().GetBanzai().isBanzaiActive)
                                        {
                                            ExtraData = seconds.ToString();
                                            UpdateState();
                                        }
                                        else
                                            break;
                                    }
                                    else
                                        interactionCountHelper++;

                                    UpdateCounter = 1;
                                }
                                else
                                {
                                    UpdateCounter = 0;
                                    GetRoom().GetBanzai().BanzaiEnd();
                                }

                                break;
                            }
                        #endregion

                        #region Banzai Tele
                        case InteractionType.banzaitele:
                            {
                                ExtraData = string.Empty;
                                UpdateState();
                                break;
                            }
                        #endregion

                        #region Banzai Floor
                        case InteractionType.banzaifloor:
                            {
                                if (value == 3)
                                {
                                    if (interactionCountHelper == 1)
                                    {
                                        interactionCountHelper = 0;

                                        switch (team)
                                        {
                                            case TEAM.BLUE:
                                                {
                                                    ExtraData = "11";
                                                    break;
                                                }

                                            case TEAM.GREEN:
                                                {
                                                    ExtraData = "8";
                                                    break;
                                                }

                                            case TEAM.RED:
                                                {
                                                    ExtraData = "5";
                                                    break;
                                                }

                                            case TEAM.YELLOW:
                                                {
                                                    ExtraData = "14";
                                                    break;
                                                }
                                        }
                                    }
                                    else
                                    {
                                        ExtraData = "";
                                        interactionCountHelper++;
                                    }

                                    UpdateState();

                                    interactionCount++;

                                    if (interactionCount < 16)
                                    {
                                        UpdateCounter = 1;
                                    }
                                    else
                                        UpdateCounter = 0;
                                }
                                break;
                            }
                        #endregion

                        #region Banzai Puck
                        case InteractionType.banzaipuck:
                            {
                                if (interactionCount > 4)
                                {
                                    interactionCount++;
                                    UpdateCounter = 1;
                                }
                                else
                                {
                                    interactionCount = 0;
                                    UpdateCounter = 0;
                                }

                                break;
                            }
                        #endregion

                        #region Freeze Tile
                        case InteractionType.FREEZE_TILE:
                            {
                                if (InteractingUser > 0)
                                {
                                    this.ExtraData = "11000";
                                    this.UpdateState(false, true);
                                    this.GetRoom().GetFreeze().onFreezeTiles(this, this.freezePowerUp);
                                    this.InteractingUser = 0;
                                    this.interactionCountHelper = 0;
                                }
                                break;
                            }
                        #endregion

                        #region Football Counter
                        case InteractionType.COUNTER:
                            {
                                if (string.IsNullOrEmpty(ExtraData))
                                    break;

                                int seconds = 0;

                                try
                                {
                                    seconds = int.Parse(ExtraData);
                                }
                                catch
                                {
                                }

                                if (seconds > 0)
                                {
                                    if (interactionCountHelper == 1)
                                    {
                                        seconds--;
                                        interactionCountHelper = 0;
                                        if (GetRoom().GetSoccer().GameIsStarted)
                                        {
                                            ExtraData = seconds.ToString();
                                            UpdateState();
                                        }
                                        else
                                            break;
                                    }
                                    else
                                        interactionCountHelper++;

                                    UpdateCounter = 1;
                                }
                                else
                                {
                                    UpdateNeeded = false;
                                    GetRoom().GetSoccer().StopGame();
                                }

                                break;
                            }
                        #endregion

                        #region Freeze Timer
                        case InteractionType.freezetimer:
                            {
                                if (string.IsNullOrEmpty(ExtraData))
                                    break;

                                int seconds = 0;

                                try
                                {
                                    seconds = int.Parse(ExtraData);
                                }
                                catch
                                {
                                }

                                if (seconds > 0)
                                {
                                    if (interactionCountHelper == 1)
                                    {
                                        seconds--;
                                        interactionCountHelper = 0;
                                        if (GetRoom().GetFreeze().GameIsStarted)
                                        {
                                            ExtraData = seconds.ToString();
                                            UpdateState();
                                        }
                                        else
                                            break;
                                    }
                                    else
                                        interactionCountHelper++;

                                    UpdateCounter = 1;
                                }
                                else
                                {
                                    UpdateNeeded = false;
                                    GetRoom().GetFreeze().StopGame();
                                }

                                break;
                            }
                        #endregion

                        #region Pressure Pad
                        case InteractionType.PRESSURE_PAD:
                            {
                                ExtraData = "1";
                                UpdateState();
                                break;
                            }
                        #endregion

                        #region Wired
                        case InteractionType.WIRED_EFFECT:
                        case InteractionType.WIRED_TRIGGER:
                        case InteractionType.WIRED_CONDITION:
                            {
                                if (ExtraData == "1")
                                {
                                    ExtraData = "0";
                                    UpdateState(false, true);
                                }
                            }
                            break;
                        #endregion

                        #region Cannon
                        case InteractionType.CANNON:
                            {
                                if (ExtraData != "1")
                                    break;

                                #region Target Calculation
                                Point TargetStart = Coordinate;
                                List<Point> TargetSquares = new List<Point>();
                                switch (Rotation)
                                {
                                    case 0:
                                        {
                                            TargetStart = new Point(GetX - 1, GetY);

                                            if (!TargetSquares.Contains(TargetStart))
                                                TargetSquares.Add(TargetStart);

                                            for (int I = 1; I <= 3; I++)
                                            {
                                                Point TargetSquare = new Point(TargetStart.X - I, TargetStart.Y);

                                                if (!TargetSquares.Contains(TargetSquare))
                                                    TargetSquares.Add(TargetSquare);
                                            }

                                            break;
                                        }

                                    case 2:
                                        {
                                            TargetStart = new Point(GetX, GetY - 1);

                                            if (!TargetSquares.Contains(TargetStart))
                                                TargetSquares.Add(TargetStart);

                                            for (int I = 1; I <= 3; I++)
                                            {
                                                Point TargetSquare = new Point(TargetStart.X, TargetStart.Y - I);

                                                if (!TargetSquares.Contains(TargetSquare))
                                                    TargetSquares.Add(TargetSquare);
                                            }

                                            break;
                                        }

                                    case 4:
                                        {
                                            TargetStart = new Point(GetX + 2, GetY);

                                            if (!TargetSquares.Contains(TargetStart))
                                                TargetSquares.Add(TargetStart);

                                            for (int I = 1; I <= 3; I++)
                                            {
                                                Point TargetSquare = new Point(TargetStart.X + I, TargetStart.Y);

                                                if (!TargetSquares.Contains(TargetSquare))
                                                    TargetSquares.Add(TargetSquare);
                                            }

                                            break;
                                        }

                                    case 6:
                                        {
                                            TargetStart = new Point(GetX, GetY + 2);


                                            if (!TargetSquares.Contains(TargetStart))
                                                TargetSquares.Add(TargetStart);

                                            for (int I = 1; I <= 3; I++)
                                            {
                                                Point TargetSquare = new Point(TargetStart.X, TargetStart.Y + I);

                                                if (!TargetSquares.Contains(TargetSquare))
                                                    TargetSquares.Add(TargetSquare);
                                            }

                                            break;
                                        }
                                }
                                #endregion

                                if (TargetSquares.Count > 0)
                                {
                                    foreach (Point Square in TargetSquares.ToList())
                                    {
                                        List<RoomUser> affectedUsers = _room.GetGameMap().GetRoomUsers(Square).ToList();

                                        if (affectedUsers == null || affectedUsers.Count == 0)
                                            continue;

                                        foreach (RoomUser Target in affectedUsers)
                                        {
                                            if (Target == null || Target.IsBot || Target.IsPet)
                                                continue;

                                            if (Target.GetClient() == null || Target.GetClient().GetHabbo() == null)
                                                continue;

                                            if (_room.CheckRights(Target.GetClient(), true))
                                                continue;

                                            Target.ApplyEffect(4);
                                            Target.GetClient().SendMessage(new RoomNotificationComposer("Expulsado de la Sala", "Usted fue golpeado por la bola del cañon directo hacia la salida!", "room_kick_cannonball"));
                                            Target.ApplyEffect(0);
                                            _room.GetRoomUserManager().RemoveUserFromRoom(Target.GetClient(), true);
                                        }
                                    }
                                }

                                ExtraData = "2";
                                UpdateState(false, true);
                            }
                            break;
                            #endregion
                    }
                }
            }
            catch (Exception e) { Logging.LogException(e.ToString()); }
        }

        public static string[] RandomizeStrings(string[] arr)
        {
            List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();

            foreach (string s in arr)
            {
                list.Add(new KeyValuePair<int, string>(_random.Next(), s));
            }

            var sorted = from item in list
                         orderby item.Key
                         select item;

            string[] result = new string[arr.Length];

            int index = 0;
            foreach (KeyValuePair<int, string> pair in sorted)
            {
                result[index] = pair.Value;
                index++;
            }

            return result;
        }

        public void RequestUpdate(int Cycles, bool setUpdate)
        {
            UpdateCounter = Cycles;
            if (setUpdate)
                UpdateNeeded = true;
        }

        public void UpdateState()
        {
            UpdateState(true, true);
        }

        public void UpdateState(bool inDb, bool inRoom)
        {
            if (GetRoom() == null)
                return;

            if (inDb)
                GetRoom().GetRoomItemHandler().UpdateItem(this);

            if (inRoom)
            {
                if (IsFloorItem)
                    GetRoom().SendMessage(new ObjectUpdateComposer(this, GetRoom().OwnerId));
                else
                    GetRoom().SendMessage(new ItemUpdateComposer(this, GetRoom().OwnerId));
            }
        }

        public void ResetBaseItem()
        {
            this._data = null;
            this._data = this.GetBaseItem();
        }

        public ItemData GetBaseItem()
        {
            if (this._data == null)
            {
                ItemData I = null;
                if (MoonEnvironment.GetGame().GetItemManager().GetItem(this.BaseItem, out I))
                    this._data = I;
            }

            return this._data;
        }

        public Room GetRoom()
        {
            if (this._room != null)
                return this._room;

            Room Room;
            if (MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(RoomId, out Room))
                return Room;

            return null;
        }



        public void UserFurniCollision(RoomUser user)
        {
            if (user == null || user.GetClient() == null || user.GetClient().GetHabbo() == null)
                return;

            GetRoom().GetWired().TriggerEvent(Wired.WiredBoxType.TriggerUserFurniCollision, user.GetClient().GetHabbo(), this);
        }

        public void UserWalksOnFurni(RoomUser user)
        {
            if (user == null || user.GetClient() == null || user.GetClient().GetHabbo() == null)
                return;

            if (GetBaseItem().InteractionType == InteractionType.TENT || GetBaseItem().InteractionType == InteractionType.TENT_SMALL)
            {
                GetRoom().AddUserToTent(Id, user, this);
            }

            //if (GetBaseItem().InteractionType == InteractionType.PRESSURE_TILE)
            //{
            //    this.ExtraData = "1";
            //    this.UpdateState();
            //}

            if (GetBaseItem().InteractionType == InteractionType.PRESSURE_PAD)
            {
                this.ExtraData = "1";
                this.UpdateState();
            }

            GetRoom().GetWired().TriggerEvent(Wired.WiredBoxType.TriggerWalkOnFurni, user.GetClient().GetHabbo(), this);
            user.LastItem = this;
        }

        public void UserWalksOffFurni(RoomUser user)
        {
            if (user == null || user.GetClient() == null || user.GetClient().GetHabbo() == null)
                return;

            if (GetBaseItem().InteractionType == InteractionType.TENT || GetBaseItem().InteractionType == InteractionType.TENT_SMALL)
                GetRoom().RemoveUserFromTent(Id, user, this);

            //if (GetBaseItem().InteractionType == InteractionType.PRESSURE_TILE)
            //{
            //    this.ExtraData = "0";
            //    this.UpdateState();
            //}

            if (GetBaseItem().InteractionType == InteractionType.PRESSURE_PAD)
            {
                this.ExtraData = "0";
                this.UpdateState();
            }

            GetRoom().GetWired().TriggerEvent(Wired.WiredBoxType.TriggerWalkOffFurni, user.GetClient().GetHabbo(), this);
        }
    }
}
