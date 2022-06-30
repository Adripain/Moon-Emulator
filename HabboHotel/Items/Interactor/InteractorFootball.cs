/*using System;
using System.Drawing;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Items.Interactor
{
    public class InteractorFootball : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Session == null || Session.GetHabbo() == null || Item == null || Item.GetRoom() == null)
                return;

            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (User == null || Item.GetRoom().Shoot == 0)
                return;

            if (!((Math.Abs((User.X - Item.GetX)) >= 2) || (Math.Abs((User.Y - Item.GetY)) >= 2)))
            {
                Point NewPoint = new Point(User.X, User.Y);
                Item.ExtraData = "55";
                Item.BallIsMoving = true;
                Item.BallValue = 1;
                Item.GetRoom().GetSoccer().MoveBall(Item, User, NewPoint, true, Item.GetRoom().Shoot);
            }
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}
*/