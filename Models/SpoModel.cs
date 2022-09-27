using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using NavWebApi.Utils;

namespace NavWebApi.Models
{
    public class SpoModel
    {
        //public Slist GetSpo_2(string barcode_string)
        //{
        //    List<Spo> tmpListSpo = new List<Spo>();
        //    using (SqlConnection conn_2013 = new SqlConnection(DataBaseConnection.connectionString2013))
        //    {
        //        SqlCommand cmd = new SqlCommand(SQLQueryText.QueryItemByPLU_Barcode, conn_2013);
        //        cmd.Parameters.Add(new SqlParameter
        //        {
        //            ParameterName = "@search_string",
        //            Value = barcode_string,
        //            SqlDbType = SqlDbType.NVarChar
        //        });
        //        conn_2013.Open();
        //        using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
        //        {
        //            if (!rdr.HasRows)
        //            {
        //                Spo s = new Spo
        //                {
        //                    Barcode = barcode_string,
        //                    Art = "---",
        //                    Art_desc = "не найдено",
        //                    Price_wo_discount = "---",
        //                    Price_with_discount = "---",
        //                    Disc_no = "---",
        //                    Disc_desc = "---"
        //                };
        //                tmpListSpo.Add(s);
        //            } else 
        //            { 

        //            }
        //        }

        //        return new Slist();
        //    }
        //}




        public static Slist GetSPO(string barcode_string)
        {
            try
            {


                string ArticlePLU = "";
                string ArticleDescription = "";
                List<Spo> tmpListSpo = new List<Spo>();
                SqlConnection conn_2013 = new SqlConnection(DataBaseConnection.connectionString2013);
                conn_2013.Open();
                SqlDataAdapter sda_2013 = new SqlDataAdapter(SQLQueryText.QueryItemByPLU_Barcode, conn_2013);
                sda_2013.SelectCommand.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@search_string",
                    Value = barcode_string,
                    SqlDbType = SqlDbType.NVarChar
                });

                DataSet ds_2013 = new DataSet();
                sda_2013.Fill(ds_2013);
                DataTable dt_2013 = ds_2013.Tables[0];
                sda_2013.Dispose();
                conn_2013.Close();
                if (dt_2013.Rows.Count == 0)
                {
                    //ArticlePLU = "---";
                    //ArticleDescription = "не найдено";

                    Spo s = new Spo
                    {
                        Barcode = barcode_string,
                        Art = "---",
                        Art_desc = "не найдено",
                        Price_wo_discount = "---",
                        Price_with_discount = "---",
                        Disc_percent = "---",
                        Disc_no = "---",
                        Disc_desc = "---"
                    };
                    tmpListSpo.Add(s);
                    //conn_2013.Close();
                    
                }
                else
                {
                    ArticlePLU = dt_2013.Rows[0]["art"].ToString();
                    ArticleDescription = dt_2013.Rows[0]["art_desc"].ToString();
                    SqlConnection conn = new SqlConnection(DataBaseConnection.connectionStringIDF);
                    conn.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(SQLQueryText.QuerySPObyBarocode, conn);
                    sda.SelectCommand.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@barcode",
                        Value = barcode_string,
                        SqlDbType = SqlDbType.NVarChar
                    });

                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    DataTable dt = ds.Tables[0];
                    sda.Dispose();
                    conn.Close();
                    if (dt.Rows.Count != 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            Spo s = new Spo
                            {
                                Barcode = dr["barcode"].ToString(),
                                Art = dr["art"].ToString(),
                                Art_desc = dr["art_desc"].ToString(),
                                Price_wo_discount = Convert.ToDecimal(dr["price_wo_discount"]).ToString("0.00"),
                                Price_with_discount = Convert.ToDecimal(dr["price_with_discount"]).ToString("0.00"),
                                Disc_percent = Convert.ToDecimal(dr["disc_percent"]).ToString("0.00"),
                                Disc_no = dr["disc_no"].ToString(),
                                Disc_desc = dr["disc_desc"].ToString()
                            };
                            tmpListSpo.Add(s);
                            //conn.Close();
                        }
                    }
                    else
                    {
                        /************************************************************************************/
                        SqlConnection connPrice = new SqlConnection(DataBaseConnection.connectionString2013);
                        connPrice.Open();
                        SqlDataAdapter sdaPrice = new SqlDataAdapter(SQLQueryText.QueryCurrentPriceByPLU, connPrice);
                        sdaPrice.SelectCommand.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@art",
                            Value = ArticlePLU,
                            SqlDbType = SqlDbType.NVarChar
                        });

                        DataSet dsPrice = new DataSet();
                        sdaPrice.Fill(dsPrice);
                        DataTable dtPrice = dsPrice.Tables[0];
                        sda.Dispose();
                        connPrice.Close();
                        if (dtPrice.Rows.Count != 0)
                        {
                            Spo s = new Spo
                            {
                                Barcode = barcode_string,
                                Art = ArticlePLU,
                                Art_desc = ArticleDescription,
                                Price_wo_discount = dtPrice.Rows[0]["p81"].ToString(),
                                Price_with_discount = "---",
                                Disc_percent = "---",
                                Disc_no = "---",
                                Disc_desc = "---"
                            };
                            tmpListSpo.Add(s);
                            //connPrice.Close();
                            //System.Diagnostics.Debug.WriteLine("стр 164 "+ barcode_string);
                        }
                        else
                        {
                            //Нет цены для 81 режима
                            Spo s = new Spo
                            {
                                Barcode = barcode_string,
                                Art = ArticlePLU,
                                Art_desc = ArticleDescription,
                                Price_wo_discount = "нет цены DP",
                                Price_with_discount = "---",
                                Disc_percent = "---",
                                Disc_no = "---",
                                Disc_desc = "---"
                            };
                            tmpListSpo.Add(s);
                            //System.Diagnostics.Debug.WriteLine("стр 180 " + barcode_string);
                        }
                    }
                } 
                return new Slist() { SpoList = tmpListSpo };

            }
            catch (InvalidOperationException e)
            {
                System.Diagnostics.Debug.WriteLine("это конец "+e.Message);
                return null;
            }

          

        }

        public static Slist GetSPO2(string barcode_string)
        {
            try
            {


                string ArticlePLU = "";
                string ArticleDescription = "";
                List<Spo> tmpListSpo = new List<Spo>();
                SqlConnection conn_2013 = new SqlConnection(DataBaseConnection.connectionString2013);
                conn_2013.Open();
                SqlDataAdapter sda_2013 = new SqlDataAdapter(SQLQueryText.QueryItemByPLU_Barcode, conn_2013);
                sda_2013.SelectCommand.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@search_string",
                    Value = barcode_string,
                    SqlDbType = SqlDbType.NVarChar
                });

                DataSet ds_2013 = new DataSet();
                sda_2013.Fill(ds_2013);
                DataTable dt_2013 = ds_2013.Tables[0];
                sda_2013.Dispose();
                conn_2013.Close();
                if (dt_2013.Rows.Count == 0)
                {
                    //ArticlePLU = "---";
                    //ArticleDescription = "не найдено";

                    Spo s = new Spo
                    {
                        Barcode = barcode_string,
                        Art = "---",
                        Art_desc = "не найдено",
                        Price_wo_discount = "---",
                        Price_with_discount = "---",
                        Disc_percent = "---",
                        Disc_no = "---",
                        Disc_desc = "---"
                    };
                    tmpListSpo.Add(s);
                    //conn_2013.Close();

                }
                else
                {
                    ArticlePLU = dt_2013.Rows[0]["art"].ToString();
                    ArticleDescription = dt_2013.Rows[0]["art_desc"].ToString();
                    SqlConnection conn = new SqlConnection(DataBaseConnection.connectionStringIDF);
                    conn.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(SQLQueryText.QuerySPObyBarocode2, conn);
                    sda.SelectCommand.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@barcode",
                        Value = barcode_string,
                        SqlDbType = SqlDbType.NVarChar
                    });

                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    DataTable dt = ds.Tables[0];
                    sda.Dispose();
                    conn.Close();
                    if (dt.Rows.Count != 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            Spo s = new Spo
                            {
                                Barcode = dr["barcode"].ToString(),
                                Art = dr["art"].ToString(),
                                Art_desc = dr["art_desc"].ToString(),
                                Price_wo_discount = Convert.ToDecimal(dr["price_wo_discount"]).ToString("0.00"),
                                Price_with_discount = Convert.ToDecimal(dr["price_with_discount"]).ToString("0.00"),
                                Disc_percent=Convert.ToDecimal(dr["disc_percent"]).ToString("0.00"),
                                Disc_no = dr["disc_no"].ToString(),
                                Disc_desc = dr["disc_desc"].ToString()
                            };
                            tmpListSpo.Add(s);
                            //conn.Close();
                        }
                    }
                    else
                    {
                        /************************************************************************************/
                        SqlConnection connPrice = new SqlConnection(DataBaseConnection.connectionString2013);
                        connPrice.Open();
                        SqlDataAdapter sdaPrice = new SqlDataAdapter(SQLQueryText.QueryCurrentPriceByPLU, connPrice);
                        sdaPrice.SelectCommand.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@art",
                            Value = ArticlePLU,
                            SqlDbType = SqlDbType.NVarChar
                        });

                        DataSet dsPrice = new DataSet();
                        sdaPrice.Fill(dsPrice);
                        DataTable dtPrice = dsPrice.Tables[0];
                        sda.Dispose();
                        connPrice.Close();
                        if (dtPrice.Rows.Count != 0)
                        {
                            Spo s = new Spo
                            {
                                Barcode = barcode_string,
                                Art = ArticlePLU,
                                Art_desc = ArticleDescription,
                                Price_wo_discount = dtPrice.Rows[0]["p55"].ToString(),
                                Price_with_discount = "---",
                                Disc_no = "---",
                                Disc_percent = "---",
                                Disc_desc = "---"
                            };
                            tmpListSpo.Add(s);
                            //connPrice.Close();
                            //System.Diagnostics.Debug.WriteLine("стр 164 "+ barcode_string);
                        }
                        else
                        {
                            //Нет цены для 55 режима
                            Spo s = new Spo
                            {
                                Barcode = barcode_string,
                                Art = ArticlePLU,
                                Art_desc = ArticleDescription,
                                Price_wo_discount = "нет цены DF",
                                Price_with_discount = "---",
                                Disc_percent = "---",
                                Disc_no = "---",
                                Disc_desc = "---"
                            };
                            tmpListSpo.Add(s);
                            //System.Diagnostics.Debug.WriteLine("стр 180 " + barcode_string);
                        }
                    }
                }
                return new Slist() { SpoList = tmpListSpo };

            }
            catch (InvalidOperationException e)
            {
                System.Diagnostics.Debug.WriteLine("это конец " + e.Message);
                return null;
            }



        }


    }
}