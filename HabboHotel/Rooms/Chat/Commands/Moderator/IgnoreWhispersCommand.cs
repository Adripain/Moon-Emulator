using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class IgnoreWhispersCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_4"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Te permite que você ignore todos os sussurros da sala"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().IgnorePublicWhispers = !Session.GetHabbo().IgnorePublicWhispers;
            Session.SendWhisper("Você " + (Session.GetHabbo().IgnorePublicWhispers ? "agora" : "já não") + "Ignora os sussurros dos outros!", 34);
        }
    }
}
