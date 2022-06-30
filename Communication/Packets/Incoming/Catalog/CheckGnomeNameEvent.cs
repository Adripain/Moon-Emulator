//using System;
//using System.Linq;
//using System.Text;
//using System.Collections.Generic;

//using Moon.HabboHotel.Rooms;
//using Moon.HabboHotel.Items;
//using Moon.HabboHotel.Rooms.AI;
//using Moon.HabboHotel.Rooms.AI.Speech;
//using Moon.HabboHotel.Items.Utilities;



//using Moon.Communication.Packets.Outgoing.Catalog;
//using Moon.Communication.Packets.Outgoing.Inventory.Furni;

//using Moon.Database.Interfaces;
//using Moon.HabboHotel.Rooms.AI.Responses;

//namespace Moon.Communication.Packets.Incoming.Catalog
//{
//    class CheckGnomeNameEvent : IPacketEvent
//    {
//        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
//        {
//            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
//                return;

//            Room Room = Session.GetHabbo().CurrentRoom;
//            if (Room == null)
//                return;

//            int ItemId = Packet.PopInt();
//            Item Item = Room.GetRoomItemHandler().GetItem(ItemId);
//            if (Item == null || Item.Data == null || Item.UserID != Session.GetHabbo().Id || Item.Data.InteractionType != InteractionType.GNOME_BOX)
//                return;

//            string PetName = Packet.PopString();
//            if (string.IsNullOrEmpty(PetName))
//            {
//                Session.SendMessage(new CheckGnomeNameComposer(PetName, 1));
//                return;
//            }

//            int X = Item.GetX;
//            int Y = Item.GetY;

//            //Quickly delete it from the database.
//            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
//            {
//                dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @ItemId LIMIT 1");
//                dbClient.AddParameter("ItemId", Item.Id);
//                dbClient.RunQuery();
//            }

//            //Remove the item.
//            Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);

//            //Apparently we need this for success.
//            Session.SendMessage(new CheckGnomeNameComposer(PetName, 0));

//            int PetRace = Item.GetBaseItem().ClothingId;

//            string Race = RandonColor();
//            //Create the pet here.
//            Pet Pet = PetUtility.CreatePet(Session.GetHabbo().Id, PetName, PetRace, Race, "ffffff");
//            if (Pet == null)
//            {
//                Session.SendNotification("Oops, ocurrio un error, reporte esto en Atencion al Cliente");
//                return;
//            }

//            List<RandomSpeech> RndSpeechList = new List<RandomSpeech>();
//            List<BotResponse> BotResponse = new List<BotResponse>();

//            Pet.RoomId = Session.GetHabbo().CurrentRoomId;
//            Pet.GnomeClothing = RandomClothing();
//            Pet.Race = RandonColor();
//            if (PetRace == 26)
//            {
//                //Update the pets gnome clothing.
//                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
//                {
//                    dbClient.SetQuery("UPDATE `bots_petdata` SET `gnome_clothing` = @GnomeClothing WHERE `id` = @PetId LIMIT 1");
//                    dbClient.AddParameter("GnomeClothing", Pet.GnomeClothing);
//                    dbClient.AddParameter("PetId", Pet.PetId);
//                    dbClient.RunQuery();
//                }
//            }

//            Pet.DBState = DatabaseUpdateState.NeedsUpdate;
//            Room.GetRoomUserManager().UpdatePets();

//            //Make a RoomUser of the pet.
//            //RoomUser PetUser = Room.GetRoomUserManager().DeployBot(new RoomBot(Pet.PetId, Pet.RoomId, "pet", "freeroam", Pet.Name, "", Pet.Look, X, Y, 0, 0, 0, 0, 0, 0, ref RndSpeechList, "", 0, Pet.OwnerId, false, 0, false, 0), Pet);
//            RoomBot RoomBot = new RoomBot(Pet.PetId, Pet.RoomId, "pet", "freeroam", Pet.Name, "", Pet.Look, X, Y, 0, 0, 0, 0, 0, 0, ref RndSpeechList, "", 0, Pet.OwnerId, false, 0, false, 0);
//            if (RoomBot == null)
//                return;

//            Room.GetRoomUserManager().DeployBot(RoomBot, Pet);

//            Pet.DBState = DatabaseUpdateState.NeedsUpdate;
//            Room.GetRoomUserManager().UpdatePets();

//            Pet ToRemove = null;
//            //Give the food.
//            ItemData PetFood = null;
//            if (MoonEnvironment.GetGame().GetItemManager().GetItem(320, out PetFood))
//            {
//                Item Food = ItemFactory.CreateSingleItemNullable(PetFood, Session.GetHabbo(), "", "");
//                if (Food != null)
//                {
//                    Session.GetHabbo().GetInventoryComponent().TryAddItem(Food);
//                    Session.SendMessage(new FurniListNotificationComposer(Food.Id, 1));
//                }
//            }
//        }

//        private static string RandomClothing()
//        {
//            Random Random = new Random();

//            int RandomNumber = Random.Next(1, 6);
//            switch (RandomNumber)
//            {
//                default:
//                case 1:
//                    return "5 0 -1 0 4 402 5 3 301 4 1 101 2 2 201 3";
//                case 2:
//                    return "5 0 -1 0 1 102 13 3 301 4 4 401 5 2 201 3";
//                case 3:
//                    return "5 1 102 8 2 201 16 4 401 9 3 303 4 0 -1 6";
//                case 4:
//                    return "5 0 -1 0 3 303 4 4 401 5 1 101 2 2 201 3";
//                case 5:
//                    return "5 3 302 4 2 201 11 1 102 12 0 -1 28 4 401 24";
//                case 6:
//                    return "5 4 402 5 3 302 21 0 -1 7 1 101 12 2 201 17";
//            }
//        }
//        private static string RandonColor()
//        {
//            Random Random = new Random();

//            int RandomNumber = Random.Next(6, 12);
//            switch (RandomNumber)
//            {
//                default:
//                case 1:
//                    return "1";
//                case 2:
//                    return "2";
//                case 3:
//                    return "3";
//                case 4:
//                    return "4";
//                case 5:
//                    return "5";
//                case 6:
//                    return "6";
//                case 7:
//                    return "7";
//                case 8:
//                    return "8";
//                case 9:
//                    return "9";
//                case 10:
//                    return "10";
//                case 11:
//                    return "11";
//                case 12:
//                    return "12";
//            }
//        }
//    }
//}
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Items;
using Moon.HabboHotel.Rooms.AI;
using Moon.HabboHotel.Rooms.AI.Speech;
using Moon.HabboHotel.Items.Utilities;



using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;

using Moon.Database.Interfaces;
using Moon.HabboHotel.Rooms.AI.Responses;
using Moon.Communication.Packets.Outgoing.Notifications;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Moderation;
using System.ServiceModel.Channels;

namespace Moon.Communication.Packets.Incoming.Catalog
{
    class CheckGnomeNameEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            int Id = Packet.PopInt();
            string Pin = Packet.PopString();

            if (Pin == Session.GetHabbo().PinClient)
            {
                MoonEnvironment.GetGame().GetClientManager().ManagerAlert(RoomNotificationComposer.SendBubble("estaff", "O usuário " + Session.GetHabbo().Username + " foi autentificado corretamente!", ""));
                Session.SendMessage(new CheckGnomeNameComposer(Pin, 0));
                Session.SendMessage(new GraphicAlertComposer("staff"));
                Session.GetHabbo().StaffOk = true;
                if (Session.GetHabbo().GetPermissions().HasRight("mod_tickets"))
                {
                    Session.SendMessage(new ModeratorInitComposer(
                      MoonEnvironment.GetGame().GetModerationManager().UserMessagePresets,
                      MoonEnvironment.GetGame().GetModerationManager().RoomMessagePresets,
                      MoonEnvironment.GetGame().GetModerationManager().GetTickets));
                }
            }
            else
            {
                Session.SendMessage(new CheckGnomeNameComposer(Pin, 0));
                Session.SendMessage(new RoomCustomizedAlertComposer("AVISO: el pin es incorrecto se avisárá a todo el equipo Staff."));
                MoonEnvironment.GetGame().GetClientManager().ManagerAlert(new MOTDNotificationComposer("ATENCION: El Staff " + Session.GetHabbo().Username + " ha fallado al autentificar su identidad."));
                Session.GetConnection().Dispose();
                this.LogCommand(Session.GetHabbo().Id, "Inicio de sesión inválido", Session.GetHabbo().MachineId, Session.GetHabbo().Username);
            }


        }
        public void LogCommand(int UserId, string Data, string MachineId, string Username)
        {
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `logs_client_staff` (`user_id`,`data_string`,`machine_id`, `timestamp`) VALUES (@UserId,@Data,@MachineId,@Timestamp)");
                dbClient.AddParameter("UserId", UserId);
                dbClient.AddParameter("Data", Data);
                dbClient.AddParameter("MachineId", MachineId);
                dbClient.AddParameter("Timestamp", MoonEnvironment.GetUnixTimestamp());
                dbClient.RunQuery();
            }
        }
    }
}