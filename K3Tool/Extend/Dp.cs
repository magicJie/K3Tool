using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Tool.K3;
using Tool.Sql;

namespace K3Tool.Extend
{
    /// <summary>
    ///大鹏
    /// </summary>
    public class Dp
    {
        public static string RelatedConn = ConfigurationManager.ConnectionStrings["dprelated"].ToString();
        public static string SourceConn = ConfigurationManager.ConnectionStrings["dpsource"].ToString();
        /// <summary>
        /// 入库单
        /// </summary>
        public class DpPurchasedWarehouse
        {
            public class Head : PurchasedWarehouse.Head
            {
                protected override string GetFpostyle()
                {
                    return "251";
                }

                protected override string GetFbillerid()
                {
                    return "16398";
                }
            }

            public class Body : PurchasedWarehouse.Body
            {
                protected override string GetFunitid()
                {
                    return "1";
                }
            }
            public static int Work()
            {
                CommonFunction.Initalize(SourceConn, "cmis_mk_voucher_main2");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring = "select 单据号,操作时间,供应商,操作人,'1.'+CONVERT(char(5),子库房) as 仓库 from cmis_mk_voucher_main2 where 业务类型='1' and kindeestate is null";
                var bodysqlstring = "select 单据号,药品ID,数量,进货总价,进货单价,药库单位 from cmis_mk_voucher_detail";
                var headtable = SqlHelper.Query(SourceConn, headsqlstring, true);
                var bodytable = SqlHelper.Query(SourceConn, bodysqlstring);
                var i = 0;
                var number = CommonFunction.GetMaxNum(RelatedConn, ICStockBill.TableName);
                foreach (DataRow itemRow in headtable.Rows)
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
                    var j = 0;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("单据号='{0}'",head.FBillNo)))
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
        public class DpSalesOutLet
        {
            public class Head : SalesOutLet.Head
            {
                protected override int GetFsalestyle()
                {
                    return 100;
                }

                protected override string GetFsupplyid()
                {
                    return "20";
                }

                protected override string GetFbillerid()
                {
                    return "16398";
                }
            }

            public class Body : SalesOutLet.Body
            {

            }
            public static int Work()
            {
                CommonFunction.Initalize(SourceConn, "cmis_chufang_detail");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring = "select 处方号,科室id,convert(nvarchar(10),录入时间,21) as 录入时间 from  cmis_chufang_detail where (处方类型=1 or 处方类型=2 or 处方类型=4) and kindeestate is null";
                var bodysqlstring = "select 处方号,总数量,收费项目id,CASE WHEN 最小单位=\'g\' THEN 单价*1000 else 单价 END as 新单价,总价格*剂数 as 新总价格,最小单位,\'2.\' + CONVERT(varchar(20),处方类型) as 出库类型 from  cmis_chufang_detail where (处方类型=1 or 处方类型=2 or 处方类型=4)";
                var headtable = SqlHelper.Query(SourceConn, headsqlstring, true);
                var bodytable = SqlHelper.Query(SourceConn, bodysqlstring);
                var i = 0;
                var number = CommonFunction.GetMaxNum(RelatedConn, ICStockBill.TableName);
                foreach (DataRow itemRow in headtable.Rows)
                {
                    Head head = new Head
                    {
                        FBillNo = itemRow["处方号"].ToString(),
                        Fdate = DateTime.Parse(itemRow["录入时间"].ToString()),
                        FDeptID=itemRow["科室id"].ToString(),
                        FInterID = number + i
                    };
                    headliList.Add(head);
                    recordlist.Add(string.Format("update cmis_chufang_detail set kindeestate='1' where 处方号='{0}'", itemRow["处方号"]));                    
                    var j = 0;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("处方号='{0}'",head.FBillNo)))
                    {
                        Body body = new Body
                        {
                            FItemID = bodyitemRow["收费项目id"].ToString(),                            
                            FQty = bodyitemRow["总数量"].ToString(),
                            Fauxqty = bodyitemRow["总数量"].ToString(),                            
                            FUnitID = bodyitemRow["最小单位"].ToString(),
                            FConsignPrice = bodyitemRow["新单价"].ToString(),
                            FConsignAmount = bodyitemRow["新总价格"].ToString(),
                            FDCStockID = bodyitemRow["出库类型"].ToString(),                            
                            FInterID = head.FInterID,
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
        /// 领料单
        /// </summary>
        public class DpPicking
        {
            public class Head:Picking.Head
            {
                protected override string GetFPurposeId()
                {
                    return "12000";
                }

                protected override string GetFbillerid()
                {
                    return "16398";
                }
            }
            public class Body:Picking.Body
            {
                 
            }
            public static int Work()
            {
                CommonFunction.Initalize(SourceConn, "cmis_mk_voucher_main2");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring = "select 单据号,接收科室,操作时间,操作人,'1.'+CONVERT(char(5),子库房) as 仓库 from cmis_mk_voucher_main2 where 业务类型=14 and kindeestate is null";
                var bodysqlstring = "select 单据号,药品ID,数量,药库单位 from cmis_mk_voucher_detail";
                var headtable = SqlHelper.Query(SourceConn, headsqlstring, true);
                var bodytable = SqlHelper.Query(SourceConn, bodysqlstring);
                var i = 0;
                var number = CommonFunction.GetMaxNum(RelatedConn, ICStockBill.TableName);
                foreach (DataRow itemRow in headtable.Rows)
                {
                    Head head = new Head
                    {
                        FBillNo = itemRow["单据号"].ToString(),
                        Fdate = DateTime.Parse(itemRow["操作时间"].ToString()),
                        FDeptID = itemRow["接收科室"].ToString(),
                        FSManagerID = itemRow["操作人"].ToString(),
                        FFManagerID = itemRow["操作人"].ToString(),                        
                        FInterID = number + i                        
                    };
                    headliList.Add(head);
                    recordlist.Add(string.Format("update cmis_mk_voucher_main2 set kindeestate='1' where 单据号='{0}'", head.FBillNo));
                    var j = 0;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("单据号='{0}'", head.FBillNo)))
                    {
                        Body body = new Body
                        {
                            FItemID = bodyitemRow["药品ID"].ToString(),
                            FQty = bodyitemRow["数量"].ToString(),
                            Fauxqty = bodyitemRow["数量"].ToString(),
                            FUnitID = bodyitemRow["药库单位"].ToString(),                            
                            FDCStockID = itemRow["仓库"].ToString(),
                            FInterID = head.FInterID,
                            FEntryID = j,                            
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
