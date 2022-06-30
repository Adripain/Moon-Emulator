using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Rooms;
using System;
using System.Threading;


namespace Moon.HabboHotel.Items.Interactor
{
    class InteractorCrackable : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
            //nothing
        }

        public void OnRemove(GameClient Session, Item Item)
        {
            //nothing
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if ((Item.GetBaseItem().ItemName.Contains("egg") || Item.GetBaseItem().ItemName.Contains("bonusbag") || Item.GetBaseItem().ItemName.Contains("pinata") || Item.GetBaseItem().ItemName.Contains("rare")) && !HasRights)
                return;

            if (Gamemap.TilesTouching(Item.GetX, Item.GetY, Session.GetRoomUser().X, Session.GetRoomUser().Y))
            {
                Int32 clickCount = 0;
                Int32.TryParse(Item.ExtraData, out clickCount);
                clickCount++;
                Item.ExtraData = clickCount.ToString();
                Item.UpdateState(false, true);
                if (clickCount == ItemBehaviourUtility.getCrackableClicks(Item) && (Item.GetBaseItem().ItemName.Contains("egg") || Item.GetBaseItem().ItemName.Contains("bonusbag") || Item.GetBaseItem().ItemName.Contains("pinata")))
                {

                    Thread thread = new Thread(() => FinishCracking(Session, Item, Session.GetHabbo().CurrentRoom));
                    thread.Start();

                }
                else if (clickCount > ItemBehaviourUtility.getCrackableClicks(Item))
                {
                    Item.ExtraData = "0";
                    Item.UpdateState(false, true);
                }

            }
            else
            {
                Session.GetRoomUser().MoveTo(Item.SquareInFront);
            }
        }

        private void FinishCracking(GameClient Session, Item Egg, Room Room)
        {
            try
            {

                if (Egg == null || Room == null)
                    return;

                Thread.Sleep(1500); // let user see the egg pop

                int prize = GetCrackablePrize(Egg);
                Session.GetHabbo().CurrentRoom.GetRoomItemHandler().RemoveFurniture(Session, Egg.Id);

                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `items` SET `base_item` = @BaseItem, `extra_data` = @edata WHERE `id` = " + Egg.Id + " LIMIT 1");
                    dbClient.AddParameter("BaseItem", prize);
                    dbClient.AddParameter("edata", "");
                    dbClient.RunQuery();

                }

                Egg.BaseItem = prize;
                Egg.ResetBaseItem();
                Egg.ExtraData = "";

                if (Egg.Data.Type == 's')
                {
                    if (!Session.GetHabbo().CurrentRoom.GetRoomItemHandler().SetFloorItem(Session, Egg, Egg.GetX, Egg.GetY, Egg.Rotation, true, false, true))
                    {
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = " + Egg.Id + " LIMIT 1");
                        }

                    }
                }
                else
                {
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = " + Egg.Id + " LIMIT 1");
                    }

                }

                Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
            }
            catch
            {
            }
        }


        public static int GetCrackablePrize(Item item)
        {
            Random random = new Random();
            int furniID = 0;
            switch (item.GetBaseItem().ItemName)
            {
                case "easter13_egg_0":
                    furniID = random.Next(1, 4);
                    if (furniID > 3)
                        furniID = random.Next(202, 212); // range of available rare prizes by furni id
                    else
                        furniID = 1048;
                    break;
                case "easter13_egg_1":
                    furniID = random.Next(1, 4);
                    if (furniID > 2)
                        furniID = random.Next(202, 212);
                    else
                        furniID = 1048;
                    break;
                case "easter13_egg_2":
                    furniID = random.Next(1, 4);
                    if (furniID > 1)
                        furniID = random.Next(202, 212);
                    else
                        furniID = 1048;
                    break;
                case "easter13_egg_3":
                    furniID = random.Next(202, 212);
                    break;

                case "hblooza_pinata1":
                    furniID = random.Next(1, 4);
                    if (furniID > 3)
                        furniID = random.Next(202, 212);
                    else
                        furniID = 1048;
                    break;

                case "hblooza_pinata2":
                    furniID = random.Next(1, 4);
                    if (furniID > 2)
                        furniID = random.Next(202, 212);
                    else
                        furniID = 1048;
                    break;

                case "hween_c16_crackable1":
                    furniID = random.Next(1, 4);
                    if (furniID > 2)
                        furniID = random.Next(202, 212);
                    else
                        furniID = 1048;
                    break;

                default:
                    furniID = 1048;
                    break;
            }
            return furniID;
        }



        public void OnWiredTrigger(Item Item)
        {
            //nothing nice try though
        }
    }
}