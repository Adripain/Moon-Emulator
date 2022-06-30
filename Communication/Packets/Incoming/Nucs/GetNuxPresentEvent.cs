using System;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;
using Moon.Utilities;
using Moon.HabboHotel.Items;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.Communication.Packets.Incoming.Rooms.Nux
{
    class GetNuxPresentEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int Data1 = Packet.PopInt(); // ELEMENTO 1
            int Data2 = Packet.PopInt(); // ELEMENTO 2
            int Data3 = Packet.PopInt(); // ELEMENTO 3
            int Data4 = Packet.PopInt(); // SELECTOR
            var RewardName = "";

            switch (Data4)
            {
                case 0:
                    int RewardDiamonds = RandomNumber.GenerateRandom(0, 5);
                    Session.GetHabbo().Diamonds += RewardDiamonds;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, RewardDiamonds, 5));
                    break;
                case 1:
                    int RewardDuckets = RandomNumber.GenerateRandom(25, 50);

                    Session.GetHabbo().Duckets += RewardDuckets;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, RewardDuckets));
                    Session.SendMessage(RoomNotificationComposer.SendBubble("duckets", "¡Usted ha recibido " + RewardDuckets + " Duckets!", ""));


                    break;
                case 2:
                    int RewardItem = RandomNumber.GenerateRandom(1, 10);
                    var RewardItemId = 0;

                    switch (RewardItem)
                    {
                        case 1:
                            RewardItemId = 212; // VIP - club_sofa
                            RewardName = "Sofá VIP";
                            break;
                        case 2:
                            RewardItemId = 9510; // Elefante Azul Colorable - rare_colourable_elephant_statue*1
                            RewardName = "Elefante Azul Colorable";
                            break;
                        case 3:
                            RewardItemId = 1587; // Lámpara Calippo - ads_calip_lava
                            RewardName = "Lámpara de Lava Calippo";
                            break;
                        case 4:
                            RewardItemId = 212; // VIP - club_sofa
                            RewardName = "Sofá VIP";
                            break;
                        case 5:
                            RewardItemId = 385; // Toldo Amarillo - marquee*4
                            RewardName = "Toldo Amarillo";
                            break;
                        case 6:
                            RewardItemId = 212; // VIP - club_sofa
                            RewardName = "Sofá VIP";
                            break;
                        case 7:
                            RewardItemId = 212; // VIP - club_sofa
                            RewardName = "Sofá VIP";
                            break;
                        case 8:
                            RewardItemId = 9506; // Parasol Azul - rare_colourable_parasol*1
                            RewardName = "Parasol Azul";
                            break;
                        case 9:
                            RewardItemId = 212; // VIP - club_sofa
                            RewardName = "Sofá VIP";
                            break;
                        case 10:
                            RewardItemId = 212; // VIP - club_sofa
                            RewardName = "Sofá VIP";
                            break;
                    }
                    ItemData Item = null;
                    if (!MoonEnvironment.GetGame().GetItemManager().GetItem(RewardItemId, out Item))
                    { return; }

                    Item GiveItem = ItemFactory.CreateSingleItemNullable(Item, Session.GetHabbo(), "", "");
                    if (GiveItem != null)
                    {
                        Session.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);

                        Session.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
                        Session.SendMessage(new FurniListUpdateComposer());
                        Session.SendMessage(RoomNotificationComposer.SendBubble("voucher", "Acabas de recibir un " + RewardName + ".\n\n¡Corre, " + Session.GetHabbo().Username + ", revisa tu inventario, hay algo nuevo al parecer!", ""));
                    }

                    Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                    break;
            }
            MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_AnimationRanking", 1);
        }
    }
}