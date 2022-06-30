using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class CloseDiceCommand : IChatCommand
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
            get { return "Fechar os 5 dados perto do teu keko organizados de forma tradicional."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser roomUser = Room?.GetRoomUserManager()?.GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (roomUser == null)
            {
                return;
            }

            List<Items.Item> userBooth = Room.GetRoomItemHandler().GetFloor.Where(x => x != null && Gamemap.TilesTouching(
                x.Coordinate, roomUser.Coordinate) && x.Data.InteractionType == Items.InteractionType.DICE).ToList();

            if (userBooth.Count != 5)
            {
                Session.SendWhisper("Deve ter 5 dados perto de você.", 34);
                return;
            }

            userBooth.ForEach(x => {
                x.ExtraData = "0";
                x.UpdateState();
            });

            Session.SendWhisper("Os dados foram encerrados", 34);
        }
    }
}