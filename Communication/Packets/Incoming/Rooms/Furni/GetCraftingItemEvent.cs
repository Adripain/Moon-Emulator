using System;
using System.Linq;
using System.Collections.Generic;

using Moon.HabboHotel.Items;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;

using Moon.Communication.Packets.Outgoing.Rooms.Furni;
using Moon.HabboHotel.Items.Crafting;

namespace Moon.Communication.Packets.Incoming.Rooms.Furni
{
    class GetCraftingItemEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            //var result = Packet.PopString();

            //CraftingRecipe recipe = null;
            //foreach (CraftingRecipe Receta in MoonEnvironment.GetGame().GetCraftingManager().CraftingRecipes.Values)
            //{
            //    if (Receta.Result.Contains(result))
            //    {
            //        recipe = Receta;
            //        break;
            //    }
            //}

            //var Final = MoonEnvironment.GetGame().GetCraftingManager().GetRecipe(recipe.Id);

            //Session.SendMessage(new CraftingResultComposer(recipe, true));
            //Session.SendMessage(new CraftableProductsComposer());
        }
    }
}