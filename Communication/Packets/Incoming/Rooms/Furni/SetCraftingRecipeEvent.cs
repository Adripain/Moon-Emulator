using System;
using Moon.Communication.Packets.Outgoing.Rooms.Furni;
using Moon.HabboHotel.Items.Crafting;

namespace Moon.Communication.Packets.Incoming.Rooms.Furni
{
    class SetCraftingRecipeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            var result = Packet.PopString();

            CraftingRecipe recipe = null;
            foreach (CraftingRecipe Receta in MoonEnvironment.GetGame().GetCraftingManager().CraftingRecipes.Values)
            {
                if (Receta.Result.Contains(result))
                {
                    recipe = Receta;
                    break;
                }
            }

            var Final = MoonEnvironment.GetGame().GetCraftingManager().GetRecipe(recipe.Id);
            if (Final == null) return;
            Session.SendMessage(new CraftingRecipeComposer(Final));
        }

    }
}