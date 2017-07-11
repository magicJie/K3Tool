using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Tool.K3;
using Tool.Sql;

namespace K3Tool.Extend
{
    /// <summary>
    /// 地磅
    /// </summary>   
    public class Db
    {
        public static string RelatedConn = ConfigurationManager.ConnectionStrings["dbrelated"].ToString();
        public static string SourceConn = ConfigurationManager.ConnectionStrings["dbsource"].ToString();
        /// <summary>
        /// 入库单
        /// </summary>
        public class DbPurchasedWarehouse
        {
            public class Head : PurchasedWarehouse.Head
            {
                protected override string GetFpostyle()
                {
                    return "251";
                }

                protected override string GetFfmanagerid()
                {
                    var filter = string.Format("FName='{0}'", Ffmanagerid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string GetFsmanagerid()
                {
                    var filter = string.Format("FName='{0}'", Fsmanagerid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string GetFempid()
                {
                    var filter = string.Format("FName='{0}'", Fempid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string GetFbillerid()
                {
                    var filter = string.Format("FName='{0}'", Fbillerid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string GetFdcstockid()
                {
                    var filter = string.Format("FName='{0}'", Fdcstockid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.仓库, filter);
                }

                protected override string GetFsupplyid()
                {
                    var filter = string.Format("FName='{0}'", Fsupplyid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.供应商, filter);
                }
                /// <summary>
                /// 司机车号
                /// </summary>
                public string FHeadSelfA0143 { get; set; }
                
            }

            public class Body : PurchasedWarehouse.Body
            {
                protected override string GetFItemId()
                {
                    var filter = string.Format("FName='{0}'", Fitemid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.物料, filter);
                }

                protected override string GetFunitid()
                {
                    var filter = string.Format("FName='{0}'", Funitid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.单位, filter);
                }
                /// <summary>
                /// 检验是否是良品
                /// </summary>
                public string FChkPassItem { get; set; }
                /// <summary>
                /// 应收数量
                /// </summary>
                public string FAuxQtyMust { get; set; }
            }
            public static int Work(string kstime, string jstime)
            {
                CommonFunction.Initalize(SourceConn, "称重信息");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring =string.Format("select 流水号,更新时间,发货单位,毛重司磅员,车号 from 称重信息 where 过磅类型='PO' and 更新时间>='{0}' and 更新时间<='{1}' and kindeestate is null",kstime,jstime);
                var bodysqlstring = "select * from 称重信息 where 过磅类型='PO'";
                var headtable = SqlHelper.Query(SourceConn, headsqlstring, true);
                var bodytable = SqlHelper.Query(SourceConn, bodysqlstring);
                var i = 0;
                var number = CommonFunction.GetMaxNum(RelatedConn, ICStockBill.TableName);
                foreach (DataRow itemRow in headtable.Rows)
                {
                    i = i + 1;
                    Head head = new Head
                    {
                        FBillNo = itemRow["流水号"].ToString(),
                        Fdate = DateTime.Parse(itemRow["更新时间"].ToString()),
                        FSupplyID = itemRow["发货单位"].ToString(),
                        FSManagerID = itemRow["毛重司磅员"].ToString(),
                        FFManagerID = itemRow["毛重司磅员"].ToString(),
                        FEmpID = itemRow["毛重司磅员"].ToString(),
                        FBillerID = itemRow["毛重司磅员"].ToString(),
                        FDCStockID = "原料仓",
                        FHeadSelfA0143 = itemRow["车号"].ToString(),
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
                            FInterID = head.FInterID,
                            FQty =(float.Parse(bodyitemRow["实重"].ToString())/1000).ToString(),
                            Fauxqty = (float.Parse(bodyitemRow["实重"].ToString()) / 1000).ToString(),
                            Famount = bodyitemRow["金额"].ToString(),
                            Fauxprice =(float.Parse(bodyitemRow["单价"].ToString())*1000).ToString(),
                            FDCStockID = head.FDCStockID,
                            FUnitID = "吨",
                            FAuxQtyMust = (float.Parse(bodyitemRow["实重"].ToString()) / 1000).ToString(),
                            FChkPassItem = "1058",
                            FEntryID = j
                        };
                        bodyliList.Add(body);
                        j = j + 1;
                    }
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
        public class DbSalesOutLet
        {
            public class Head : SalesOutLet.Head
            {
                protected override int GetFsalestyle()
                {
                    return 100;
                }

                protected override string Getfdeptid()
                {
                    var filter = string.Format("FName='{0}'", Fdeptid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.部门, filter);
                }

                protected override string GetFsmanagerid()
                {
                    var filter = string.Format("FName='{0}'", Fsmanagerid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string GetFfmanagerid()
                {
                    var filter = string.Format("FName='{0}'", Ffmanagerid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string GetFbillerid()
                {
                    var filter = string.Format("FName='{0}'", Fbillerid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string GetFsupplyid()
                {
                    var filter = string.Format("FName='{0}'", Fsupplyid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.客户, filter);
                }

                protected override string GetFempid()
                {
                    if (Fempid == "")
                    {
                        return "";
                    }
                    var filter = string.Format("FName='{0}'", Fempid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }
               
                /// <summary>
                /// 工程名称
                /// </summary>
                public string FHeadSelfB0155 { get; set; }
                /// <summary>
                /// 司机车号
                /// </summary>
                public string FHeadSelfB0156 { get; set; }

            }

            public class Body : SalesOutLet.Body
            {
                protected override string GetFItemId()
                {
                    var filter = string.Format("FName='{0}'", Fitemid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.物料, filter);
                }

                protected override string GetFdcstockid()
                {
                    var filter = string.Format("FName='{0}'", Fdcstockid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.仓库, filter);
                }

                protected override string Getfunitid()
                {
                    var filter = string.Format("FName='{0}'", Funitid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.单位, filter);
                } 
                /// <summary>
                /// 检验是否是良品
                /// </summary>
                public string FChkPassItem { get; set; }
                /// <summary>
                /// 应发数量
                /// </summary>
                public string FAuxQtyMust { get; set; }
            }
            public static int Work(string kstime, string jstime)
            {
                CommonFunction.Initalize(SourceConn, "称重信息");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring =string.Format("select 流水号,更新时间,更新人,收货单位,备用1,车号,备用3 from  称重信息 where 过磅类型='SO' and 更新时间>='{0}' and 更新时间<='{1}' and kindeestate is null",kstime,jstime);
                var bodysqlstring = "select * from  称重信息";
                var headtable = SqlHelper.Query(SourceConn, headsqlstring, true);
                var bodytable = SqlHelper.Query(SourceConn, bodysqlstring);
                var i = 0;
                var number = CommonFunction.GetMaxNum(RelatedConn, ICStockBill.TableName);
                foreach (DataRow itemRow in headtable.Rows)
                {
                    i = i + 1;
                    Head head = new Head
                    {
                        FBillNo = itemRow["流水号"].ToString(),
                        Fdate = DateTime.Parse(itemRow["更新时间"].ToString()),
                        FDeptID = "销售部",
                        FBillerID = itemRow["更新人"].ToString(),
                        FFManagerID = itemRow["更新人"].ToString(),
                        FSManagerID = itemRow["更新人"].ToString(),
                        FSupplyID = itemRow["收货单位"].ToString(),
                        FHeadSelfB0155 = itemRow["备用1"].ToString(),
                        FHeadSelfB0156 = itemRow["车号"].ToString(),
                        FEmpID = itemRow["备用3"].ToString(),
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
                            FQty = (float.Parse(bodyitemRow["实重"].ToString()) / 1000).ToString() == "" ? "0" : (float.Parse(bodyitemRow["实重"].ToString()) / 1000).ToString(),
                            Fauxqty = (float.Parse(bodyitemRow["实重"].ToString()) / 1000).ToString() == "" ? "0" : (float.Parse(bodyitemRow["实重"].ToString()) / 1000).ToString(),
                            FUnitID = "吨",
                            FConsignPrice = (float.Parse(bodyitemRow["单价"].ToString()) * 1000).ToString() == "" ? "0" : (float.Parse(bodyitemRow["单价"].ToString()) * 1000).ToString(),
                            FConsignAmount = bodyitemRow["金额"].ToString() == "" ? "0" : bodyitemRow["金额"].ToString(),
                            FDCStockID ="成品库",
                            FInterID = head.FInterID,
                            FChkPassItem = "1058",
                            FAuxQtyMust = (float.Parse(bodyitemRow["实重"].ToString()) / 1000).ToString() == "" ? "0" : (float.Parse(bodyitemRow["实重"].ToString()) / 1000).ToString(),
                            FEntryID = j
                        };
                        bodyliList.Add(body);
                        j = j + 1;
                    }
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
