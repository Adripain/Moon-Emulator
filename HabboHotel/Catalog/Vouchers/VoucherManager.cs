﻿using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Moon.Database.Interfaces;


namespace Moon.HabboHotel.Catalog.Vouchers
{
    public class VoucherManager
    {
        private Dictionary<string, Voucher> _vouchers;

        public VoucherManager()
        {
            this._vouchers = new Dictionary<string, Voucher>();
            this.Init();
        }

        public void Init()
        {
            if (this._vouchers.Count > 0)
                this._vouchers.Clear();

            DataTable GetVouchers = null;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `voucher`,`type`,`value`,`current_uses`,`max_uses` FROM `catalog_vouchers` WHERE `enabled` = '1'");
                GetVouchers = dbClient.getTable();
            }

            if (GetVouchers != null)
            {
                foreach (DataRow Row in GetVouchers.Rows)
                {
                    this._vouchers.Add(Convert.ToString(Row["voucher"]), new Voucher(Convert.ToString(Row["voucher"]), Convert.ToString(Row["type"]), Convert.ToInt32(Row["value"]), Convert.ToInt32(Row["current_uses"]), Convert.ToInt32(Row["max_uses"])));
                }
            }
        }

        public bool TryGetVoucher(string Code, out Voucher Voucher)
        {
            if (this._vouchers.TryGetValue(Code, out Voucher))
                return true;
            return false;
        }

        public void AddVoucher(string Code, string Type, int Value, int Uses)
        {
            this._vouchers.Add(Code, new Voucher(Code, Type, Value, 0, Uses));
        }
    }
}
