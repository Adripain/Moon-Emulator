using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableSpamCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Ativa ou desativa a opção de receber spam."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Session.GetHabbo().AllowMessengerInvites = false;
            Session.SendWhisper("Você " + (Session.GetHabbo().AllowMessengerInvites ? "agora" : "agora não") + "recebe spams no console!", 34);
        }
    }
}
