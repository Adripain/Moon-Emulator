using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class SellRoomCommand : IChatCommand
    {
        public string Description
        {
            get { return "Coloca a sala em que você está para venda."; }
        }

        public string Parameters
        {
            get { return "%preço%"; }
        }

        public string PermissionRequired
        {
            get { return "user_normal"; }
        }


        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Deves colocar um preço.");
                return;
            }

            if (!Room.CheckRights(Session, true))
                return;

            if (Room == null)

                if (Params.Length == 1)
                {
                    Session.SendWhisper("Oops, você deve escolher um preço para vender este quarto.");
                    return;
                }
                else if (Room.Group != null)
                {
                    Session.SendWhisper("Oops, ao que parece está sala tem um grupo, para poder vender, delete o grupo.");
                    return;
                }

            int Value = 0;
            if (!int.TryParse(Params[1], out Value))
            {
                Session.SendWhisper("Oops, coloque um valor valido");
                return;
            }

            if (Value < 0)
            {
                Session.SendWhisper("Nã é possivel vender a sala com um valor negativo.");
                return;
            }

            if (Room.ForSale)
            {
                Room.SalePrice = Value;
            }
            else
            {
                Room.ForSale = true;
                Room.SalePrice = Value;
            }

            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers())
            {
                if (User == null || User.GetClient() == null)
                    continue;

                Session.SendWhisper("Está sala esta a venda, seu preço atual é de :  " + Value + " Duckets!Compre-a esrevendo :comprarsala.");
            }

            Session.SendNotification("Se você quer vender uma sala, deve colocar um valor numérico. \n\nPOR FAVOR NOTA:\nSe você vender um quarto, não poderá recupera-lo.!\n\n" +
        "Você pode cancelar uma venda escrevendo ':unload' (sem as '')");

            return;
        }
    }
}
