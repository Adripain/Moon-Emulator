using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Rooms;
using Moon.Utilities;
using System;
using System.Threading;
using Moon.Communication.Interfaces;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;

namespace Moon.HabboHotel.Items.Interactor
{
    class InteractorSlotmachine : IFurniInteractor
    {
        string Rand1;
        string Rand2;
        string Rand3;

        int paga;

        public void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            // Revisar la cantidad de diamantes que tiene.
            if (Session.GetHabbo().Diamonds <= 0)
            {
                Session.SendWhisper("Para poder apostar debes tener diamantes, ahora mismo tienes 0.", 34);
                return;
            }

            if (Session.GetHabbo()._bet > Session.GetHabbo().Diamonds)
            {
                Session.SendWhisper("Estás apostando más diamantes de los que tienes.", 34);
                return;
            }

            if (Session.GetHabbo()._bet <= 0)
            {
                Session.SendWhisper("No puedes apostar 0 diamantes. Utiliza el siguiente comando para apostar:", 34);
                Session.SendWhisper("\":apostar %cantidad%\"", 34);

                return;
            }

            if (Session == null || Session.GetHabbo() == null || Item == null)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser Actor = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (Actor == null)
                return;

            if (Gamemap.TileDistance(Actor.X, Actor.Y, Item.GetX, Item.GetY) < 2)
            {
                int Bet = Session.GetHabbo()._bet;

                Session.GetHabbo().Diamonds -= Bet;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, -Bet, 5));

                int Random1 = RandomNumber.GenerateRandom(1, 3);
                switch (Random1)
                {
                    case 1:
                        Rand1 = "¥";
                        break;
                    case 2:
                        Rand1 = "|";
                        break;
                    case 3:
                        Rand1 = "ª";
                        break;
                }

                int Random2 = RandomNumber.GenerateRandom(1, 3);
                switch (Random2)
                {
                    case 1:
                        Rand2 = "¥";
                        break;
                    case 2:
                        Rand2 = "|";
                        break;
                    case 3:
                        Rand2 = "ª";
                        break;
                }

                int Random3 = RandomNumber.GenerateRandom(1, 3);
                switch (Random3)
                {
                    case 1:
                        Rand3 = "¥";
                        break;
                    case 2:
                        Rand3 = "|";
                        break;
                    case 3:
                        Rand3 = "ª";
                        break;
                }
                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (User == null || User.GetClient() == null)
                    return;

                Room.SendMessage(new ChatComposer(User.VirtualId, "¡" + Session.GetHabbo().Username + " Jala la palanca y saca [ " + Rand1 + " - " + Rand2 + " - " + Rand3 + " ]!", 0, 34));
                Item.ExtraData = "1";
                Item.UpdateState(true, true);

                new Thread(() =>
                {
                    Thread.Sleep(1000);
                    Item.ExtraData = "0";
                    Item.UpdateState(true, true);
                }).Start();

                int Paga = RandomNumber.GenerateRandom(1, 5);
                switch (Paga)
                {
                    case 1:
                        paga = 1;
                        break;
                    case 2:
                        paga = 2;
                        break;
                    case 3:
                        paga = 3;
                        break;
                    case 4:
                        paga = 4;
                        break;
                    case 5:
                        paga = 5;
                        break;
                    case 6:
                        paga = 6;
                        break;
                }

                if (Random1 == Random2 && Random1 == Random3 && Random3 == Random2)
                {
                    //  ¥ - Estrella - » Trebol - ª Calavera
                    switch (Random1)
                    {
                        case 1:
                            Session.GetHabbo().Diamonds += Bet * 7;
                            Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, -Bet, 5));
                            Room.SendMessage(new ChatComposer(User.VirtualId, "¡" + Session.GetHabbo().Username + " Ha ganado " + Bet * 7 + " diamantes con una triple estrella!", 0, 34));
                            MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Actor.GetClient(), "ACH_StarBet", 1);
                            break;
                        case 2:
                            Session.GetHabbo().Diamonds += Bet * paga;
                            Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, -Bet, 5));
                            Room.SendMessage(new ChatComposer(User.VirtualId, "¡" + Session.GetHabbo().Username + " Ha ganado " + Bet * paga + " diamantes con un triple corazón!", 0, 34));
                            MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Actor.GetClient(), "ACH_HeartBet", 1);
                            break;
                        case 3:
                            Session.GetHabbo().Diamonds += Bet * paga;
                            Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, -Bet, 5));
                            Room.SendMessage(new ChatComposer(User.VirtualId, "¡" + Session.GetHabbo().Username + " Ha ganado " + Bet * paga + " diamantes con una triple calavera!", 0, 34));
                            MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Actor.GetClient(), "ACH_SkullBet", 1);
                            break;
                    }
                }

                MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Actor.GetClient(), "ACH_GeneralBet", 1);
                return;
            }
        }

        public void OnWiredTrigger(Item Item)
        {

        }
    }
}
