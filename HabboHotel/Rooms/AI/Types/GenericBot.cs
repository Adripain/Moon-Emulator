using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

using Moon.Core;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Items;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Rooms.AI.Speech;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.HabboHotel.Rooms.AI.Responses;
using Moon.HabboHotel.Items.Wired;

namespace Moon.HabboHotel.Rooms.AI.Types
{
    public class GenericBot : BotAI
    {
        private int VirtualId;
        private int ActionTimer = 0;
        private int SpeechTimer = 0;

        public GenericBot(int VirtualId)
        {
            this.VirtualId = VirtualId;
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

            if (SpeechTimer <= 0)
            {
                if (GetBotData().RandomSpeech.Count > 0)
                {
                    if (GetBotData().AutomaticChat == false)
                        return;

                    RandomSpeech Speech = GetBotData().GetRandomSpeech();

                    string word;
                    string String = MoonEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Speech.Message, out word) ? "Spam" : Speech.Message;
                    if (String.Contains("<") || String.Contains(">"))
                        String = "I really shouldn't be using HTML within bot speeches.";
                    GetRoomUser().Chat(String, false, GetBotData().ChatBubble);
                }
                SpeechTimer = GetBotData().SpeakingInterval;
            }
            else
                SpeechTimer--;
           
            if (ActionTimer <= 0)
            {
                Point nextCoord;
                switch (GetBotData().WalkingMode.ToLower())
                {
                    default:
                    case "stand":
                        break;

                    case "freeroam":
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
                            RoomUser Target = GetRoom().GetRoomUserManager().GetRoomUserByHabbo(GetBotData().ForcedUserTargetMovement);
                            if (Target == null)
                            {
                                GetBotData().ForcedUserTargetMovement = 0;
                                GetRoomUser().ClearMovement(true);
                            }
                            else
                            {
                                var Sq = new Point(Target.X, Target.Y);

                                if (Target.RotBody == 0)
                                {
                                    Sq.Y--;
                                }
                                else if (Target.RotBody == 2)
                                {
                                    Sq.X++;
                                }
                                else if (Target.RotBody == 4)
                                {
                                    Sq.Y++;
                                }
                                else if (Target.RotBody == 6)
                                {
                                    Sq.X--;
                                }

                                //if (Gamemap.TileDistance(Sq.X, Sq.Y, Target.X, Target.Y) <= 1)
                                //{
                                //    Room Room = GetRoom();
                                //    Room.GetWired().TriggerEvent(WiredBoxType.TriggerBotReachedAvatar, true);
                                //}

                                GetRoomUser().MoveTo(Sq);
                            }
                        }
                        else if(GetBotData().TargetUser == 0)
                        {
                            nextCoord = GetRoom().GetGameMap().getRandomWalkableSquare();
                            GetRoomUser().MoveTo(nextCoord.X, nextCoord.Y);
                        }
                        break;

                    case "specified_range":

                        break;
                }

                ActionTimer = new Random(DateTime.Now.Millisecond + this.VirtualId ^ 2).Next(5, 15);
            }
            else
                ActionTimer--;
        }
    }
}