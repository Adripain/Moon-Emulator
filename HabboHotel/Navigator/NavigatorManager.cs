using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Moon.Database.Interfaces;

using Moon.HabboHotel.Navigator;

using log4net;

namespace Moon.HabboHotel.Navigator
{
    public sealed class NavigatorManager
    {
        private static readonly ILog log = LogManager.GetLogger("Moon.HabboHotel.Navigator.NavigatorManager");

        private readonly Dictionary<int, FeaturedRoom> _featuredRooms;
        private readonly Dictionary<int, FeaturedRoom2> _featuredRooms2;
        private readonly Dictionary<int, StaffPick> _staffPicks;
        private readonly Dictionary<int, LoteSalas> _loteSalas;


        private readonly Dictionary<int, TopLevelItem> _topLevelItems;
        private readonly Dictionary<int, SearchResultList> _searchResultLists;

        public NavigatorManager()
        {
            this._topLevelItems = new Dictionary<int, TopLevelItem>();
            this._searchResultLists = new Dictionary<int, SearchResultList>();

            //Does this need to be dynamic?
            this._topLevelItems.Add(1, new TopLevelItem(1, "official_view", "", ""));
            this._topLevelItems.Add(2, new TopLevelItem(2, "hotel_view", "", ""));
            this._topLevelItems.Add(3, new TopLevelItem(3, "roomads_view", "", ""));
            this._topLevelItems.Add(4, new TopLevelItem(4, "myworld_view", "", ""));

            this._featuredRooms = new Dictionary<int, FeaturedRoom>();
            this._featuredRooms2 = new Dictionary<int, FeaturedRoom2>();
            this._staffPicks = new Dictionary<int, StaffPick>();
            this._loteSalas = new Dictionary<int, LoteSalas>();

            this.Init();
        }

        public void Init()
        {
            if (this._searchResultLists.Count > 0)
                this._searchResultLists.Clear();

            if (this._featuredRooms.Count > 0)
                this._featuredRooms.Clear();

            DataTable Table = null;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `navigator_categories` ORDER BY `id` ASC");
                Table = dbClient.getTable();

                if (Table != null)
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        if (Convert.ToInt32(Row["enabled"]) == 1)
                        {
                            if (!this._searchResultLists.ContainsKey(Convert.ToInt32(Row["id"])))
                                this._searchResultLists.Add(Convert.ToInt32(Row["id"]), new SearchResultList(Convert.ToInt32(Row["id"]), Convert.ToString(Row["category"]), Convert.ToString(Row["category_identifier"]), Convert.ToString(Row["public_name"]), true, -1, Convert.ToInt32(Row["required_rank"]), NavigatorViewModeUtility.GetViewModeByString(Convert.ToString(Row["view_mode"])), Convert.ToString(Row["category_type"]), Convert.ToString(Row["search_allowance"]), Convert.ToInt32(Row["order_id"])));
                        }
                    }
                }

                dbClient.SetQuery("SELECT `room_id`,`image` FROM `navigator_publics`");
                DataTable GetPublics = dbClient.getTable();

                if (GetPublics != null)
                {
                    foreach (DataRow Row in GetPublics.Rows)
                    {
                        if (!this._featuredRooms.ContainsKey(Convert.ToInt32(Row["room_id"])))
                            this._featuredRooms.Add(Convert.ToInt32(Row["room_id"]), new FeaturedRoom(Convert.ToInt32(Row["room_id"]), Convert.ToString(Row["image"])));
                    }
                }

                dbClient.SetQuery("SELECT `room_id`,`image` FROM `navigator_juegos`");
                DataTable GetPublics2 = dbClient.getTable();

                if (GetPublics2 != null)
                {
                    foreach (DataRow Row in GetPublics2.Rows)
                    {
                        if (!this._featuredRooms2.ContainsKey(Convert.ToInt32(Row["room_id"])))
                            this._featuredRooms2.Add(Convert.ToInt32(Row["room_id"]), new FeaturedRoom2(Convert.ToInt32(Row["room_id"]), Convert.ToString(Row["image"])));
                    }
                }


                dbClient.SetQuery("SELECT `room_id`,`image` FROM `navigator_staff_picks`");
                DataTable GetPicks = dbClient.getTable();

                if (GetPicks != null)
                {
                    foreach (DataRow Row in GetPicks.Rows)
                    {
                        if (!this._staffPicks.ContainsKey(Convert.ToInt32(Row["room_id"])))
                            this._staffPicks.Add(Convert.ToInt32(Row["room_id"]), new StaffPick(Convert.ToInt32(Row["room_id"]), Convert.ToString(Row["image"])));
                    }
                }

                dbClient.SetQuery("SELECT `room_id`,`image` FROM `navigator_lote`");
                DataTable GetLotes = dbClient.getTable();

                if (GetLotes != null)
                {
                    foreach (DataRow Row in GetLotes.Rows)
                    {
                        if (!this._loteSalas.ContainsKey(Convert.ToInt32(Row["room_id"])))
                            this._loteSalas.Add(Convert.ToInt32(Row["room_id"]), new LoteSalas(Convert.ToInt32(Row["room_id"]), Convert.ToString(Row["image"])));
                    }
                }

            }

            log.Info(">> Navigator Manager -> Pronto!");
        }

        public List<SearchResultList> GetCategorysForSearch(string Category)
        {
            IEnumerable<SearchResultList> Categorys =
                (from Cat in this._searchResultLists
                 where Cat.Value.Category == Category
                 orderby Cat.Value.OrderId ascending
                 select Cat.Value);
            return Categorys.ToList();
        }

        public ICollection<SearchResultList> GetResultByIdentifier(string Category)
        {
            IEnumerable<SearchResultList> Categorys =
                (from Cat in this._searchResultLists
                 where Cat.Value.CategoryIdentifier == Category
                 orderby Cat.Value.OrderId ascending
                 select Cat.Value);
            return Categorys.ToList();
        }

        public ICollection<SearchResultList> GetFlatCategories()
        {
            IEnumerable<SearchResultList> Categorys =
                (from Cat in this._searchResultLists
                 where Cat.Value.CategoryType == NavigatorCategoryType.CATEGORY
                 orderby Cat.Value.OrderId ascending
                 select Cat.Value);
            return Categorys.ToList();
        }

        public ICollection<SearchResultList> GetEventCategories()
        {
            IEnumerable<SearchResultList> Categorys =
                (from Cat in this._searchResultLists
                 where Cat.Value.CategoryType == NavigatorCategoryType.PROMOTION_CATEGORY
                 orderby Cat.Value.OrderId ascending
                 select Cat.Value);
            return Categorys.ToList();
        }

        public ICollection<TopLevelItem> GetTopLevelItems()
        {
            return this._topLevelItems.Values;
        }

        public ICollection<SearchResultList> GetSearchResultLists()
        {
            return this._searchResultLists.Values;
        }

        public bool TryGetTopLevelItem(int Id, out TopLevelItem TopLevelItem)
        {
            return this._topLevelItems.TryGetValue(Id, out TopLevelItem);
        }

        public bool TryGetSearchResultList(int Id, out SearchResultList SearchResultList)
        {
            return this._searchResultLists.TryGetValue(Id, out SearchResultList);
        }

        public bool TryGetFeaturedRoom(int RoomId, out FeaturedRoom PublicRoom)
        {
            return this._featuredRooms.TryGetValue(RoomId, out PublicRoom);
        }


        public bool TryGetFeatured2Room(int RoomId, out FeaturedRoom2 PublicRoom2)
        {
            return this._featuredRooms2.TryGetValue(RoomId, out PublicRoom2);
        }


        public bool TryGetStaffPickedRoom(int roomId, out StaffPick room)
        {
            return this._staffPicks.TryGetValue(roomId, out room);
        }

        public bool TryGetLoteRoom(int RoomIds, out LoteSalas LoteRoom)
        {
            return this._loteSalas.TryGetValue(RoomIds, out LoteRoom);
        }

        public bool TryAddStaffPickedRoom(int roomId)
        {
            if (this._staffPicks.ContainsKey(roomId))
                return false;

            this._staffPicks.Add(roomId, new StaffPick(roomId, ""));
            return true;
        }

        public bool TryRemoveStaffPickedRoom(int roomId)
        {
            if (!this._staffPicks.ContainsKey(roomId))
                return false;

            return this._staffPicks.Remove(roomId);
        }

        public ICollection<FeaturedRoom> GetFeaturedRooms()
        {
            return this._featuredRooms.Values;
        }

        public ICollection<FeaturedRoom2> GetFeaturedRooms2()
        {
            return this._featuredRooms2.Values;
        }


        public ICollection<StaffPick> GetStaffPicks()
        {
            return this._staffPicks.Values;
        }
        public ICollection<LoteSalas> GetLoteRooms()
        {
            return this._loteSalas.Values;
        }
    }
}
