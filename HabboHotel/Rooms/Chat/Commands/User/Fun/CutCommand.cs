using System;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using System.Threading;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class
        CutCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_vip"; }
        }
        public string Parameters
        {
            get { return "[USUARIO]"; }
        }
        public string Description
        {
            get { return "Atirar em um usuário"; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
                if (Params.Length == 1)
                {
                    Session.SendWhisper("Você deve digitar um nome de usuário!", 34);
                    return;
                }

                GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
                if (TargetClient == null)
                {
                    Session.SendWhisper("Esse usuário não pode ser encontrado, talvez ele não esteja online ou não esteja no quarto.", 34);
                    return;
                }

                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
                if (User == null)
                {
                    Session.SendWhisper("Esse usuário não pode ser encontrado, talvez ele não esteja online ou não esteja no quarto.", 34);
                    return;
                }
                RoomUser Self = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (User == Self)
                {
                    Session.SendWhisper("Você não pode atirar!");
                    return;
                }
            if (TargetClient.GetHabbo().Username == "Forbi" || TargetClient.GetHabbo().Username == "Forb")
            {
                Session.SendWhisper("¡El es tu patón!", 34);
                return;
            }
            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (ThisUser == null)
                    return;

                if (Math.Abs(User.X - ThisUser.X) < 2 && Math.Abs(User.Y - ThisUser.Y) < 2)
                {
                    Room.SendMessage(new ShoutComposer(ThisUser.VirtualId, "*Atirar na cabeça de" + TargetClient.GetHabbo().Username + "*", 0, ThisUser.LastBubble));
                    Room.SendMessage(new ChatComposer(User.VirtualId, "*Morrendo*", 0, User.LastBubble));

                    User.GetClient().SendWhisper("Ele morre em 3 segundos!");
                    ThisUser.ApplyEffect(539);
                    User.ApplyEffect(93);
                    TargetClient.SendMessage(new FloodControlComposer(3));
                    if (User != null)
                        User.Frozen = true;
                    System.Threading.Thread thrd = new System.Threading.Thread(delegate ()
                    {
                        Thread.Sleep(4000);
                        if (User != null)
                            User.Frozen = false;
                        User.ApplyEffect(23);
                        Thread.Sleep(2000);
                        ThisUser.ApplyEffect(539);
                        User.ApplyEffect(23);
                        if (User == null)
                            return;

                        if (User.Statusses.ContainsKey("lie") || User.isLying || User.RidingHorse || User.IsWalking)
                            return;

                        if (!User.Statusses.ContainsKey("lay"))
                        {
                            if ((User.RotBody % 2) == 0)
                            {
                                if (User == null)
                                    return;

                                try
                                {
                                    User.Statusses.Add("lay", "1.0");
                                    User.Z -= 0.35;
                                    User.isLying = true;
                                    User.UpdateNeeded = true;
                                }
                                catch { }
                            }
                            else
                            {
                                User.RotBody--;
                                User.Statusses.Add("lay", "1.0");
                                User.Z -= 0.35;
                                User.isLying = true;
                                User.UpdateNeeded = true;
                            }
                        }
                        else if (User.isLying == true)
                        {
                            User.Z += 0.35;
                            User.Statusses.Remove("lay");
                            User.Statusses.Remove("1.0");
                            User.isLying = false;
                            User.UpdateNeeded = true;
                        }
                        Room.SendMessage(new ChatComposer(User.VirtualId, "*Eu morri*", 0, User.LastBubble));

                    });
                    thrd.Start();
                }
                else
                {
                    Session.SendWhisper("Ese usuario está demasiado lejos, trata de acercarte.", 34);
                    return;
                }
            }
        }
    }