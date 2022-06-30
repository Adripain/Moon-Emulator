using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class MultiwhisperModeCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_5"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Ativa ou desativa solicitações de amizade."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Session.GetHabbo().MultiWhisper = !Session.GetHabbo().MultiWhisper;
            Session.SendWhisper("Agora mesmo " + (Session.GetHabbo().MultiWhisper == true ? "você não aceita" : "aceita") + " novos pedidos de amizade");
        }
    }
}