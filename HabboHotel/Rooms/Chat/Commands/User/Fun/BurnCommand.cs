using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class BurnCommand : IChatCommand
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
            get { return "Queimar outro usuário."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Introduza o nome do usuário que você deseja queimar. { :queimar NOME }", 34);
                return;
            }

            //if (!Room.QuemarEnabled && !Session.GetHabbo().GetPermissions().HasRight("room_override_custom_config"))
            //{
            //    Session.SendWhisper("@red@ Oops, el dueño de la sala no permite que quemes a otros en su sala.", 34);
            //    return;
            //}

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Houve um problema, aparentemente o usuário não está online ou você não escreveu o nome corretamente", 34);
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetUser == null)
            {
                Session.SendWhisper("Ocorreu um erro, escreva o nome corretamente, o usuário NÃO está on - line ou na sala.", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == "Forbi" || TargetClient.GetHabbo().Username == "Forb")
            {
                Session.SendWhisper("¡El es tu patón!", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Não é ruim que você tente se queimar ... mas pode parecer estranho e eles vão pensar que você é louco...", 34);
                return;
            }

            if (TargetUser.TeleportEnabled)
            {
                Session.SendWhisper("Oops, Você não pode queimar alguém se você usar o teleport.", 34);
                return;
            }

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            if (!((Math.Abs(TargetUser.X - ThisUser.X) >= 2) || (Math.Abs(TargetUser.Y - ThisUser.Y) >= 2)))
            {
                Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*Eu começo a queimar  " + Params[1] + "!", 0, ThisUser.LastBubble));
                ThisUser.ApplyEffect(5);
                Room.SendMessage(new ChatComposer(TargetUser.VirtualId, "Ajuda! Ele está me queimando!", 0, ThisUser.LastBubble));
                TargetUser.ApplyEffect(25);
            }
            else
            {
                Session.SendWhisper("@green@ Oops, " + Params[1] + " Não está perto o suficiente!", 34);
            }
        }
    }
}
