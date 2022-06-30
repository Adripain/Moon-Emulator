using Moon.Communication.Packets.Outgoing.Inventory.Purse;
using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Users;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class BuyRoomCommand : IChatCommand
    {
        public string Description
        {
            get { return "Compra um quarto à venda."; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Room _Room = Session.GetHabbo().CurrentRoom;
            RoomUser RoomOwner = _Room.GetRoomUserManager().GetRoomUserByHabbo(_Room.OwnerName);
            if (_Room == null)
            {
                return;
            }
            if (_Room.OwnerName == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Você está tentando comprar um quarto que já é seu.", 34);
                return;
            }

            if (!Room.ForSale)
            {
                Session.SendWhisper("Este quarto não está à venda!", 34);
                return;
            }

            if (Session.GetHabbo().Duckets < _Room.SalePrice)
            {
                Session.SendWhisper("Você não tem Duckets suficientes para comprar esse quarto!", 34);
                return;
            }

            if (RoomOwner == null || RoomOwner.GetClient() == null)
            {
                Session.SendWhisper("Erro. Este quarto não está à venda!.", 34);
                _Room.ForSale = false;
                _Room.SalePrice = 0;
                return;
            }

            GameClient Owner = RoomOwner.GetClient();

            Owner.GetHabbo().Duckets += _Room.SalePrice;
            Owner.SendMessage(new HabboActivityPointNotificationComposer(Owner.GetHabbo().Duckets, _Room.SalePrice));
            Session.GetHabbo().Duckets -= _Room.SalePrice;
            Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, _Room.SalePrice));

            _Room.OwnerName = Session.GetHabbo().Username;
            _Room.OwnerId = (int)Session.GetHabbo().Id;
            _Room.RoomData.OwnerName = Session.GetHabbo().Username;
            _Room.RoomData.OwnerId = (int)Session.GetHabbo().Id;
            int RoomId = _Room.RoomId;



            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE rooms SET owner='" + Session.GetHabbo().Id + "' WHERE id='" + Room.RoomId + "' LIMIT 1");
                dbClient.RunQuery("UPDATE items SET user_id='" + Session.GetHabbo().Id + "' WHERE room_id='" + Room.RoomId + "'");
            }

            Session.GetHabbo().UsersRooms.Add(_Room.RoomData);
            Owner.GetHabbo().UsersRooms.Remove(_Room.RoomData);
            MoonEnvironment.GetGame().GetRoomManager().UnloadRoom(_Room);

            RoomData Data = MoonEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            Session.GetHabbo().PrepareRoom(Session.GetHabbo().CurrentRoom.RoomId, "");

        }
    }
}
