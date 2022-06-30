using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableWhispersCommand : IChatCommand
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
            get { return "Ativa ou desativa a opção de receber sussurros."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().ReceiveWhispers = !Session.GetHabbo().ReceiveWhispers;
            Session.SendWhisper("Você " + (Session.GetHabbo().ReceiveWhispers ? "agora" : "não") + " recebe susurros!", 34);
        }
    }
}
