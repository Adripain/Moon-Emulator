using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.HabboHotel.Items;

namespace Moon.Communication.Packets.Outgoing.Rooms.Furni
{
    class CraftableProductsComposer : ServerPacket
    {
        public CraftableProductsComposer(Item item)
            : base(ServerPacketHeader.CraftableProductsMessageComposer)
        {
            int total = 0;
            var crafting = MoonEnvironment.GetGame().GetCraftingManager();
            foreach (var recipe in crafting.CraftingRecipes.Values)
            {
                if (recipe.Type == item.GetBaseItem().Id)
                {
                    int count = total + 1;
                    total = count;
                }
            }
            base.WriteInteger(total); //crafting.CraftingRecipes.Count
            foreach (var recipe in crafting.CraftingRecipes.Values)
            {
                if (recipe.Type == item.GetBaseItem().Id)
                {
                    base.WriteString(recipe.Result);
                    base.WriteString(recipe.Result);
                }
            }
            base.WriteInteger(crafting.CraftableItems.Count);
            foreach (var itemName in crafting.CraftableItems)
            {
                base.WriteString(itemName);
            }
        }
    }
}