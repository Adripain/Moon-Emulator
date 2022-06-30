using System;
using System.Drawing;

using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Rooms.AI.Speech;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.HabboHotel.Rooms.AI.Responses;
using Moon.Utilities;
using Moon.HabboHotel.Rooms.AI;
using Moon.HabboHotel.Rooms;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.HabboHotel.Items;
using Moon.Communication.Packets.Outgoing.Nux;
using Moon.Communication.Packets.Outgoing;
using Moon.Communication.Packets.Outgoing.Rooms.Nux;

namespace Moon.HabboHotel.Rewards.Rooms.AI.Types
{
    class WelcomeBot : BotAI
    {
        private int VirtualId;
        private int ActionTimer = 0;

        public WelcomeBot(int VirtualId)
        {
            this.VirtualId = VirtualId;
            ActionTimer = 0;

        }


        public override void OnSelfEnterRoom()
        {

        }
        public override void OnSelfLeaveRoom(bool Kicked)
        {
        }

        public override void OnUserEnterRoom(RoomUser User)
        {
        }

        public override void OnUserLeaveRoom(GameClient Client)
        {
            //if ()
        }

        public override void OnUserSay(RoomUser User, string Message)
        {

        }

        public override void OnUserShout(RoomUser User, string Message)
        {

        }

        public override void OnTimerTick()
        {
            if (GetBotData() == null)
                return;

            GameClient Target = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(GetRoom().OwnerName);
            if (Target == null || Target.GetHabbo() == null || Target.GetHabbo().CurrentRoom != GetRoom())
            {
                GetRoom().GetGameMap().RemoveUserFromMap(GetRoomUser(), new Point(GetRoomUser().X, GetRoomUser().Y));
                GetRoom().GetRoomUserManager().RemoveBot(GetRoomUser().VirtualId, false);
                return;
            }
            var habbo = Target.GetHabbo();

            if (ActionTimer <= 0)
            {/*
                switch (Target.GetHabbo().GetStats().WelcomeLevel)
                {
                    case 0:
                    default:
                        Point nextCoord;
                        RoomUser Target2 = GetRoom().GetRoomUserManager().GetRoomUserByHabbo(GetBotData().ForcedUserTargetMovement);
                        if (GetBotData().ForcedMovement)
                        {
                            if (GetRoomUser().Coordinate == GetBotData().TargetCoordinate)
                            {
                                GetBotData().ForcedMovement = false;
                                GetBotData().TargetCoordinate = new Point();

                                GetRoomUser().MoveTo(GetBotData().TargetCoordinate.X, GetBotData().TargetCoordinate.Y);
                            }
                        }
                        else if (GetBotData().ForcedUserTargetMovement > 0)
                        {

                            if (Target2 == null)
                            {
                                GetBotData().ForcedUserTargetMovement = 0;
                                GetRoomUser().ClearMovement(true);
                            }
                            else
                            {
                                var Sq = new Point(Target2.X, Target2.Y);

                                if (Target2.RotBody == 0)
                                {
                                    Sq.Y--;
                                }
                                else if (Target2.RotBody == 2)
                                {
                                    Sq.X++;
                                }
                                else if (Target2.RotBody == 4)
                                {
                                    Sq.Y++;
                                }
                                else if (Target2.RotBody == 6)
                                {
                                    Sq.X--;
                                }


                                GetRoomUser().MoveTo(Sq);
                            }
                        }
                        else if (GetBotData().TargetUser == 0)
                        {
                            nextCoord = GetRoom().GetGameMap().getRandomWalkableSquare();
                            GetRoomUser().MoveTo(nextCoord.X, nextCoord.Y);
                        }
                        Target.GetHabbo().GetStats().WelcomeLevel++;
                        break;
                    case 1:
                        GetRoomUser().Chat("Bem-vindo ao " + MoonEnvironment.HotelName + ", " + GetRoom().OwnerName + "!\nEu sou Frank o gerente do Hotel.\nPronto para encontrar muitas\nsurpresas ?", false, 33);
                        Target.GetHabbo().GetStats().WelcomeLevel++;
                        break;

                    case 2:
                        GetRoomUser().Chat("Agora preste atenção, te\nexplicaremos como se joga no\n" + MoonEnvironment.HotelName + ".", false, 33);
                        Target.GetHabbo().GetStats().WelcomeLevel++;
                        break;

                    case 3:
                        if (Target.GetHabbo()._NUX)
                        {
                            var nuxStatus = new ServerPacket(ServerPacketHeader.NuxUserStatus);
                            nuxStatus.WriteInteger(2);
                            Target.SendMessage(nuxStatus);
                            Target.SendMessage(new NuxAlertComposer("nux/lobbyoffer/hide"));
                            Target.SendMessage(new NuxAlertComposer("helpBubble/add/HC_JOIN_BUTTON/Insceva-se no Club VIP do " + MoonEnvironment.HotelName + ", para obtener muchas ventajas."));
                        }
                        Target.GetHabbo().GetStats().WelcomeLevel++;
                        break;

                    case 4:
                        if (habbo.PassedNuxNavigator && habbo.PassedNuxCatalog && habbo.PassedNuxItems && habbo.PassedNuxMMenu && habbo.PassedNuxChat && habbo.PassedNuxCredits && habbo.PassedNuxDuckets)
                        {
                        GetRoomUser().Chat("Aproveite a sua estadia. No\n" + MoonEnvironment.HotelName + " todos nós somos únicos!", false, 33);
                        Target.GetHabbo().GetStats().WelcomeLevel++;
                        }
                        break;

                    case 5:
                        GetRoomUser().Chat("...eu ainda não acabei,\n¡Escolha o seu presente de boas-vindas!", false, 33);
                        Target.GetHabbo().GetStats().WelcomeLevel++;
                        Target.SendMessage(new NuxItemListComposer());
                        break;

                    case 6:
                        Target.GetHabbo().GetStats().WelcomeLevel++;
                        break;

                    case 7:
                        GetRoomUser().Chat("Bem " + GetRoom().OwnerName + ", está na hora de eu partir.\nNão se esqueça de conectar todos os\ndías no " + MoonEnvironment.HotelName + "!", false, 33);
                        Target.GetHabbo().GetStats().WelcomeLevel++;
                        break;

                    case 8:
                        if (habbo.PassedNuxNavigator && habbo.PassedNuxCatalog && habbo.PassedNuxItems && habbo.PassedNuxMMenu && habbo.PassedNuxChat && habbo.PassedNuxCredits && habbo.PassedNuxDuckets)
                        {
                            Target.SendMessage(new NuxAlertComposer("nux/lobbyoffer/show"));
                            var nuxStatus = new ServerPacket(ServerPacketHeader.NuxUserStatus);
                            nuxStatus.WriteInteger(0);
                            Target.SendMessage(nuxStatus);
                        }*/
                        GetRoom().GetGameMap().RemoveUserFromMap(GetRoomUser(), new Point(GetRoomUser().X, GetRoomUser().Y));
                        GetRoom().GetRoomUserManager().RemoveBot(GetRoomUser().VirtualId, false);
                        Target.GetHabbo().GetStats().WelcomeLevel = 9;
                        //break;
                        
                }
                ActionTimer = new Random(DateTime.Now.Millisecond + this.VirtualId ^ 2).Next(5, 15);
            }
           /* else
                ActionTimer--;*/
        }
    }

