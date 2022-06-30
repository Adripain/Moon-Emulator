using log4net;
using Moon.Database.Interfaces;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;
using Moon.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces;

namespace Moon.HabboHotel.Items.RentableSpaces
{
    public class RentableSpaceManager
    {
        private static readonly ILog log = LogManager.GetLogger("Moon.HabboHotel.Items.RentableSpaces");

        private Dictionary<int, RentableSpaceItem> _items;

        public RentableSpaceManager()
        {
            this.Init();
        }

        public void Init()
        {
            this._items = new Dictionary<int, RentableSpaceItem>();

            using (IQueryAdapter con = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                con.SetQuery("SELECT * FROM `items_rentablespace`");
                DataTable table = con.getTable();
                if (table != null)
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow row = table.Rows[i];
                        if (row != null)
                        {
                            int id = Convert.ToInt32(row["item_id"].ToString());
                            int ownerid = Convert.ToInt32(row["owner"].ToString());
                            string ownername = "";
                            if (ownerid > 0)
                            {
                                Habbo owner = MoonEnvironment.GetHabboById(ownerid);
                                if (owner != null)
                                    ownername = owner.Username;
                            }
                            int expirestamp = Convert.ToInt32(row["expire"].ToString());
                            int price = Convert.ToInt32(row["price"].ToString());
                            this.AddItem(new RentableSpaceItem(id, ownerid, ownername, expirestamp, price));
                        }
                    }
                }
            }
        }

        public bool ConfirmCancel(GameClient Session, RentableSpaceItem RentableSpace)
        {
            if (Session == null)
                return false;
            if (Session.GetHabbo() == null)
                return false;
            if (RentableSpace == null)
                return false;
            if (!RentableSpace.IsRented())
                return false;
            if (RentableSpace.OwnerId != Session.GetHabbo().Id)
                return false;

            RentableSpace.OwnerId = 0;
            RentableSpace.OwnerUsername = "";
            RentableSpace.ExpireStamp = 0;

            using (IQueryAdapter con = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                con.SetQuery("UPDATE `items_rentablespace` SET owner = @owner, ownerusername = @username, expire = @expire WHERE item_id = @itemid LIMIT 1");
                con.AddParameter("itemid", RentableSpace.ItemId);
                con.AddParameter("owner", 0);
                con.AddParameter("username", "");
                con.AddParameter("expire", 0);
                con.RunQuery();
            }

            return true;
        }

        public bool ConfirmBuy(GameClient Session, RentableSpaceItem RentableSpace, int ExpireSeconds)
        {

            if (Session == null)
                return false;
            if (Session.GetHabbo() == null)
                return false;
            if (RentableSpace == null)
                return false;
            if (Session.GetHabbo().Credits < RentableSpace.Price)
                return false;
            if (ExpireSeconds < 1)
                return false;

            Session.GetHabbo().Credits -= RentableSpace.Price;
            RentableSpace.OwnerId = Session.GetHabbo().Id;
            RentableSpace.OwnerUsername = Session.GetHabbo().Username;
            RentableSpace.ExpireStamp = (int)MoonEnvironment.GetUnixTimestamp() + ExpireSeconds;
            Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
            Session.SendMessage(new RentableSpaceComposer(RentableSpace.OwnerId, RentableSpace.OwnerUsername, RentableSpace.GetExpireSeconds()));
            using (IQueryAdapter con = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                con.SetQuery("UPDATE `items_rentablespace` SET owner = @owner, ownerusername = @username, expire = @expire WHERE item_id = @itemid LIMIT 1");
                con.AddParameter("itemid", RentableSpace.ItemId);
                con.AddParameter("owner", Session.GetHabbo().Id);
                con.AddParameter("username", Session.GetHabbo().Username);
                con.AddParameter("expire", MoonEnvironment.GetUnixTimestamp() + 604800);
                con.RunQuery();
            }
            return true;

        }

        public int GetBuyErrorCode(GameClient Session, RentableSpaceItem RentableSpace)
        {
            if (Session == null || Session.GetHabbo() == null)
                return 400;
            if (RentableSpace.Rented)
                return 100;
            if (Session.GetHabbo().Credits < RentableSpace.Price)
                return 200;
            return 0;
        }

        public int GetCancelErrorCode(GameClient Session, RentableSpaceItem RentableSpace)
        {
            if (Session == null || Session.GetHabbo() == null)
                return 400;
            if (!RentableSpace.IsRented())
                return 101;
            if (RentableSpace.OwnerId != Session.GetHabbo().Id)
                return 102;
            return 0;
        }

        public int GetButtonErrorCode(GameClient Session, RentableSpaceItem RentableSpace)
        {

            if (Session == null)
                return 400;
            if (Session.GetHabbo() == null)
                return 400;
            if (RentableSpace.Rented)
                return 100;
            if (Session.GetHabbo().Credits < RentableSpace.Price)
                return 201;
            return 0;
        }

        public RentableSpaceItem[] GetArray()
        {
            return this._items.Values.ToArray();
        }

        public RentableSpaceItem CreateAndAddItem(int ItemId, GameClient Session)
        {
            RentableSpaceItem i = this.CreateItem(ItemId);
            this.AddItem(i);
            using (IQueryAdapter con = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                con.SetQuery("INSERT INTO `items_rentablespace` (item_id, owner, expire, price) VAlUES (@id, @ownerid, @expire, @price)");
                con.AddParameter("id", i.ItemId);
                con.AddParameter("ownerid", Session.GetHabbo().Id);
                con.AddParameter("expire", MoonEnvironment.GetUnixTimestamp() + 604800);
                con.AddParameter("price", i.Price);
                con.RunQuery();
            }
            return i;
        }

        public RentableSpaceItem CreateItem(int ItemId)
        {
            return new RentableSpaceItem(ItemId, 0, "", 0, 100);
        }

        public void AddItem(RentableSpaceItem SpaceItem)
        {
            if (this._items.ContainsKey(SpaceItem.ItemId))
                this._items.Remove(SpaceItem.ItemId);
            this._items.Add(SpaceItem.ItemId, SpaceItem);
        }

        public bool GetRentableSpaceItem(int Id, out RentableSpaceItem rsitem)
        {
            return _items.TryGetValue(Id, out rsitem);
        }


    }
}