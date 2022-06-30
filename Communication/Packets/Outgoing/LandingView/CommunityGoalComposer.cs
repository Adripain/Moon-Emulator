using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.LandingView
{
    class CommunityGoalComposer : ServerPacket
    {
        public CommunityGoalComposer()
            : base(ServerPacketHeader.CommunityGoalComposer)
        {
            int VOTE_LONG = MoonEnvironment.GetGame().GetCommunityGoalVS().GetLeftVotes();
            int VOTE_SHORT = MoonEnvironment.GetGame().GetCommunityGoalVS().GetRightVotes(); 

            base.WriteBoolean(false); // Achieved?
            base.WriteInteger(0); // personalContributionScore (Si no es 0 no se muestran los botones)
            base.WriteInteger(0); // User Rank
            base.WriteInteger(1); // totalAmount Parts | En cuantas partes se divide la votación.
                                  //base.WriteInteger(getCommunityHighestAchievedLevel(VOTE_LONG, VOTE_SHORT)); // communityHighestAchievedLevel (nivel en el que está la flecha. De -3 a 3)
            base.WriteInteger(2);
            base.WriteInteger(VOTE_LONG > VOTE_SHORT ? -1 : 1); // scoreRemainingUntilNextLevel (puntuación restante hasta el siguiente nivel)
            base.WriteInteger(getPercentCompletionTowardsNextLevel(VOTE_LONG, VOTE_SHORT)); // percentCompletionTowardsNextLevel (porcentaje completado hasta el siguiente nivel)
            base.WriteString(MoonEnvironment.GetGame().GetCommunityGoalVS().GetName());
            base.WriteInteger(0); // countdown_widget | 0 = disable, > 0 = enable
            base.WriteInteger(0); // Número de premios disponibles.
            // base.WriteInteger(2); // Número de ganadores (ganador 1, 2, 3...)
        }

        public CommunityGoalComposer(bool Whats): base(ServerPacketHeader.CommunityGoalComposer)
        {
            base.WriteBoolean(true); //Achieved?
            base.WriteInteger(3); //User Amount
            base.WriteInteger(1); //User Rank
            base.WriteInteger(12); //Total Amount

            base.WriteInteger(3);

            base.WriteInteger(3); // Puntos que faltan para el siguiente nivel
            base.WriteInteger(30); // Porcentaje antes del siguiente nivel
            base.WriteString("avidPizzaEater");
            base.WriteInteger(-1); //Timer
            base.WriteInteger(1); //Rank Count
            base.WriteInteger(1); //Rank level
        }

        public CommunityGoalComposer(bool Whats, int Hola) : base(ServerPacketHeader.CommunityGoalComposer)
        {
            base.WriteBoolean(false); //Achieved?
            base.WriteInteger(-1); //User Amount
            base.WriteInteger(0); //User Rank
            base.WriteInteger(0); //Total Amount

            base.WriteInteger(0);

            base.WriteInteger(0); // Puntos que faltan para el siguiente nivel
            base.WriteInteger(0); // Porcentaje antes del siguiente nivel
            base.WriteString("tourdefrance17CommunityGoal");
            base.WriteInteger(10000); //Timer
            base.WriteInteger(0); //Rank Count
            base.WriteInteger(0); //Rank level
        }

        public CommunityGoalComposer(bool Whats, int Hola, string Pene) : base(ServerPacketHeader.CommunityGoalComposer)
        {
            base.WriteBoolean(false); //Achieved?
            base.WriteInteger(0); //User Amount
            base.WriteInteger(0); //User Rank
            base.WriteInteger(0); //Total Amount

            base.WriteInteger(0);

            base.WriteInteger(0); // Puntos que faltan para el siguiente nivel
            base.WriteInteger(0); // Porcentaje antes del siguiente nivel
            base.WriteString("tourdefrance17CommunityGoal");
            base.WriteInteger(10000); //Timer
            base.WriteInteger(0); //Rank Count
            base.WriteInteger(0); //Rank level
        }

        private int[] array1 = { -3, -2, -1, 0, 1, 2, 3 };
        private double[] array2 = { 0, 0, 4.75, 11.5, 16.25, 23, 23 };

        /// <summary>
        /// Obtiene un valor entre -3 y 3 para indicar la posición de la flecha en la votación.
        /// </summary>
        /// <param name="A">Votos Izquierda</param>
        /// <param name="B">Votos Derecha</param>
        /// <returns></returns>
        private int getCommunityHighestAchievedLevel(int A, int B)
        {
            int SUM_TOTAL = A + B;
            int POSITION = 0;
            double UNO = 1.0;
            double DOS = 2.0;
            double TRES = 3.0;
            double UN_TERCIO = UNO / TRES;
            double DOS_TERCIOS = DOS / TRES;

            if (A > B) // Movemos hacia la izquierda
            {
                int IZQ_PERCENT = ((A * 100) / SUM_TOTAL);

                if (IZQ_PERCENT == 100) POSITION = 0;
                else if (IZQ_PERCENT > (DOS_TERCIOS * 100)) POSITION = 1;
                else if (IZQ_PERCENT > (UN_TERCIO * 100)) POSITION = 2;
                else POSITION = 3;
            }
            else if(B > A) // Movemos hacia la derecha
            {
                int DER_PERCENT = ((B * 100) / SUM_TOTAL);

                if (DER_PERCENT == 100) POSITION = 6;
                else if (DER_PERCENT > (DOS_TERCIOS * 100)) POSITION = 5;
                else if (DER_PERCENT > (UN_TERCIO * 100)) POSITION = 4;
                else POSITION = 3;
            }
            else // Centro
            {
                POSITION = 3; // valor 0
            }

            return array1[POSITION];
        }

        private int getPercentCompletionTowardsNextLevel(int A, int B)
        {
            int SUM_TOTAL = A + B;
            double UNO = 1.0;
            double DOS = 2.0;
            double TRES = 3.0;
            double UN_TERCIO = UNO / TRES;
            double DOS_TERCIOS = DOS / TRES;

            if (A > B) // Movemos hacia la izquierda
            {
                int IZQ_PERCENT = ((A * 100) / SUM_TOTAL);

                if (IZQ_PERCENT == 100) return 100;
                else if (IZQ_PERCENT > ((2 / 3) * 100)) return (100 - IZQ_PERCENT) * 3;
                else if (IZQ_PERCENT > ((1 / 3) * 100)) return (int)(((DOS_TERCIOS * 100) - IZQ_PERCENT) * 3);
                else return (int)(((UN_TERCIO * 100) - IZQ_PERCENT) * 3);
            }
            else if (B > A) // Movemos hacia la derecha
            {
                int DER_PERCENT = ((B * 100) / SUM_TOTAL);

                if (DER_PERCENT == 100) return 100;
                else if (DER_PERCENT > ((2 / 3) * 100)) return (100 - DER_PERCENT) * 3;
                else if (DER_PERCENT > ((1 / 3) * 100)) return (int)(((DOS_TERCIOS * 100) - DER_PERCENT) * 3);
                else return (int)(((UN_TERCIO * 100) - DER_PERCENT) * 3);
            }

            return 0;
        }

        /*
             * En caso de activar el Rank Count funcionaría tal que:
             * Cada usuario podrá obtener un ranking
             * Más información en external_variables_texts: landing.view.competition.prizes.X
             * En este caso sólo nos interesa el String
             * 23 Needle (flechas)
             * private static const _SafeStr_10823:Array = [-3, -2, -1, 0, 1, 2, 3];
             * private static const _SafeStr_10824:Array = [0, 0, 4.75, 11.5, 16.25, 23, 23];
             * 
             * function getCurrentNeedleFrame() {
                   if (communityHighestAchievedLevel <= -3) {
	                   return 0;
                   }
                
                   if(communityHighestAchievedLevel >= 3) {
	                   return 23;
                   }

                   int k = (scoreRemainingUntilNextLevel < 0) ? -1 : 1;
                   int local_2 = communityHighestAchievedLevel;
                   int local_3 = array2[posición(local_2)];
                   int local_4 = Math.abs(array2[posición(local_2 + k)] - array2[posición(local_2)]);
                   return Math.round(local_3 + (((percentCompletionTowardsNextLevel / 100) * local_4) * k);
               }
         */
    }
}
