﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Support
{    /// <summary>
     /// TODO: Utilize ModerationTicket.cs
     /// </summary>
    public enum TicketStatus
    {
        OPEN = 0,
        PICKED = 1,
        RESOLVED = 2,
        ABUSIVE = 3,
        INVALID = 4,
        DELETED = 5
    }
}
