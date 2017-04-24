using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Tool.K3;
using Tool.Sql;

namespace K3Tool.Extend
{
    /// <summary>
    /// 过磅
    /// </summary>   
    public class Gb
    {
        public static string RelatedConn = ConfigurationManager.ConnectionStrings["gbrelated"].ToString();
        public static string SourceConn = ConfigurationManager.ConnectionStrings["gbsource"].ToString();
        /// <summary>
        /// 入库单
        /// </summary>
        public class GbPurchasedWarehouse
        {
            public class Head : PurchasedWarehouse.Head
            {

            }

            public class Body : PurchasedWarehouse.Body
            {

            }
            public static int Work()
            {
                CommonFunction.Initalize(SourceConn, "cmis_mk_voucher_main2");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring = "select 单据号,操作时间,供应商,操作人,'1.'+CONVERT(char(5),子库房) as 仓库 from cmis_mk_voucher_main2 where 业务类型='1' and kindeestate is null";
                var i = 0;
                var number = CommonFunction.GetMaxNum(RelatedConn, ICStockBill.TableName);
                foreach (DataRow itemRow in SqlHelper.Query(SourceConn, headsqlstring, true).Rows)
                {
                    Head head = new Head
                    {
                        FBillNo = itemRow["单据号"].ToString(),
                        Fdate = DateTime.Parse(itemRow["操作时间"].ToString()),
                        FSupplyID = itemRow["供应商"].ToString(),
                        FSManagerID = itemRow["操作人"].ToString(),
                        FFManagerID = itemRow["操作人"].ToString(),
                        FEmpID = itemRow["操作人"].ToString(),
                        FInterID = number + i
                    };
                    headliList.Add(head);
                    recordlist.Add(string.Format("update cmis_mk_voucher_main2 set kindeestate='1' where 单据号='{0}'", itemRow["单据号"]));
                    var bodysqlstring = string.Format("select 药品ID,数量,进货总价,进货单价,药库单位 from cmis_mk_voucher_detail where 单据号='{0}'", head.FBillNo);
                    var j = 0;
                    foreach (DataRow bodyitemRow in SqlHelper.Query(SourceConn, bodysqlstring).Rows)
                    {
                        Body body = new Body
                        {
                            FItemID = bodyitemRow["药品ID"].ToString(),
                            FInterID = head.FInterID,
                            FQty = bodyitemRow["数量"].ToString(),
                            Fauxqty = bodyitemRow["数量"].ToString(),
                            Famount = bodyitemRow["进货总价"].ToString(),
                            Fauxprice = bodyitemRow["进货单价"].ToString(),
                            FDCStockID = head.FDCStockID,
                            FUnitID = bodyitemRow["药库单位"].ToString(),
                            FEntryID = j
                        };
                        bodyliList.Add(body);
                        j = j + 1;
                    }
                    i = i + 1;
                }
                var headsqlstringlist = CommonFunction.GetSqlList(RelatedConn, headliList, ICStockBill.TableName);
                var bodysqlstringlist = CommonFunction.GetSqlList(RelatedConn, bodyliList, ICStockBillEntry.TableName);
                headsqlstringlist.AddRange(bodysqlstringlist);
                var resultnumber = SqlHelper.ExecuteSqlTran(RelatedConn, headsqlstringlist);
                SqlHelper.ExecuteSqlTran(SourceConn, recordlist);
                CommonFunction.UpdateMaxNum(RelatedConn, ICStockBill.TableName, number + i);
                return resultnumber;
            }
        }
        /// <summary>
        /// 出库单
        /// </summary>
        public class GbSalesOutLet
        {
            public class Head : SalesOutLet.Head
            {

            }

            public class Body : SalesOutLet.Body
            {

            }
            public static int Work()
            {
                CommonFunction.Initalize(SourceConn, "cmis_mk_voucher_main2");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring = "select 单据号,操作时间,供应商,操作人,'1.'+CONVERT(char(5),子库房) as 仓库 from cmis_mk_voucher_main2 where 业务类型='1' and kindeestate is null";
                var i = 0;
                var number = CommonFunction.GetMaxNum(RelatedConn, ICStockBill.TableName);
                foreach (DataRow itemRow in SqlHelper.Query(SourceConn, headsqlstring, true).Rows)
                {
                    Head head = new Head
                    {
                        FBillNo = itemRow["单据号"].ToString(),
                        Fdate = DateTime.Parse(itemRow["操作时间"].ToString()),
                        FSupplyID = itemRow["供应商"].ToString(),                       
                        FInterID = number + i
                    };
                    headliList.Add(head);
                    recordlist.Add(string.Format("update cmis_mk_voucher_main2 set kindeestate='1' where 单据号='{0}'", itemRow["单据号"]));
                    var bodysqlstring = string.Format("select 药品ID,数量,进货总价,进货单价,药库单位 from cmis_mk_voucher_detail where 单据号='{0}'", head.FBillNo);
                    var j = 0;
                    foreach (DataRow bodyitemRow in SqlHelper.Query(SourceConn, bodysqlstring).Rows)
                    {
                        Body body = new Body
                        {
                            FItemID = bodyitemRow["药品ID"].ToString(),
                            FInterID = head.FInterID,
                            FQty = bodyitemRow["数量"].ToString(),
                            Fauxqty = bodyitemRow["数量"].ToString(),                                                        
                            FUnitID = bodyitemRow["药库单位"].ToString(),
                            FEntryID = j
                        };
                        bodyliList.Add(body);
                        j = j + 1;
                    }
                    i = i + 1;
                }
                var headsqlstringlist = CommonFunction.GetSqlList(RelatedConn, headliList, ICStockBill.TableName);
                var bodysqlstringlist = CommonFunction.GetSqlList(RelatedConn, bodyliList, ICStockBillEntry.TableName);
                headsqlstringlist.AddRange(bodysqlstringlist);
                var resultnumber = SqlHelper.ExecuteSqlTran(RelatedConn, headsqlstringlist);
                SqlHelper.ExecuteSqlTran(SourceConn, recordlist);
                CommonFunction.UpdateMaxNum(RelatedConn, ICStockBill.TableName, number + i);
                return resultnumber;
            }
        }
       
    }
}
