﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Moon.HabboHotel.Badges
{
    public class BadgeDefinition
    {
        private string _code;
        private string _requiredRight;

        public BadgeDefinition(string Code, string RequiredRight)
        {
            this._code = Code;
            this._requiredRight = RequiredRight;
        }

        public string Code
        {
            get { return this._code; }
            set { this._code = value; }
        }

        public string RequiredRight
        {
            get { return this._requiredRight; }
            set { this._requiredRight = value; }
        }
    }
}