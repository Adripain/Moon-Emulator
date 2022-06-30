namespace Moon.HabboHotel.Items.RentableSpaces
{
    public class RentableSpaceItem
    {
        private int _itemId;
        private int _ownerId;
        private string _ownerUsername;
        private int _expireStamp;
        private int _price;

        public RentableSpaceItem(int ItemId, int OwnerId, string OwnerUsername, int ExpireStamp, int Price)
        {
            this._itemId = ItemId;
            this._ownerId = OwnerId;
            this._ownerUsername = OwnerUsername;
            this._expireStamp = ExpireStamp;
            this._price = Price;
        }

        public bool IsRented()
        {
            return this._expireStamp > MoonEnvironment.GetUnixTimestamp();
        }

        public bool Rented
        {
            get { return this.IsRented(); }
        }

        public int ItemId
        {
            get { return this._itemId; }
            set { this._itemId = value; }
        }


        public int OwnerId
        {
            get { return this._ownerId; }
            set { this._ownerId = value; }
        }

        public string OwnerUsername
        {
            get { return this._ownerUsername; }
            set { this._ownerUsername = value; }
        }

        public int ExpireStamp
        {
            get { return this._expireStamp; }
            set { this._expireStamp = value; }
        }

        public int Price
        {
            get { return this._price; }
            set { this._price = value; }
        }

        public int GetExpireSeconds()
        {
            int i = this._expireStamp - (int)MoonEnvironment.GetUnixTimestamp();
            return i > 0 ? i : 0;
        }

    }
}