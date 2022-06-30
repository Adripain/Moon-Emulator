using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;

using log4net;

using Moon.HabboHotel.Items;
using Moon.HabboHotel.Catalog.Pets;
using Moon.HabboHotel.Catalog.Vouchers;
using Moon.HabboHotel.Catalog.Marketplace;
using Moon.HabboHotel.Catalog.Clothing;

using Moon.Database.Interfaces;
using Moon.HabboHotel.Catalog.PredesignedRooms;

namespace Moon.HabboHotel.Catalog
{
    public class CatalogManager
    {
        private static readonly ILog log = LogManager.GetLogger("Moon.HabboHotel.Catalog.CatalogManager");

        private MarketplaceManager _marketplace;
        private PetRaceManager _petRaceManager;
        private VoucherManager _voucherManager;
        private ClothingManager _clothingManager;
        private PredesignedRoomsManager _predesignedManager;

        private Dictionary<int, int> _itemOffers;
        private Dictionary<int, CatalogPage> _pages;
        private Dictionary<int, BCCatalogPage> _bcpages;
        private Dictionary<int, CatalogBot> _botPresets;
        private Dictionary<int, Dictionary<int, CatalogItem>> _items;
        private Dictionary<int, Dictionary<int, BCCatalogItem>> _bcitems;
        private Dictionary<int, Dictionary<int, CatalogDeal>> _deals;
        private Dictionary<int, PredesignedContent> _predesignedItems;

        public CatalogManager()
        {
            this._marketplace = new MarketplaceManager();
            this._petRaceManager = new PetRaceManager();
            this._voucherManager = new VoucherManager();
            this._clothingManager = new ClothingManager();
            this._predesignedManager = new PredesignedRoomsManager();
            this._predesignedManager.Initialize();

            this._itemOffers = new Dictionary<int, int>();
            this._pages = new Dictionary<int, CatalogPage>();
            this._bcpages = new Dictionary<int, BCCatalogPage>();
            this._botPresets = new Dictionary<int, CatalogBot>();
            this._items = new Dictionary<int, Dictionary<int, CatalogItem>>();
            this._bcitems = new Dictionary<int, Dictionary<int, BCCatalogItem>>();
            this._deals = new Dictionary<int, Dictionary<int, CatalogDeal>>();
            this._predesignedItems = new Dictionary<int, PredesignedContent>();
        }

        public void Init(ItemDataManager ItemDataManager)
        {
            if (this._pages.Count > 0)
                this._pages.Clear();
            if (this._bcpages.Count > 0)
                this._bcpages.Clear();
            if (this._botPresets.Count > 0)
                this._botPresets.Clear();
            if (this._items.Count > 0)
                this._items.Clear();
            if (this._bcitems.Count > 0)
                this._bcitems.Clear();
            if (this._deals.Count > 0)
                this._deals.Clear();
            if (this._predesignedItems.Count > 0)
                this._predesignedItems.Clear();

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`item_id`,`catalog_name`,`cost_credits`,`cost_pixels`,`cost_diamonds`,`amount`,`page_id`,`limited_sells`,`limited_stack`,`offer_active`,`extradata`,`badge`,`offer_id`,`cost_honor`,`predesigned_id` FROM `catalog_items`");
                DataTable CatalogueItems = dbClient.getTable();

                if (CatalogueItems != null)
                {
                    foreach (DataRow Row in CatalogueItems.Rows)
                    {
                        if (Convert.ToInt32(Row["amount"]) <= 0) continue;

                        int ItemId = Convert.ToInt32(Row["id"]);
                        int PageId = Convert.ToInt32(Row["page_id"]);
                        int BaseId = Convert.ToInt32(Row["item_id"]);
                        int OfferId = Convert.ToInt32(Row["offer_id"]);
                        uint PredesignedId = Convert.ToUInt32(Row["predesigned_id"]);
                        if (BaseId == 0 && PredesignedId > 0)
                        {
                            var roomPack = _predesignedManager.predesignedRoom[PredesignedId];
                            if (roomPack == null) continue;
                            if (roomPack.CatalogItems.Contains(";"))
                            {
                                var cataItems = new Dictionary<int, int>();
                                var itemArray = roomPack.CatalogItems.Split(new char[] { ';' });
                                foreach (var item in itemArray)
                                {
                                    var items = item.Split(',');
                                    ItemData PredesignedData = null;
                                    if (!ItemDataManager.GetItem(Convert.ToInt32(items[0]), out PredesignedData))
                                    {
                                        log.Error("Catalog Bundle " + ItemId + " has no furniture data.");
                                        continue;
                                    }

                                    cataItems.Add(Convert.ToInt32(items[0]), Convert.ToInt32(items[1]));
                                }

                                this._predesignedItems[PageId] = new PredesignedContent(ItemId, cataItems);
                            }
                        }

                        ItemData Data = null;
                        if (PredesignedId <= 0)
                            if (!ItemDataManager.GetItem(BaseId, out Data))
                            {
                                log.Error("Couldn't load Catalog Item " + ItemId + ", no furniture record found. {3}");
                                continue;
                            }

                        if (!this._items.ContainsKey(PageId))
                            this._items[PageId] = new Dictionary<int, CatalogItem>();

                        if (OfferId != -1 && !this._itemOffers.ContainsKey(OfferId))
                            this._itemOffers.Add(OfferId, PageId);

                        this._items[PageId].Add(Convert.ToInt32(Row["id"]), new CatalogItem(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["item_id"]),
                            Data, Convert.ToString(Row["catalog_name"]), Convert.ToInt32(Row["page_id"]), Convert.ToInt32(Row["cost_credits"]), Convert.ToInt32(Row["cost_pixels"]), Convert.ToInt32(Row["cost_diamonds"]),
                            Convert.ToInt32(Row["amount"]), Convert.ToInt32(Row["limited_sells"]), Convert.ToInt32(Row["limited_stack"]), MoonEnvironment.EnumToBool(Row["offer_active"].ToString()),
                            Convert.ToString(Row["extradata"]), Convert.ToString(Row["badge"]), Convert.ToInt32(Row["offer_id"]), Convert.ToInt32(Row["cost_honor"]),
                            Convert.ToInt32(Row["predesigned_id"])));
                    }
                }


                dbClient.SetQuery("SELECT `id`,`item_id`,`catalog_name`,`cost_credits`,`cost_pixels`,`cost_diamonds`,`amount`,`page_id`,`limited_sells`,`limited_stack`,`offer_active`,`extradata`,`badge`,`offer_id`,`cost_honor`,`predesigned_id` FROM `catalog_bc_items`");
                DataTable BCCatalogueItems = dbClient.getTable();

                if (BCCatalogueItems != null)
                {
                    foreach (DataRow Row in BCCatalogueItems.Rows)
                    {
                        if (Convert.ToInt32(Row["amount"]) <= 0) continue;

                        int ItemId = Convert.ToInt32(Row["id"]);
                        int PageId = Convert.ToInt32(Row["page_id"]);
                        int BaseId = Convert.ToInt32(Row["item_id"]);
                        int OfferId = Convert.ToInt32(Row["offer_id"]);
                        uint PredesignedId = Convert.ToUInt32(Row["predesigned_id"]);
                        if (BaseId == 0 && PredesignedId > 0)
                        {
                            var roomPack = _predesignedManager.predesignedRoom[PredesignedId];
                            if (roomPack == null) continue;
                            if (roomPack.CatalogItems.Contains(";"))
                            {
                                var cataItems = new Dictionary<int, int>();
                                var itemArray = roomPack.CatalogItems.Split(new char[] { ';' });
                                foreach (var item in itemArray)
                                {
                                    var items = item.Split(',');
                                    ItemData PredesignedData = null;
                                    if (!ItemDataManager.GetItem(Convert.ToInt32(items[0]), out PredesignedData))
                                    {
                                        log.Error("Couldn't load Catalog BC Item " + ItemId + ", no furniture record found. {2}");
                                        continue;
                                    }

                                    cataItems.Add(Convert.ToInt32(items[0]), Convert.ToInt32(items[1]));
                                }

                                this._predesignedItems[PageId] = new PredesignedContent(ItemId, cataItems);
                            }
                        }

                        ItemData Data = null;
                        if (PredesignedId <= 0)
                            if (!ItemDataManager.GetItem(BaseId, out Data))
                            {
                                log.Error("Couldn't load BC Item " + ItemId + ", no furniture record found. {1}");
                                continue;
                            }

                        if (!this._bcitems.ContainsKey(PageId))
                            this._bcitems[PageId] = new Dictionary<int, BCCatalogItem>();

                        if (OfferId != -1 && !this._itemOffers.ContainsKey(OfferId))
                            this._itemOffers.Add(OfferId, PageId);

                        this._bcitems[PageId].Add(Convert.ToInt32(Row["id"]), new BCCatalogItem(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["item_id"]),
                            Data, Convert.ToString(Row["catalog_name"]), Convert.ToInt32(Row["page_id"]), Convert.ToInt32(Row["cost_credits"]), Convert.ToInt32(Row["cost_pixels"]), Convert.ToInt32(Row["cost_diamonds"]),
                            Convert.ToInt32(Row["amount"]), Convert.ToInt32(Row["limited_sells"]), Convert.ToInt32(Row["limited_stack"]), MoonEnvironment.EnumToBool(Row["offer_active"].ToString()),
                            Convert.ToString(Row["extradata"]), Convert.ToString(Row["badge"]), Convert.ToInt32(Row["offer_id"]), Convert.ToInt32(Row["cost_honor"]),
                            Convert.ToInt32(Row["predesigned_id"])));
                    }
                }


                dbClient.SetQuery("SELECT * FROM `catalog_deals`");
                DataTable GetDeals = dbClient.getTable();

                if (GetDeals != null)
                {
                    foreach (DataRow Row in GetDeals.Rows)
                    {
                        int Id = Convert.ToInt32(Row["id"]);
                        int PageId = Convert.ToInt32(Row["page_id"]);
                        string Items = Convert.ToString(Row["items"]);
                        string Name = Convert.ToString(Row["name"]);
                        int Credits = Convert.ToInt32(Row["cost_credits"]);
                        int Pixels = Convert.ToInt32(Row["cost_pixels"]);

                        if (!this._deals.ContainsKey(PageId))
                            this._deals[PageId] = new Dictionary<int, CatalogDeal>();

                        CatalogDeal Deal = new CatalogDeal(Id, PageId, Items, Name, Credits, Pixels, ItemDataManager);
                        this._deals[PageId].Add(Deal.Id, Deal);
                    }
                }



                dbClient.SetQuery("SELECT `id`,`parent_id`,`caption`,`page_link`,`visible`,`enabled`,`min_rank`,`min_vip`,`icon_image`,`page_layout`,`page_strings_1`,`page_strings_2` FROM `catalog_pages` ORDER BY `order_num`");
                DataTable CatalogPages = dbClient.getTable();

                if (CatalogPages != null)
                {
                    foreach (DataRow Row in CatalogPages.Rows)
                    {
                        this._pages.Add(Convert.ToInt32(Row["id"]), new CatalogPage(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["parent_id"]), Row["enabled"].ToString(), Convert.ToString(Row["caption"]),
                            Convert.ToString(Row["page_link"]), Convert.ToInt32(Row["icon_image"]), Convert.ToInt32(Row["min_rank"]), Convert.ToInt32(Row["min_vip"]), Row["visible"].ToString(), Convert.ToString(Row["page_layout"]),
                            Convert.ToString(Row["page_strings_1"]), Convert.ToString(Row["page_strings_2"]),
                            this._items.ContainsKey(Convert.ToInt32(Row["id"])) ? this._items[Convert.ToInt32(Row["id"])] : new Dictionary<int, CatalogItem>(),
                            this._deals.ContainsKey(Convert.ToInt32(Row["id"])) ? this._deals[Convert.ToInt32(Row["id"])] : new Dictionary<int, CatalogDeal>(),
                            this._predesignedItems.ContainsKey(Convert.ToInt32(Row["id"])) ? this._predesignedItems[Convert.ToInt32(Row["id"])] : null,
                            ref this._itemOffers));
                    }
                }

                dbClient.SetQuery("SELECT `id`,`parent_id`,`caption`,`page_link`,`visible`,`enabled`,`min_rank`,`min_vip`,`icon_image`,`page_layout`,`page_strings_1`,`page_strings_2` FROM `catalog_bc_pages` ORDER BY `order_num`");
                DataTable BCCatalogPages = dbClient.getTable();

                if (BCCatalogPages != null)
                {
                    foreach (DataRow Row in BCCatalogPages.Rows)
                    {
                        this._bcpages.Add(Convert.ToInt32(Row["id"]), new BCCatalogPage(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["parent_id"]), Row["enabled"].ToString(), Convert.ToString(Row["caption"]),
                            Convert.ToString(Row["page_link"]), Convert.ToInt32(Row["icon_image"]), Convert.ToInt32(Row["min_rank"]), Convert.ToInt32(Row["min_vip"]), Row["visible"].ToString(), Convert.ToString(Row["page_layout"]),
                            Convert.ToString(Row["page_strings_1"]), Convert.ToString(Row["page_strings_2"]),
                            this._bcitems.ContainsKey(Convert.ToInt32(Row["id"])) ? this._bcitems[Convert.ToInt32(Row["id"])] : new Dictionary<int, BCCatalogItem>(),
                            this._deals.ContainsKey(Convert.ToInt32(Row["id"])) ? this._deals[Convert.ToInt32(Row["id"])] : new Dictionary<int, CatalogDeal>(),
                            this._predesignedItems.ContainsKey(Convert.ToInt32(Row["id"])) ? this._predesignedItems[Convert.ToInt32(Row["id"])] : null,
                            ref this._itemOffers));
                    }
                }

                dbClient.SetQuery("SELECT `id`,`name`,`figure`,`motto`,`gender`,`ai_type` FROM `catalog_bot_presets`");
                DataTable bots = dbClient.getTable();



                if (bots != null)
                {
                    foreach (DataRow Row in bots.Rows)
                    {
                        this._botPresets.Add(Convert.ToInt32(Row[0]), new CatalogBot(Convert.ToInt32(Row[0]), Convert.ToString(Row[1]), Convert.ToString(Row[2]), Convert.ToString(Row[3]), Convert.ToString(Row[4]), Convert.ToString(Row[5])));
                    }
                }

                this._petRaceManager.Init();
                this._clothingManager.Init();
            }

            log.Info(">> Catalog Manager -> Pronto!");
        }

        public bool TryGetBot(int ItemId, out CatalogBot Bot)
        {
            return this._botPresets.TryGetValue(ItemId, out Bot);
        }

        public Dictionary<int, int> ItemOffers
        {
            get { return this._itemOffers; }
        }

        public bool TryGetPage(int pageId, out CatalogPage page)
        {
            return this._pages.TryGetValue(pageId, out page);
        }

        public bool TryGetBCPage(int pageId, out BCCatalogPage page)
        {
            return this._bcpages.TryGetValue(pageId, out page);
        }

        public ICollection<CatalogPage> GetPages()
        {
            return this._pages.Values;
        }

        public ICollection<BCCatalogPage> GetBCPages()
        {
            return this._bcpages.Values;
        }

        public MarketplaceManager GetMarketplace()
        {
            return this._marketplace;
        }

        public PetRaceManager GetPetRaceManager()
        {
            return this._petRaceManager;
        }

        public CatalogPage TryGetPageByTemplate(string template)
        {
            return this._pages.Values.Where(current => current.Template == template).First();
        }

        public VoucherManager GetVoucherManager()
        {
            return this._voucherManager;
        }

        public ClothingManager GetClothingManager()
        {
            return this._clothingManager;
        }

        internal PredesignedRoomsManager GetPredesignedRooms()
        {
            return this._predesignedManager;
        }
    }
}