using Moon.HabboHotel.Calendar;
using System;
using System.Collections.Generic;

namespace Moon.Communication.Packets.Outgoing.Campaigns
{
    class CampaignCalendarDataComposer : ServerPacket
    {
        public CampaignCalendarDataComposer(bool[] OpenedBoxes)
            : base(ServerPacketHeader.CampaignCalendarDataMessageComposer)
        {
            base.WriteString(MoonEnvironment.GetGame().GetCalendarManager().GetCampaignName()); // NOMBRE DE LA CAMPAÑA.
            base.WriteString("asd"); // NO TIENE FUNCIÓN EN LA SWF.
            base.WriteInteger(MoonEnvironment.GetGame().GetCalendarManager().GetUnlockDays()); // DÍAS ACTUAL (DESBLOQUEADOS).
            base.WriteInteger(MoonEnvironment.GetGame().GetCalendarManager().GetTotalDays()); // DÍAS TOTALES.
            int OpenedCount = 0;
            int LateCount = 0;

            for (int i = 0; i < OpenedBoxes.Length; i++)
            {
                if (OpenedBoxes[i])
                {
                    OpenedCount++;
                }
                else
                {
                    // DÍA ACTUAL (EVITAMOS)
                    if (MoonEnvironment.GetGame().GetCalendarManager().GetUnlockDays() == i)
                        continue;

                    LateCount++;
                }
            }
            // CAJAS ABIERTAS HASTA EL MOMENTO.
            base.WriteInteger(OpenedCount);
            for (int i = 0; i < OpenedBoxes.Length; i++)
            {
                if (OpenedBoxes[i])
                {
                    base.WriteInteger(i);
                }
            }

            // CAJAS QUE SE HAN PASADO DE FECHA.
            base.WriteInteger(LateCount);
            for (int i = 0; i < OpenedBoxes.Length; i++)
            {
                // DÍA ACTUAL (EVITAMOS)
                if (MoonEnvironment.GetGame().GetCalendarManager().GetUnlockDays() == i)
                    continue;

                if (!OpenedBoxes[i])
                    base.WriteInteger(i);
            }
        }
    }
}