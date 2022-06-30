using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Database.Interfaces;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class spamCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_6"; }
        }
        public string Parameters
        {
            get { return ""; }
        }
        public string Description
        {
            get { return "Evitar spam!"; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null || User.GetClient() == null)
                return;

            //Comando por Desconocido.

            else
            {
                Room.SendMessage(new ChatComposer(User.VirtualId, "**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM* *NÃO SPAM**", 0, User.LastBubble));
                Room.SendMessage(new ChatComposer(User.VirtualId, "**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM* *NÃO SPAM**", 0, User.LastBubble));
                Room.SendMessage(new ChatComposer(User.VirtualId, "**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM* *NÃO SPAM**", 0, User.LastBubble));
                Room.SendMessage(new ChatComposer(User.VirtualId, "**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM* *NÃO SPAM**", 0, User.LastBubble));
                Room.SendMessage(new ChatComposer(User.VirtualId, "**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM* *NÃO SPAM**", 0, User.LastBubble));
                Room.SendMessage(new ChatComposer(User.VirtualId, "**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM* *NÃO SPAM**", 0, User.LastBubble));
                Room.SendMessage(new ChatComposer(User.VirtualId, "**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM* *NÃO SPAM**", 0, User.LastBubble));
                Room.SendMessage(new ChatComposer(User.VirtualId, "**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM* *NÃO SPAM**", 0, User.LastBubble));
                Room.SendMessage(new ChatComposer(User.VirtualId, "**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM* *NÃO SPAM**", 0, User.LastBubble));
                Room.SendMessage(new ChatComposer(User.VirtualId, "**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM* *NÃO SPAM**", 0, User.LastBubble));
                Room.SendMessage(new ChatComposer(User.VirtualId, "**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM* *NÃO SPAM**", 0, User.LastBubble));
                Room.SendMessage(new ChatComposer(User.VirtualId, "**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM* *NÃO SPAM**", 0, User.LastBubble));
                Room.SendMessage(new ChatComposer(User.VirtualId, "**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM* *NÃO SPAM**", 0, User.LastBubble));
                Room.SendMessage(new ChatComposer(User.VirtualId, "**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM* *NÃO SPAM**", 0, User.LastBubble));
                Room.SendMessage(new ChatComposer(User.VirtualId, "**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM* *NÃO SPAM**", 0, User.LastBubble));
                Room.SendMessage(new ChatComposer(User.VirtualId, "**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM**NÃO SPAM* *NÃO SPAM**", 0, User.LastBubble));

                Room.SendMessage(new ChatComposer(User.VirtualId, "*Olá perdoa o inconveniente att: " + Session.GetHabbo().Username + "", 33, User.LastBubble));
            }
        }
    }
}
