﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Permissions
{
    public class PermissionGroup
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Badge { get; set; }

        public PermissionGroup(string Name, string Description, string Badge)
        {
            this.Name = Name;
            this.Description = Description;
            this.Badge = Badge;
        }
    }
}
