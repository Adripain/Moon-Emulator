using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class MassiveEventCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_13"; }
        }

        public string Parameters
        {
            get { return "[EVENTO]"; }
        }

        public string Description
        {
            get { return "Envia um evento a todos os usuarios online."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor escreva o evento a realizar.");
                return;
            }

            string Event = CommandManager.MergeParams(Params, 1);
            MoonEnvironment.GetGame().GetClientManager().SendMessage(new MassEventComposer(Event));
            return;
        }
    }
}
