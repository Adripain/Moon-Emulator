using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class SelfieCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_vip"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Tirar uma selfie"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
                RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (ThisUser == null)
                    return;
                Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*Pego meu iPhone*", 0, ThisUser.LastBubble));
                Session.GetHabbo().Effects().ApplyEffect(65);
                Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*E tiro uma Selfie*", 0, ThisUser.LastBubble));

            }
        }
    }
