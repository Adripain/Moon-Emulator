namespace Moon.HabboHotel.Catalog
{
    using log4net;
    using Moon;
    using Moon.Database.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class FrontpageManager
    {
        private Dictionary<int, Frontpage> _frontPageData = new Dictionary<int, Frontpage>();
        private Dictionary<int, Frontpage> _bcfrontPageData = new Dictionary<int, Frontpage>();
        private static readonly ILog log = LogManager.GetLogger("Moon.HabboHotel.Catalog.FrontPage");

        public FrontpageManager()
        {
            this.LoadFrontPage();
        }

        public ICollection<Frontpage> GetCatalogFrontPage()
        {
            return this._frontPageData.Values;
        }
        public ICollection<Frontpage> GetBCCatalogFrontPage()
        {
            return this._bcfrontPageData.Values;
        }
        public void LoadFrontPage()
        {
            if (this._frontPageData.Count > 0)
                this._frontPageData.Clear();
            if (this._bcfrontPageData.Count > 0)
                this._bcfrontPageData.Clear();


            using (IQueryAdapter adapter = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                adapter.SetQuery("SELECT * FROM `catalog_frontpage`");
                DataTable table = adapter.getTable();
                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        this._frontPageData.Add(Convert.ToInt32(row["id"]), new Frontpage(Convert.ToInt32(row["id"]), Convert.ToString(row["front_name"]), Convert.ToString(row["front_link"]), Convert.ToString(row["front_image"])));
                    }
                }
            }

            using (IQueryAdapter adapter = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                adapter.SetQuery("SELECT * FROM `catalog_bc_frontpage`");
                DataTable table = adapter.getTable();
                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        this._bcfrontPageData.Add(Convert.ToInt32(row["id"]), new Frontpage(Convert.ToInt32(row["id"]), Convert.ToString(row["front_name"]), Convert.ToString(row["front_link"]), Convert.ToString(row["front_image"])));
                    }
                }
            }
        }
    }
}

