using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Catalog
{
    class GetClubOffersMessageComposer : ServerPacket
    {
        public GetClubOffersMessageComposer()
            : base(ServerPacketHeader.GetClubOffersMessageComposer)
        {
            int credits = 1;
            int diamonds = 3;

            base.WriteInteger(0); // Dont know
            base.WriteString("asd"); // Dont know
            base.WriteBoolean(true); // Dont know
            base.WriteInteger(75); // Resultado créditos  credits
            base.WriteInteger(5); // Resultado extra    diamonds
            base.WriteInteger(-1); // Dont know
            base.WriteBoolean(true); // Alargar o Prolongar
            base.WriteInteger(1); // Precio multiplicado
            base.WriteInteger(1); // Dont know
            base.WriteBoolean(true); // Activar moneda extra

            base.WriteInteger(0); // Dont know
            base.WriteInteger(0); // Dont know
            base.WriteInteger(0); // Dont know
            base.WriteInteger(0); // Dont know
            base.WriteInteger(80); // Créditos
            base.WriteInteger(5); // Extra
            base.WriteInteger(105); // Tipo de Moneda
            base.WriteInteger(1); // Dias disponible

        }
    }
}
