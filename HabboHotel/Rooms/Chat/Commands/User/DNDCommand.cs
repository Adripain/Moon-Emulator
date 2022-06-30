using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class DNDCommand : IChatCommand
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
            get { return "Ativa ou desativa as mensagens do console."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowConsoleMessages = !Session.GetHabbo().AllowConsoleMessages;
            Session.SendWhisper("Você " + (Session.GetHabbo().AllowConsoleMessages == true ? "agora" : "não") + " aceita receber mensagem no console de amigos.", 34);
        }
    }
}