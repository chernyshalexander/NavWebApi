using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NavWebApi.Utils
{
    
        public class ProcResult
        {
            public ProcResult(bool _res, string _msg, string _err, bool _sh)
            {
                Result = _res;
                Error = _err;
                Message = _msg;
                Show = _sh;
            }
            public bool Result;
            public bool Show;
            public string Message;
            public string Error;
        }
        public static class DataBaseConnection
        {
            public const string connectionString2013 = "Data Source = cluster-sql; Initial Catalog = IDF_2013; User Id = QlikDB;Password =G01DHX70zF";
            public const string connectionStringIDF = "Data Source = cluster-sql; Initial Catalog = IDF; User Id = QlikDB;Password =G01DHX70zF";

    }
    public static class SQLQueryText
        {
            public const string QueryCurrentPriceByPLU =
            @"SELECT
                  [Item No_]
                 ,[p55] = ISNULL([55], 0)
                 ,[p81] = ISNULL([81], 0)
                 ,[p57] = ISNULL([57], 0)
                FROM (SELECT
                    [Item No_]
                   ,pricecode
                   ,unitprice
                  FROM (SELECT
                      [Item No_] = sp.[Item No_]
                     ,[pricecode] = sp.[Sales Code]
                     ,[unitprice] = CAST(sp.[Unit Price] AS DECIMAL(18, 2))
                     ,ROW_NUMBER() OVER (PARTITION BY sp.[Item No_], sp.[Sales Code] ORDER BY sp.[Starting Date] DESC) AS row
                    FROM IDF_2013.dbo.[Sales Price] sp WITH (NOLOCK)
                    WHERE sp.[Sales Code] IN ('81','55','57')
                    AND sp.[Unit of Measure Code] = 'PCS'
                    AND sp.[Item No_] = @art) v1
                  WHERE v1.row = 1) pr
                PIVOT (MAX(pr.unitprice)
                FOR pricecode IN ([55], [81], [57])) pvt";
            public const string QueryItemByPLU_Barcode =
            @"SELECT
                     DISTINCT
                     i.No_ AS [art]
                     ,i.[Full Description RUS] AS [art_desc]
                    FROM Item i WITH (NOLOCK)
                    JOIN Barcodes b WITH (NOLOCK)
                      ON i.No_ = b.[Item No_]
                    WHERE 
                    -- i.No_ = @search_string
                    -- OR 
                       b.[Barcode No_] = @search_string
                      AND b.[Unit of Measure Code]= 'PCS'";
            public const string QuerySPObyBarocode =
            @"SELECT
                    b.[Barcode No_] AS [barcode]
                    ,idfpdl.No_ AS [art]
                    ,idfpdl.Description AS [art_desc]
                    ,idfpdl.[Standard Price Including VAT] AS [price_wo_discount]
                    ,IIF(idfpdl.[Disc_ Type] = 0, idfpdl.[Deal Price_Disc_ %], idfpdl.[Offer Price Including VAT]) AS [price_with_discount]
                    ,IIF(idfpdl.[Disc_ Type] = 1, idfpdl.[Deal Price_Disc_ %],
                        (idfpdl.[Standard Price Including VAT]- idfpdl.[Offer Price Including VAT])/idfpdl.[Standard Price Including VAT]) AS [disc_percent]
                    ,ipd.No_ AS [disc_no]
                    ,ipd.Description AS [disc_desc]
                FROM [Imperial Duty Free$Periodic Discount Line] idfpdl WITH(NOLOCK)
                JOIN [Imperial Duty Free$Periodic Discount] ipd WITH(NOLOCK)
                    ON idfpdl.[Offer No_] = ipd.No_
                JOIN Barcodes b WITH(NOLOCK)
                    ON @barcode = b.[Barcode No_]
                WHERE (idfpdl.No_ = b.[Item No_])
                AND idfpdl.[No_ of Items Needed] = 1
                AND idfpdl.[Line Group] = 1
                AND ipd.[Price Group] = '81'
                AND ipd.[Status] = 1
                AND b.[Unit of Measure Code] = 'PCS'";
        public const string QuerySPObyBarocode2 =
            @"SELECT
                    b.[Barcode No_] AS [barcode]
                    ,idfpdl.No_ AS [art]
                    ,idfpdl.Description AS [art_desc]
                    ,idfpdl.[Standard Price Including VAT] AS [price_wo_discount]
                    ,IIF(idfpdl.[Disc_ Type] = 0, idfpdl.[Deal Price_Disc_ %], idfpdl.[Offer Price Including VAT]) AS [price_with_discount]
                    ,IIF(idfpdl.[Disc_ Type] = 1, idfpdl.[Deal Price_Disc_ %],
                        (idfpdl.[Standard Price Including VAT]- idfpdl.[Offer Price Including VAT])/idfpdl.[Standard Price Including VAT]) AS [disc_percent]
                    ,ipd.No_ AS [disc_no]
                    ,ipd.Description AS [disc_desc]
                FROM [Imperial Duty Free$Periodic Discount Line] idfpdl WITH(NOLOCK)
                JOIN [Imperial Duty Free$Periodic Discount] ipd WITH(NOLOCK)
                    ON idfpdl.[Offer No_] = ipd.No_
                JOIN Barcodes b WITH(NOLOCK)
                    ON @barcode = b.[Barcode No_]
                WHERE (idfpdl.No_ = b.[Item No_])
                AND idfpdl.[No_ of Items Needed] = 1
                AND idfpdl.[Line Group] = 1
                AND ipd.[Price Group] = '55'
                AND ipd.[Status] = 1
                AND b.[Unit of Measure Code] = 'PCS'";


        public const string QueryPriceForTablet =
            @"SELECT
  t.[name]
 ,t.nameeng
 ,t.price
 ,t.price2
FROM (SELECT
    i.No_
   ,REPLACE(i.[Full Description ENG], '""', '') AS [nameeng]
   ,REPLACE(i.[Full Description RUS], '""', '') AS [name]
   ,CASE
      WHEN [SAP Mer_ Cat_] = 'L07000010' OR
        ([SAP Mer_ Cat_] = 'L01000030' AND
        [Full Description ENG] LIKE '%iQOS%') THEN 'Стики'
      WHEN [SAP Mer_ Cat_] IN ('L01000010', 'L01000015') THEN 'Сигареты'
      WHEN [SAP Mer_ Cat_] = 'L01000020' THEN 'Сигары/Сигариллы'
      WHEN [SAP Mer_ Cat_] = 'L01000030' AND
        [Full Description ENG] NOT LIKE '%iQOS%' THEN 'Табак курительный'
      WHEN [SAP Mer_ Cat_] = 'L07000040' THEN 'Система нагревания'
      ELSE [SAP Mer_ Cat_]
    END AS [Type]
   ,'€ ' + CAST(sp2.[55] AS VARCHAR(25)) AS [price]
   ,'€ ' + CAST(sp2.[57] AS VARCHAR(25)) AS [price2]
  FROM IDF_2013.dbo.Item i WITH (NOLOCK)
  LEFT JOIN (SELECT
      [Item No_]
     ,[55] = ISNULL([55], 0)
     ,[81] = ISNULL([81], 0)
     ,[57] = ISNULL([57], 0)
    FROM (SELECT
        [Item No_]
       ,pricecode
       ,unitprice
      FROM (SELECT
          [Item No_] = sp.[Item No_]
         ,[pricecode] = sp.[Sales Code]
         ,[unitprice] = CAST(sp.[Unit Price] AS DECIMAL(18, 2))
         ,ROW_NUMBER() OVER (PARTITION BY sp.[Item No_], sp.[Sales Code] ORDER BY sp.[Starting Date] DESC) AS row
        FROM IDF_2013.dbo.[Sales Price] sp WITH (NOLOCK)
        WHERE sp.[Sales Code] IN ('55', '81', '57')
        AND sp.[Unit of Measure Code] = 'PCS') v1
      WHERE v1.row = 1) pr
    PIVOT (MAX(pr.unitprice)
    FOR pricecode IN ([55], [81], [57])) pvt) sp2
    ON sp2.[Item No_] = i.No_
  WHERE i.[Division Code] = 'L'
  AND i.Translation_fTxt = 'Табачные изделия'
  AND i.[Full Description RUS] <> ''
  AND (sp2.[55] IS NOT NULL
  OR sp2.[55] <> 0)) t
WHERE t.No_ IN (SELECT DISTINCT
    itemno
  FROM WH_IDF.dbo.whRestTRS r
  WHERE itemno IN (SELECT
      No_
    FROM IDF_2013.dbo.Item i WITH (NOLOCK)
    WHERE i.[Division Code] = 'L'
    AND i.Translation_fTxt = 'Табачные изделия'
    AND i.[Full Description RUS] <> '')
  AND postdate IN (SELECT
      intDate
    FROM WH_IDF.dbo.calendar
    WHERE Date112 = CONVERT(VARCHAR(8), GETDATE() - 1, 112) /*'20220529'*/))
ORDER BY t.[Type], t.nameeng";
            }
    
}