namespace Moon.HabboHotel.Catalog
{
    using System;

    public class Frontpage
    {
        public string _frontImage;
        public string _frontLink;
        public string _frontName;
        public int _id;

        public Frontpage(int Id, string FrontName, string FrontLink, string FrontImage)
        {
            this._id = Id;
            this._frontName = FrontName;
            this._frontLink = FrontLink;
            this._frontImage = FrontImage;
        }

        public string FrontImage()
        {
            return this._frontImage;
        }

        public string FrontLink()
        {
            return this._frontLink;
        }

        public string FrontName()
        {
            return this._frontName;
        }

        public int Id()
        {
            return this._id;
        }
    }
}

