using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Catalog
{
    public class WiredRewardAlertComposer : ServerPacket
    {
        public WiredRewardAlertComposer(int codeMsg)
            : base(ServerPacketHeader.WiredRewardAlertComposer)
        {
            base.WriteInteger(codeMsg);
        }
    }
}

/*
 *     6 = Premio recibido 
 *     7 = Placa recibida
 *     0=Lo sentimos. Los premios disponibles son limitados y ya han sido todos repartidos.
 *     1=Ya has obtenido este premio. Cada usuari@ sólo puede ganar el mismo premio una vez.
 *     2=Hoy ya has sido premiad@. Prueba de nuevo mañana.
 *     3=Ya has sido premiad@ durante la última hora. Prueba de nuevo dentro de una hora.
 *     4=No ha habido suerte esta vez. Prueba de nuevo para hacerte con el premio.
 *     5=Ya has reunido todos los premios que podias obtener.
 *     8=Acaban de darte una recompensa. ¡Inténtalo de nuevo en un minuto!
 */
