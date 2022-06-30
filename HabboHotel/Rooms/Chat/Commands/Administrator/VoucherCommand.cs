using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moon.Database.Interfaces;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class VoucherCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_16"; }
        }

        public string Parameters
        {
            get { return "[MENSAGEM]"; }
        }

        public string Description
        {
            get { return "Envia uma mensagem a todos os Staffs online."; }
        }

        public void Execute(GameClient Session, Rooms.Room Room, string[] Params)
        {
            #region Parametros
            string type = Params[1];
            int value = int.Parse(Params[2]);
            int uses = int.Parse(Params[3]);
            #endregion

            int Voucher = 10;
            string _CaracteresPermitidos = "abcdefghijklmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789!@$?";
            Byte[] randomBytes = new Byte[Voucher];
            char[] Caracter = new char[Voucher];
            int CuentaPermitida = _CaracteresPermitidos.Length;

            for (int i = 0; i < Voucher; i++)
            {
                Random randomObj = new Random();
                randomObj.NextBytes(randomBytes);
                Caracter[i] = _CaracteresPermitidos[(int)randomBytes[i] % CuentaPermitida];
            }

            var code = new string(Caracter);

            MoonEnvironment.GetGame().GetCatalog().GetVoucherManager().AddVoucher(code, type, value, uses);

            MoonEnvironment.GetGame().GetClientManager().SendMessage(new RoomCustomizedAlertComposer("AVISO: Um novo voucher foi divulgado, para pega-lo, vá ao catálogo, na aba 'Inicio' na parte inferior, no quadrado, insira: \n\n" +
                "Código: " + code + "\nA recompensa é: " + type + "\n Pode ser usado até mesmo em " + uses + " ocasiones\n\n Sorte, resgatá-lo!"));

        }
    }
}
