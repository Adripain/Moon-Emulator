using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

using log4net;
using Moon.Core;

using Moon.Database.Interfaces;


namespace Moon.HabboHotel.Items
{
    public class ItemDataManager
    {
        private static readonly ILog log = LogManager.GetLogger("Moon.HabboHotel.Items.ItemDataManager");

        public Dictionary<int, ItemData> _items;
        public Dictionary<int, ItemData> _gifts;//<SpriteId, Item>

        public ItemDataManager()
        {
            this._items = new Dictionary<int, ItemData>();
            this._gifts = new Dictionary<int, ItemData>();
        }

        public void Init()
        {
            if (this._items.Count > 0)
                this._items.Clear();

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`item_name`,`public_name`,`type`,`width`,`length`,`stack_height`,`can_stack`,`can_sit`,`is_walkable`,`sprite_id`,`allow_recycle`,`allow_trade`,`allow_marketplace_sell`,`allow_gift`,`allow_inventory_stack`,`interaction_type`,`interaction_modes_count`,`vending_ids`,`height_adjustable`,`effect_id`,`wired_id`,`is_rare`,`clothing_id`, `extra_rot`,  `song_Id` FROM `furniture`");
                DataTable ItemData = dbClient.getTable();

                if (ItemData != null)
                {
                    foreach (DataRow Row in ItemData.Rows)
                    {
                        try
                        {
                            int id = Convert.ToInt32(Row["id"]);
                            int spriteID = Convert.ToInt32(Row["sprite_id"]);
                            string itemName = Convert.ToString(Row["item_name"]);
                            string PublicName = Convert.ToString(Row["public_name"]);
                            string type = Row["type"].ToString();
                            int width = Convert.ToInt32(Row["width"]);
                            int length = Convert.ToInt32(Row["length"]);
                            double height = Convert.ToDouble(Row["stack_height"]);
                            bool allowStack = MoonEnvironment.EnumToBool(Row["can_stack"].ToString());
                            bool allowWalk = MoonEnvironment.EnumToBool(Row["is_walkable"].ToString());
                            bool allowSit = MoonEnvironment.EnumToBool(Row["can_sit"].ToString());
                            bool allowRecycle = MoonEnvironment.EnumToBool(Row["allow_recycle"].ToString());
                            bool allowTrade = MoonEnvironment.EnumToBool(Row["allow_trade"].ToString());
                            bool allowMarketplace = Convert.ToInt32(Row["allow_marketplace_sell"]) == 1;
                            bool allowGift = Convert.ToInt32(Row["allow_gift"]) == 1;
                            bool allowInventoryStack = MoonEnvironment.EnumToBool(Row["allow_inventory_stack"].ToString());
                            InteractionType interactionType = InteractionTypes.GetTypeFromString(Convert.ToString(Row["interaction_type"]));
                            int cycleCount = Convert.ToInt32(Row["interaction_modes_count"]);
                            string vendingIDS = Convert.ToString(Row["vending_ids"]);
                            string heightAdjustable = Convert.ToString(Row["height_adjustable"]);
                            int EffectId = Convert.ToInt32(Row["effect_id"]);
                            int WiredId = Convert.ToInt32(Row["wired_id"]);
                            bool IsRare = MoonEnvironment.EnumToBool(Row["is_rare"].ToString());
                            int ClothingId = Convert.ToInt32(Row["clothing_id"]);
                            bool ExtraRot = MoonEnvironment.EnumToBool(Row["extra_rot"].ToString());
                            int SongID = Convert.ToInt32(Row["song_Id"]);

                            if (!this._gifts.ContainsKey(spriteID))
                                this._gifts.Add(spriteID, new ItemData(id, spriteID, itemName, PublicName, type, width, length, height, allowStack, allowWalk, allowSit, allowRecycle, allowTrade, allowMarketplace, allowGift, allowInventoryStack, interactionType, cycleCount, vendingIDS, heightAdjustable, EffectId, WiredId, IsRare, ClothingId, ExtraRot, SongID));

                            if (!this._items.ContainsKey(id))
                                this._items.Add(id, new ItemData(id, spriteID, itemName, PublicName, type, width, length, height, allowStack, allowWalk, allowSit, allowRecycle, allowTrade, allowMarketplace, allowGift, allowInventoryStack, interactionType, cycleCount, vendingIDS, heightAdjustable, EffectId, WiredId, IsRare, ClothingId, ExtraRot, SongID));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.ReadKey();
                            Logging.WriteLine("Could not load item #" + Convert.ToInt32(Row[0]) + ", please verify the data is okay.");
                        }
                    }
                }
            }

            log.Info(">> Items Manager -> Pronto!");
        }

        public bool GetItem(int Id, out ItemData Item)
        {
            if (this._items.TryGetValue(Id, out Item))
                return true;
            return false;
        }

        internal ItemData GetItemByName(string name)
        {
            foreach (KeyValuePair<int, ItemData> entry in _items)
            {
                ItemData item = entry.Value;
                if (item.ItemName == name)
                    return item;
            }
            return null;
        }

        public bool GetGift(int SpriteId, out ItemData Item)
        {
            if (this._gifts.TryGetValue(SpriteId, out Item))
                return true;
            return false;
        }
    }
}