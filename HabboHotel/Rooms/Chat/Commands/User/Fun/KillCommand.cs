using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Pathfinding;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class KillCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return "[USUARIO]"; }
        }

        public string Description
        {
            get { return "Matar o usuário x.x"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Escreva o nome da sua vítima.", 34);
                return;
            }

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);

            if (TargetClient == null)
            {
                Session.SendWhisper("¡Oops!Provavelmente o usuário não está online.", 34);
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            if (TargetUser == null)
            {
                Session.SendWhisper("¡Oops! Provavelmente o usuário não está no quarto", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Não cometer suicídio! :(", 34);
                return;
            }

            if (TargetClient.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.SendWhisper("Você não pode matar este usuário.", 34);
                return;
            }

            if (TargetUser.isLying || TargetUser.isSitting)
            {
                Session.SendWhisper("Você não pode matá - lo assim...", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == "Forbi" || TargetClient.GetHabbo().Username == "Forb")
            {
                Session.SendWhisper("Você não pode matar esse usuário!", 34);
                return;
            }

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            if (!((Math.Abs(TargetUser.X - ThisUser.X) > 1) || (Math.Abs(TargetUser.Y - ThisUser.Y) > 1)))
            {
                Room.SendMessage(new ChatComposer(ThisUser.VirtualId, " *Ele assassinou " + Params[1] + " * ", 0, ThisUser.LastBubble));
                Room.SendMessage(new ChatComposer(TargetUser.VirtualId, "Oh não, eu morri x.x", 0, TargetUser.LastBubble));
                TargetUser.RotBody--;//
                TargetUser.Statusses.Add("lay", "1.0 null");
                TargetUser.Z -= 0.35;
                TargetUser.isLying = true;
                TargetUser.UpdateNeeded = true;
            }
            else
            {
                Session.SendWhisper("Oops! " + Params[1] + " está muito longe.", 34);
            }
        }
    }
}