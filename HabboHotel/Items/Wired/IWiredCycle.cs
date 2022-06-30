using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Items.Wired
{
    interface IWiredCycle
    {
        int Delay { get; set; }
        int TickCount { get; set; }
        bool OnCycle();
    }
}
