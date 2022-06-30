using System;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Furni;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Items.Interactor
{
    class InteractorCrafting : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            Session.SendMessage(new MassEventComposer("inventory/open"));
            Session.SendMessage(new CraftableProductsComposer(Item));
            Session.GetHabbo().LastCraftingMachine = Item.GetBaseItem().Id;
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}