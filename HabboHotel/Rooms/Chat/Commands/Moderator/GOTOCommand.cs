using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GOTOCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_vip"; }
        }

        public string Parameters
        {
            get { return "[ROOMID]"; }
        }

        public string Description
        {
            get { return "Ir para algum quarto"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Você deve especificar o ID do quarto", 34);
                return;
            }

            int RoomID;

            if (!int.TryParse(Params[1], out RoomID))
                Session.SendWhisper("Você deve escrever o ID do quarto corretamente", 34);
            else
            {
                Room _room = MoonEnvironment.GetGame().GetRoomManager().LoadRoom(RoomID);
                if (_room == null)
                    Session.SendWhisper("Esse quarto não existe!", 34);
                else
                {
                    Session.GetHabbo().PrepareRoom(_room.Id, "");
                }
            }
        }
    }
}