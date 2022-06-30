using Moon.Communication.Packets.Outgoing.Campaigns;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Users;
using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Incoming.Calendar
{
    class OpenCalendarBoxEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string CampaignName = Packet.PopString();
            int CampaignDay = Packet.PopInt(); // INDEX VALUE.

            // Si no es el nombre de campaña actual.
            if (CampaignName != MoonEnvironment.GetGame().GetCalendarManager().GetCampaignName())
                return;

            // Si es un día inválido.
            if (CampaignDay < 0 || CampaignDay > MoonEnvironment.GetGame().GetCalendarManager().GetTotalDays() - 1 || CampaignDay < MoonEnvironment.GetGame().GetCalendarManager().GetUnlockDays())
                // Mini fix
                return;



            // Días próximos
            if (CampaignDay > MoonEnvironment.GetGame().GetCalendarManager().GetUnlockDays())

                return;


            // Esta recompensa ya ha sido recogida.
            if (Session.GetHabbo().calendarGift[CampaignDay])

                return;


            Session.GetHabbo().calendarGift[CampaignDay] = true;

            // PACKET PARA ACTUALIZAR?
            Session.SendMessage(new CalendarPrizesComposer(MoonEnvironment.GetGame().GetCalendarManager().GetCampaignDay(CampaignDay + 1)));
            Session.SendMessage(new CampaignCalendarDataComposer(Session.GetHabbo().calendarGift));


            string Gift = MoonEnvironment.GetGame().GetCalendarManager().GetGiftByDay(CampaignDay + 1);
            string GiftType = Gift.Split(':')[0];
            string GiftValue = Gift.Split(':')[1];

            switch (GiftType.ToLower())
            {
                case "itemid":
                    {
                        ItemData Item = null;
                        if (!MoonEnvironment.GetGame().GetItemManager().GetItem(int.Parse(GiftValue), out Item))
                        {
                            // No existe este ItemId.
                            return;
                        }

                        Item GiveItem = ItemFactory.CreateSingleItemNullable(Item, Session.GetHabbo(), "", "");
                        if (GiveItem != null)
                        {
                            Session.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);

                            Session.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
                            Session.SendMessage(new FurniListUpdateComposer());
                        }

                        Session.GetHabbo().GetInventoryComponent().UpdateItems(false);
                    }
                    break;

                case "badge":
                    {
                        Session.GetHabbo().GetBadgeComponent().GiveBadge(GiftValue, true, Session);
                    }
                    break;

                case "diamonds":
                    {
                        Session.GetHabbo().Diamonds += int.Parse(GiftValue);
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                    }
                    break;

                case "gotwpoints":
                    {
                        Session.GetHabbo().GOTWPoints += int.Parse(GiftValue);
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                    }
                    break;

                case "vip":
                    {
                        var IsVIP = Session.GetHabbo().GetClubManager().HasSubscription("club_vip");
                        if (IsVIP)
                        {
                            Session.SendMessage(new AlertNotificationHCMessageComposer(4));
                        }
                        else
                        {
                            Session.SendMessage(new AlertNotificationHCMessageComposer(5));
                        }
                        if (Session.GetHabbo().Rank > 2)
                        {
                            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.RunQuery("UPDATE `users` SET `rank_vip` = '1' WHERE `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                            }
                        }
                        else
                        {
                            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.RunQuery("UPDATE `users` SET `rank` = '2' WHERE `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                                dbClient.RunQuery("UPDATE `users` SET `rank_vip` = '1' WHERE `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                            }
                        }

                        Session.GetHabbo().GetClubManager().AddOrExtendSubscription("club_vip", int.Parse(GiftValue) * 24 * 3600, Session);
                        Session.GetHabbo().GetBadgeComponent().GiveBadge("VIP", true, Session);

                        MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_VipClub", 1);
                        Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    }
                    break;
            }

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("INSERT INTO user_campaign_gifts VALUES (NULL, '" + Session.GetHabbo().Id + "','" + CampaignName + "','" + (CampaignDay + 1) + "')");
            }
        }
    }
}
