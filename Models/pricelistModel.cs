using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using NavWebApi.Utils;

namespace NavWebApi.Models
{
    public static class PricelistModel
    {
        public static Plist GetTobaccoPriceList()
        {

            SqlConnection conn = new SqlConnection(DataBaseConnection.connectionString2013);
            conn.Open();

            SqlDataAdapter sda = new SqlDataAdapter(SQLQueryText.QueryPriceForTablet , conn);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            DataTable dt = ds.Tables[0];
            sda.Dispose();
            List<Item> items = new List<Item>();
            foreach (DataRow dr in dt.Rows)
            {
                Item m = new Item
                {
                    //No = dr["itemno"].ToString(),
                    Nameeng = dr["nameeng"].ToString(),
                    Name = dr["name"].ToString(),
                    Price = dr["price"].ToString(),
                    Price2 = dr["price2"].ToString()
                    //pricelist = new Dictionary<string, decimal>() {
                    //{"55", (decimal) dr["55"]},{"57", (decimal) dr["57"]}, {"81", (decimal) dr["81"]}}
                };
                items.Add(m);
            }
            conn.Close();

            return new Plist() { pricelist=items};
        }
    }
}