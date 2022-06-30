﻿using System;

namespace Moon.HabboHotel.Rooms
{
    public class RoomPromotion
    {

        private string _name;
        private string _description;
        private double _timestampExpires;
        private double _timestampStarted;
        private int _categoryId;

        public RoomPromotion(string Name, string Desc, int CategoryId)
        {
            this._name = Name;
            this._description = Desc;
            this._timestampStarted = MoonEnvironment.GetUnixTimestamp();
            this._timestampExpires = (MoonEnvironment.GetUnixTimestamp()) + (MoonStaticGameSettings.RoomPromotionLifeTime * 60);
            this._categoryId = CategoryId;
        }

        public RoomPromotion(string Name, string Desc, double Started, double Expires, int CategoryId)
        {
            this._name = Name;
            this._description = Desc;
            this._timestampStarted = Started;
            this._timestampExpires = Expires;
            this._categoryId = CategoryId;
        }

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public string Description
        {
            get { return this._description; }
            set { this._description = value; }
        }
        public double TimestampStarted
        {
            get { return this._timestampStarted; }
        }

        public double TimestampExpires
        {
            get { return this._timestampExpires; }
            set { this._timestampExpires = value; }
        }

        public bool HasExpired
        {
            get { return (this.TimestampExpires - MoonEnvironment.GetUnixTimestamp()) < 0; }
        }

        public int MinutesLeft
        {
            get { return Convert.ToInt32(Math.Ceiling((this.TimestampExpires - MoonEnvironment.GetUnixTimestamp()) / 60)); }
        }

        public int CategoryId
        {
            get { return this._categoryId; }
            set { this._categoryId = value; }
        }
    }
}