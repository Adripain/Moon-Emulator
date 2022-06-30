﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Items.Televisions
{
    public class TelevisionItem
    {
        private int _id;
        private string _youtubeId;
        private string _title;
        private string _description;
        private Boolean _enabled;

        public TelevisionItem(int Id, string YouTubeId, string Title, string Description, Boolean Enabled)
        {
            this._id = Id;
            this._youtubeId = YouTubeId;
            this._title = Title;
            this._description = Description;
            this._enabled = Enabled;
        }

        public int Id
        {
            get
            {
                return this._id;
            }
        }

        public string YouTubeId
        {
            get
            {
                return this._youtubeId;
            }
        }


        public string Title
        {
            get
            {
                return this._title;
            }
        }

        public string Description
        {
            get
            {
                return this._description;
            }
        }

        public bool Enabled
        {
            get
            {
                return this._enabled;
            }
        }
    }
}
