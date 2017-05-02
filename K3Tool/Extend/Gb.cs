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
                protected override string GetFpostyle()
                {
                    return "251";
                }

                protected override string GetFfmanagerid()
                {
                    var filter = string.Format("FNumber='{0}'", Ffmanagerid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string GetFsmanagerid()
                {
                    var filter = string.Format("FNumber='{0}'", Fsmanagerid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string GetFempid()
                {
                    var filter = string.Format("FNumber='{0}'", Fempid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string GetFbillerid()
                {
                    var filter = string.Format("FNumber='{0}'", Fbillerid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string GetFdcstockid()
                {
                    var filter = string.Format("FNumber='{0}'", Fdcstockid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.仓库, filter);
                }

                protected override string GetFsupplyid()
                {
                    var filter = string.Format("FNumber='{0}'", Fsupplyid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.供应商, filter);
                }
            }

            public class Body : PurchasedWarehouse.Body
            {
                protected override string GetFItemId()
                {
                    var filter = string.Format("FNumber='{0}'", Fitemid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.物料, filter);
                }

                protected override string GetFunitid()
                {
                    var filter = string.Format("FName='{0}'", Funitid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.单位, filter);
                }
            }
            public static int Work()
            {
                CommonFunction.Initalize(SourceConn, "称重信息");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring = "select * from 称重信息 where 过磅类型='PO' kindeestate is null";
                var bodysqlstring = "select * from 称重信息 where 过磅类型='PO'";
                var headtable = SqlHelper.Query(SourceConn, headsqlstring, true);
                var bodytable = SqlHelper.Query(SourceConn, bodysqlstring);
                var i = 0;
                var number = CommonFunction.GetMaxNum(RelatedConn, ICStockBill.TableName);
                foreach (DataRow itemRow in headtable.Rows)
                {
                    Head head = new Head
                    {
                        FBillNo = itemRow["流水号"].ToString(),
                        Fdate = DateTime.Parse(itemRow["毛重时间"].ToString()),
                        FSupplyID = itemRow["发货单位"].ToString(),
                        FSManagerID = itemRow["毛中司磅员"].ToString(),
                        FFManagerID = itemRow["毛中司磅员"].ToString(),
                        FEmpID = itemRow["毛中司磅员"].ToString(),
                        FBillerID = itemRow["毛中司磅员"].ToString(),
                        FDCStockID = itemRow[""].ToString(),
                        FInterID = number + i
                    };
                    headliList.Add(head);
                    recordlist.Add(string.Format("update 称重 set kindeestate='1' where 流水号='{0}'", itemRow["流水号"]));
                    var j = 1;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("流水号='{0}'", head.FBillNo)))
                    {
                        Body body = new Body
                        {
                            FItemID = bodyitemRow["货名"].ToString(),
                            FInterID = head.FInterID,
                            FQty = bodyitemRow["实重"].ToString(),
                            Fauxqty = bodyitemRow["实重"].ToString(),
                            Famount = bodyitemRow["金额"].ToString(),
                            Fauxprice = bodyitemRow["单价"].ToString(),
                            FDCStockID = head.FDCStockID,
                            FUnitID = bodyitemRow[""].ToString(),
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
                protected override int GetFsalestyle()
                {
                    return 100;
                }

                protected override string Getfdeptid()
                {
                    var filter = string.Format("FNumber='{0}'", Fdeptid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.部门, filter);
                }

                protected override string GetFsmanagerid()
                {
                    var filter = string.Format("FName='{0}'", "邹洪雪");
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string GetFfmanagerid()
                {
                    var filter = string.Format("FName='{0}'", "邹洪雪");
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string GetFbillerid()
                {
                    var filter = string.Format("FName='{0}'", "邹洪雪");
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string GetFsupplyid()
                {
                    var filter = string.Format("FNumber='{0}'", Fsupplyid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.客户, filter);
                }

            }

            public class Body : SalesOutLet.Body
            {
                protected override string GetFItemId()
                {
                    var filter = string.Format("FNumber='{0}'", Fitemid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.物料, filter);
                }

                protected override string GetFdcstockid()
                {
                    var filter = string.Format("FNumber='{0}'", Fdcstockid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.仓库, filter);
                }

                protected override string Getfunitid()
                {
                    var filter = string.Format("FName='{0}'", Funitid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.单位, filter);
                }
            }
            public static int Work()
            {
                CommonFunction.Initalize(SourceConn, "称重信息");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring = "select * from  称重信息 where 过磅类型='SO' kindeestate is null";
                var bodysqlstring = "select * from  称重信息";
                var headtable = SqlHelper.Query(SourceConn, headsqlstring, true);
                var bodytable = SqlHelper.Query(SourceConn, bodysqlstring);
                var i = 0;
                var number = CommonFunction.GetMaxNum(RelatedConn, ICStockBill.TableName);
                foreach (DataRow itemRow in headtable.Rows)
                {
                    Head head = new Head
                    {
                        FBillNo = itemRow["流水号"].ToString(),
                        Fdate = DateTime.Parse(itemRow["毛重时间"].ToString()),
                        FDeptID = itemRow[""].ToString(),
                        FBillerID = itemRow["毛重司磅员"].ToString(),
                        FInterID = number + i
                    };
                    headliList.Add(head);
                    recordlist.Add(string.Format("update 称重信息 set kindeestate='1' where 流水号='{0}'", itemRow["流水号"]));
                    var j = 1;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("流水号='{0}'", head.FBillNo)))
                    {
                        Body body = new Body
                        {
                            FItemID = bodyitemRow["货名"].ToString(),
                            FQty = bodyitemRow["实重"].ToString() == "" ? "0" : bodyitemRow["实重"].ToString(),
                            Fauxqty = bodyitemRow["实重"].ToString() == "" ? "0" : bodyitemRow["实重"].ToString(),
                            FUnitID = bodyitemRow[""].ToString(),
                            FConsignPrice = bodyitemRow["单价"].ToString() == "" ? "0" : bodyitemRow["单价"].ToString(),
                            FConsignAmount = bodyitemRow["金额"].ToString() == "" ? "0" : bodyitemRow["金额"].ToString(),
                            FDCStockID = bodyitemRow[""].ToString(),
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
       
    }
}
