using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Tool.Common;
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
        /// 外购入库单
        /// </summary>
        public class DpPurchasedWarehouse
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
            public static int Work(string kstime, string jstime)
            {
                CommonFunction.Initalize(SourceConn, "cmis_mk_voucher_main2");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring =string.Format("select 单据号,操作时间,供应商,操作人,'1.'+CONVERT(char(5),子库房) as 仓库 from cmis_mk_voucher_main2 where 业务类型='1' and kindeestate is null and 操作时间>='{0}' and 操作时间<='{1}'",kstime,jstime);
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
                        FBillerID = itemRow["操作人"].ToString(),
                        FDCStockID = itemRow["仓库"].ToString(),
                        FInterID = number + i
                    };
                    headliList.Add(head);
                    recordlist.Add(string.Format("update cmis_mk_voucher_main2 set kindeestate='1' where 单据号='{0}'", itemRow["单据号"]));
                    var j = 1;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("单据号='{0}'", head.FBillNo)))
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
        /// 销售出库单
        /// </summary>
        public class DpSalesOutLet
        {
            public class Head : SalesOutLet.Head
            {
                private string _ys;
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
                    var filter = string.Format("FName='{0}'", "客户");
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.客户, filter);
                }

                protected override string GetFempid()
                {
                    var filter = string.Format("FName='{0}'", Fempid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }
                /// <summary>
                /// 医师
                /// </summary>
                public string FHeadSelfB0154
                {
                    get
                    {
                        var filter = string.Format("FNumber='{0}'", _ys);
                        return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.医师, filter);
                    }
                    set { _ys = value; } }
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

            public static int Work(string kstime,string jstime)
            {
                CommonFunction.Initalize(SourceConn, "cmis_chufang_detail");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring = string.Format("select 处方号,科室id,医生id,录入人,convert(nvarchar(10),录入时间,21) as 录入时间 from  cmis_chufang_detail where 录入时间>='{0}' and 录入时间<='{1}' and (处方类型=1 or 处方类型=2 or 处方类型=4) and kindeestate is null",kstime,jstime);
                var bodysqlstring = "select 处方号,总数量*剂数 as 实发数量,收费项目id,CASE WHEN 最小单位=\'g\' THEN 单价 else 单价 END as 新单价,总价格*剂数 as 新总价格,最小单位,单位,剂数,\'2.\' + CONVERT(varchar(20),处方类型) as 出库类型 from  cmis_chufang_detail where (处方类型=1 or 处方类型=2 or 处方类型=4)";
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
                        FDeptID = itemRow["科室id"].ToString(),
                        FBillerID = itemRow["录入人"].ToString(),
                        FEmpID = itemRow["录入人"].ToString(),
                        //FSupplyID = itemRow["科室id"].ToString(),
                        FHeadSelfB0154=itemRow["医生id"].ToString(),
                        FInterID = number + i
                    };                    
                    headliList.Add(head);
                    recordlist.Add(string.Format("update cmis_chufang_detail set kindeestate='1' where 处方号='{0}'", itemRow["处方号"]));
                    var j = 1;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("处方号='{0}'", head.FBillNo)))
                    {
                        Body body = new Body
                        {
                            FItemID = bodyitemRow["收费项目id"].ToString(),
                            FQty = bodyitemRow["实发数量"].ToString() == "" ? "0" : bodyitemRow["实发数量"].ToString(),
                            Fauxqty = bodyitemRow["实发数量"].ToString() == "" ? "0" : bodyitemRow["实发数量"].ToString(),
                            FUnitID = bodyitemRow["最小单位"].ToString(),
                            FConsignPrice = bodyitemRow["新单价"].ToString() == "" ? "0" : bodyitemRow["新单价"].ToString(),
                            FConsignAmount = bodyitemRow["新总价格"].ToString() == "" ? "0" : bodyitemRow["新总价格"].ToString(),
                            FDCStockID = bodyitemRow["出库类型"].ToString(),
                            FInterID = head.FInterID,
                            FEntryID = j
                        };
                        if (bodyitemRow["最小单位"].ToString() == "g" && bodyitemRow["单位"].ToString() == "kg")
                        {
                            body.FQty = (Convert.ToDouble(body.FQty)/1000).ToString(CultureInfo.InvariantCulture);
                        }
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
        /// 生产领料单
        /// </summary>
        public class DpPicking
        {
            public class Head : Picking.Head
            {
                protected override string GetFPurposeId()
                {
                    return "12000";
                }

                protected override string GetFfmanagerId()
                {
                    var filter = string.Format("FNumber='{0}'", FfmanagerId);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string GetFsManagerId()
                {
                    var filter = string.Format("FNumber='{0}'", FsmanagerId);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string Getfdeptid()
                {
                    var filter = string.Format("FNumber='{0}'", Fdeptid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.部门, filter);
                }

                protected override string GetFbillerid()
                {
                    var filter = string.Format("FNumber='{0}'", Fbillerid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }
            }
            public class Body : Picking.Body
            {
                protected override string GetFItemId()
                {
                    var filter = string.Format("FNumber='{0}'", Fitemid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.物料, filter);
                }

                protected override string Getfunitid()
                {
                    var filter = string.Format("FName='{0}'", Funitid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.单位, filter);
                }

                protected override string GetFscstockid()
                {
                    var filter = string.Format("FNumber='{0}'", FscStockId);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.仓库, filter);
                }
            }
            public static int Work(string kstime, string jstime)
            {
                CommonFunction.Initalize(SourceConn, "cmis_mk_voucher_main2");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring =string.Format("select 单据号,接收科室,操作时间,操作人,'1.'+CONVERT(char(5),子库房) as 仓库 from cmis_mk_voucher_main2 where 业务类型=14 and kindeestate is null and 操作时间>='{0}' and 操作时间<='{1}'",kstime,jstime);
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
                        FBillerID = itemRow["操作人"].ToString(),
                        FInterID = number + i
                    };
                    headliList.Add(head);
                    recordlist.Add(string.Format("update cmis_mk_voucher_main2 set kindeestate='1' where 单据号='{0}'", head.FBillNo));
                    var j = 1;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("单据号='{0}'", head.FBillNo)))
                    {
                        Body body = new Body
                        {
                            FItemID = bodyitemRow["药品ID"].ToString(),
                            FQty = bodyitemRow["数量"].ToString(),
                            Fauxqty = bodyitemRow["数量"].ToString(),
                            FUnitID = bodyitemRow["药库单位"].ToString(),
                            FSCStockID = itemRow["仓库"].ToString(),
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
        /// 产品入库单
        /// </summary>
        public class DpProductInventory
        {
            public class Head : ProductInventory.Head
            {
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
                protected override string GetFbillerid()
                {
                    var filter = string.Format("FName='{0}'", "邹洪雪");
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }
                protected override string Getfdeptid()
                {
                    var filter = string.Format("FNumber='{0}'", Fdeptid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.部门, filter);
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

                protected override string GetFdcstockid()
                {
                    var filter = string.Format("FNumber='{0}'", Fdcstockid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.仓库, filter);
                }
            }
            public static int Work(string kstime, string jstime)
            {
                CommonFunction.Initalize(SourceConn, "cmis_mk_voucher_main2");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring = string.Format("select 单据号,'1.'+CONVERT(char(5),子库房) as 仓库,20 as 交货单位,操作时间,操作人 from cmis_mk_voucher_main2 where 业务类型='2' and kindeestate is null and 操作时间>='{0}' and 操作时间<='{1}'",kstime,jstime);
                var bodysqlstring = "select 单据号,药品ID,药库单位,数量,进货单价,进货总价 from cmis_mk_voucher_detail";
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
                        FSManagerID = itemRow["操作人"].ToString(),
                        FFManagerID = itemRow["操作人"].ToString(),
                        FDeptID = itemRow["交货单位"].ToString(),
                        FInterID = number + i
                    };
                    headliList.Add(head);
                    recordlist.Add(string.Format("update cmis_mk_voucher_main2 set kindeestate='1' where 单据号='{0}'", itemRow["单据号"]));
                    var j = 1;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("单据号='{0}'", head.FBillNo)))
                    {
                        Body body = new Body
                        {
                            FItemID = bodyitemRow["药品ID"].ToString(),
                            FInterID = head.FInterID,
                            FQty = bodyitemRow["数量"].ToString(),
                            Fauxqty = bodyitemRow["数量"].ToString(),
                            Famount = bodyitemRow["进货总价"].ToString(),
                            Fauxprice = bodyitemRow["进货单价"].ToString(),
                            FUnitID = bodyitemRow["药库单位"].ToString(),
                            FDCStockID = itemRow["仓库"].ToString(),
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
        /// 调拨单
        /// </summary>
        public class DpRequisitionSlip
        {
            public class Head : RequisitionSlip.Head
            {

                protected override string GetFsmanagerid()
                {
                    var filter = string.Format("FNumber='{0}'", Fsmanagerid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string GetFfmanagerid()
                {
                    var filter = string.Format("FNumber='{0}'", Ffmanagerid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }

                protected override string GetFbillerid()
                {
                    var filter = string.Format("FName='{0}'", "邹洪雪");
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }
            }
            public class Body : RequisitionSlip.Body
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

                protected override string GetFscstockid()
                {
                    var filter = string.Format("FNumber='{0}'", Fscstockid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.仓库, filter);
                }

                protected override string GetFdcstockid()
                {
                    var filter = string.Format("FNumber='{0}'", Fdcstockid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.仓库, filter);
                }
            }
            public static int Work(string kstime, string jstime)
            {
                CommonFunction.Initalize(SourceConn, "cmis_mk_voucher_main2");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring = string.Format("select 单据号,操作人,操作时间,'1.' + CONVERT(varchar(20),子库房) as 调出仓库,'2.' + CONVERT(varchar(20),子库房) as 调入仓库 from cmis_mk_voucher_main2 where 业务类型=11 and kindeestate is null and 操作时间>='{0}' and 操作时间<='{1}'",kstime,jstime);
                var bodysqlstring = "select 单据号,药品ID,数量,进货单价,进货总价,药库单位 from cmis_mk_voucher_detail";
                var headtable = SqlHelper.Query(SourceConn, headsqlstring, true);
                var bodytable = SqlHelper.Query(SourceConn, bodysqlstring);
                var i = 0;
                var number = CommonFunction.GetMaxNum(RelatedConn, ICStockBill.TableName);
                var logList = new List<Tuple<string, List<Tuple<string, string, string>>>>();
                foreach (DataRow itemRow in headtable.Rows)
                {
                    Head head = new Head
                    {
                        FBillNo = itemRow["单据号"].ToString(),
                        Fdate = DateTime.Parse(itemRow["操作时间"].ToString()),
                        FSManagerID = itemRow["操作人"].ToString(),
                        FFManagerID = itemRow["操作人"].ToString(),
                        FInterID = number + i
                    };
                    headliList.Add(head);
                    recordlist.Add(string.Format("update cmis_mk_voucher_main2 set kindeestate='1' where 单据号='{0}'", head.FBillNo));
                    var j = 1;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("单据号='{0}'", head.FBillNo)))
                    {
                        var fqty = bodyitemRow["数量"].ToString() == "" ? "0" : bodyitemRow["数量"].ToString();
                        var fAmtRef = bodyitemRow["进货总价"].ToString() == "" ? "0" : bodyitemRow["进货总价"].ToString();
                        var fAuxPriceRef = bodyitemRow["进货单价"].ToString() == "" ? "0" : bodyitemRow["进货单价"].ToString();
                        Body body = new Body
                        {
                            FItemID = bodyitemRow["药品ID"].ToString(),
                            FQty = fqty,
                            Fauxqty = fqty,
                            FUnitID = bodyitemRow["药库单位"].ToString(),
                            FDCStockID = itemRow["调入仓库"].ToString(),
                            FSCStockID = itemRow["调出仓库"].ToString(),
                            FInterID = head.FInterID,
                            FAmtRef = fAmtRef,
                            FAuxPriceRef = fAuxPriceRef,
                            Fauxprice = fAuxPriceRef,
                            Famount = fAmtRef,
                            FEntryID = j
                        };
                        var list = new List<Tuple<string, string, string>>();
                        if (bodyitemRow["数量"].ToString() == "")
                        {
                            list.Add(new Tuple<string, string, string>("FQty", "", "0"));
                            list.Add(new Tuple<string, string, string>("Fauxqty", "", "0"));
                        }
                        if (bodyitemRow["进货总价"].ToString() == "")
                        {
                            list.Add(new Tuple<string, string, string>("FAmtRef", "", "0"));
                            list.Add(new Tuple<string, string, string>("Famount", "", "0"));
                        }
                        if (bodyitemRow["进货单价"].ToString() == "")
                        {
                            list.Add(new Tuple<string, string, string>("FAuxPriceRef", "", "0"));
                            list.Add(new Tuple<string, string, string>("Fauxprice", "", "0"));
                        }
                        logList.Add(new Tuple<string, List<Tuple<string, string, string>>>(head.FBillNo, list));
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
                //事务提交了才写日志
                logList.ForEach(x => LoggerHelper.WriteWarnInfo(ICStockBillEntry.TableName, "单据号", x.Item1, x.Item2));
                return resultnumber;
            }
        }
        /// <summary>
        /// 收款单
        /// </summary>
        public class DpNewReceiveBill
        {
            public class Head : NewReceiveBill.Head
            {
                private string _fPreparer;
                private string _fEmployee;
                private int _fDepartment;
                private string _FBase;
                /// <summary>
                /// 单据金额
                /// </summary>
                public decimal FAmountFor
                {
                    get;
                    set;
                }
                /// <summary>
                /// 单据金额（本位币）
                /// </summary>
                public decimal FAmount
                {
                    get;
                    set;
                }
                /// <summary>
                /// 制单人
                /// </summary>
                public string FPreparer
                {
                    set { _fPreparer = value; }
                    get
                    {
                        //var filter = string.Format("FName='{0}'", _fPreparer);
                        //return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                        return "28";
                    }
                }
                /// <summary>
                /// 业务员
                /// </summary>
                public string FEmployee
                {
                    set { _fEmployee = value; }
                    get
                    {
                        //var filter = string.Format("FName='{0}'", _fEmployee);
                        //return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                        return "28";
                    }
                }
                /// <summary>
                /// 部门
                /// </summary>
                public int FDepartment
                {
                    set { _fDepartment = value; }
                    get
                    {
                        var filter = string.Format("FNumber='{0}'", _fDepartment);
                        return int.Parse(CommonFunction.Getfitemid(RelatedConn, Fitemclassid.部门, filter));
                    }
                }
                /// <summary>
                /// 医师
                /// </summary>
                public string FBase
                {
                    get
                    {
                        var filter = string.Format("FNumber='{0}'", _FBase);
                        return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.医师, filter);
                    }
                    set { _FBase = value; }
                }

                protected override int GetFBillType()
                {                   
                    try
                    {
                        var sqlstring = string.Format("select fitemid from t_rp_systemenum where FNumber='{0}'", _FBillType);
                        var fitemid = SqlHelper.Query(RelatedConn, sqlstring);
                        var a = fitemid.Rows[0]["fitemid"].ToString();
                        return int.Parse(a);
                    }
                    catch (Exception ex)
                    {
                       throw new Exception("收款类型"+_FBillType+"找不到");
                    }                    
                }

                protected override int GetFAccountID()
                {
                    //现金收费科目暂时固定为0
                    return 0;
                }

                protected override int GetFAccountID_CN()
                {
                    //现金收费科目暂时固定为0
                    return 0;
                }
            }
            public class Body : NewReceiveBill.Body
            {
                /// <summary>
                /// 结算折扣金额
                /// </summary>
                public string FDiscountFor { get; set; }
                /// <summary>
                /// 结算折扣金额本位币
                /// </summary>
                public string FDiscount { get; set; }

                protected override int GetFAccountID()
                {
                    //暂时固定为1700
                    return 1700;
                }
            }
            //public class RPContact : NewReceiveBill.RPContact
            //{
            //}
            public static int Work(string kstime,string jstime)
            {
                CommonFunction.Initalize(SourceConn, "cmis_chufang_detail");
                var headliList = new List<NewReceiveBill.Head>();
                var bodyliList = new List<NewReceiveBill.Body>();
                var contactList = new List<NewReceiveBill.RPContact>();
                var headsqlstring =String.Format(@"select id,处方号,科室id,医生id,处方类型,总价格,录入人,录入时间,'耿惠平' as 制单人,'客户' as 客户,处方类型,折扣金额 
                                      from  cmis_chufang_detail where 录入时间>='{0}' and 录入时间<='{1}' and 处方类型 in (3,5,6,8,9,10,15) and kindeestate is null", kstime,jstime);
                var conn = new SqlConnection(SourceConn);
                var headReader = SqlHelper.GetDataReader(conn, headsqlstring);
                try
                {
                    var number = CommonFunction.GetMaxNum(RelatedConn, NewReceiveBill.Head.TableName);
                    var i = 0;
                    var batchNum = 0;
                    var recordIds = new List<string>();
                    //1000条数据提交一次，规避大事务
                    while (headReader.Read())
                    {
                        if (batchNum == 1000)
                        {
                            DoBatch(number + i, headliList, bodyliList, contactList, recordIds);
                            headliList.Clear();
                            bodyliList.Clear();
                            contactList.Clear();
                            recordIds.Clear();
                            batchNum = 0;
                            continue;
                        }
                        var totalAmount = decimal.Parse(headReader["总价格"].ToString());
                        var discountamount = decimal.Parse(headReader["折扣金额"].ToString());
                        Head head = new Head
                        {
                            FDate = DateTime.Parse(headReader["录入时间"].ToString()),
                            FFincDate = DateTime.Parse(headReader["录入时间"].ToString()),
                            FExchangeRateType = 1,
                            FCurrencyID = 1,
                            FExchangeRate = 1,
                            FItemClassID=1,
                            FCustomer = 318,
                            FRP = 1,
                            FBillID = number + i,
                            FNumber = "XSKD" + number + i,
                            FAmountFor = totalAmount,
                            FAmount = totalAmount,
                            FPreparer = headReader["制单人"].ToString(),
                            FEmployee = headReader["录入人"].ToString(),
                            FDepartment = int.Parse(headReader["科室id"].ToString()),
                            FAdjustExchangeRate = 1,
                            FBillType =int.Parse(headReader["处方类型"].ToString()),
                            FClassTypeID = "1000005",
                            FReceiveAmount = totalAmount.ToString(),
                            FReceiveAmountFor = totalAmount.ToString(),
                            FSettleAmount = totalAmount.ToString(),
                            FSettleAmountFor =totalAmount.ToString(),
                            FBase = headReader["医生id"].ToString(),                            
                            FYear = DateTime.Parse(headReader["录入时间"].ToString()).Year,
                            FPeriod = DateTime.Parse(headReader["录入时间"].ToString()).Month
                        };
                        headliList.Add(head);
                        recordIds.Add(headReader["id"].ToString());
                        Body body = new Body
                        {
                            FBillID = head.FBillID,
                            FReceiveAmount = head.FAmount,
                            FReceiveAmountFor = head.FAmount,
                            FReceiveExchangeRate = 1,
                            FSettleAmount = head.FAmount,
                            FSettleAmountFor = head.FAmount,
                            FReceiveCyID = 1,
                            FSettleCyID = 1,
                            FSettleExchangeRate = 1
                            //FDiscountFor = headReader["折扣金额"].ToString(),
                            //FDiscount = headReader["折扣金额"].ToString()
                        };
                        bodyliList.Add(body);
                        var rpContact = new NewReceiveBill.RPContact
                        {
                            FAmount=head.FAmount,
                            FAmountFor=head.FAmount,
                            FBillID=head.FBillID,
                            FCurrencyID=head.FCurrencyID,
                            FCustomer=head.FCustomer,
                            FDate=head.FDate,
                            FDepartment=head.FDepartment,
                            FEmployee=head.FEmployee,
                            FExchangeRate=head.FExchangeRate,
                            FFincDate=head.FFincDate,
                            FNumber=head.FNumber,
                            FPeriod= head.FDate.Month,
                            FRemainAmount = head.FAmount,
                            FRemainAmountFor = head.FAmount,
                            FRP=head.FRP,
                            FRPDate=head.FDate,
                            FYear=head.FDate.Year,
                            FType=5
                        };
                        contactList.Add(rpContact);
                        i++;
                        batchNum++;
                    }
                    DoBatch(number + i, headliList, bodyliList, contactList, recordIds);
                    return i;
                }
                catch (Exception ex)
                {
                    log4net.LogManager.GetLogger("logger").Error(ex.ToString());
                    throw;
                }
                finally
                {

                    headReader.Close();
                    conn.Close();
                }
            }

            private static int DoBatch(int number, List<NewReceiveBill.Head> heads, List<NewReceiveBill.Body> bodys,List<NewReceiveBill.RPContact> contracts, List<string> recordIds)
            {
                var headsqlstringlist = CommonFunction.GetSqlList(RelatedConn, heads, NewReceiveBill.Head.TableName);
                var bodysqlstringlist = CommonFunction.GetSqlList(RelatedConn, bodys, NewReceiveBill.Body.TableName);
                var contractsqlstringlist = CommonFunction.GetSqlList(RelatedConn, contracts, NewReceiveBill.RPContact.TableName);
                headsqlstringlist.AddRange(bodysqlstringlist);
                headsqlstringlist.AddRange(contractsqlstringlist);
                var resultnumber = SqlHelper.ExecuteSqlTran(RelatedConn, headsqlstringlist);
                SqlHelper.ExecuteSqlTran(SourceConn, new List<string>()
                                    { string.Format("update cmis_chufang_detail set kindeestate='1' where id in ({0})",
                                        string.Join(",",recordIds.Select(x=>"'"+x+"'"))) });
                CommonFunction.UpdateMaxNum(RelatedConn, NewReceiveBill.Head.TableName, number);
                return resultnumber;
            }
        }
        /// <summary>
        /// 其他出库单
        /// </summary>
        public class OtherOutboundBill
        {
            public class Head : OtherOutboundBills.Head
            {
                private string _ys;

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

            public class Body : OtherOutboundBills.Body
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

            public static int Work(string kstime, string jstime)
            {
                CommonFunction.Initalize(SourceConn, "cmis_mk_voucher_main2");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring = string.Format("select *,'1.'+CONVERT(char(5),子库房) as 仓库,21 as 领用部门,16398 as zhidanren,0 as chukuleixing from  cmis_mk_voucher_main2 where 操作时间>='{0}' and 操作时间<='{1}' and 业务类型=10 and kindeestate is null", kstime, jstime);
                var bodysqlstring = "select * from  cmis_mk_voucher_detail";
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
                        FDeptID = itemRow["领用部门"].ToString(),
                        FBillerID = itemRow["zhidanren"].ToString(),
                        FSupplyID = itemRow["接收科室"].ToString(),
                        FFManagerID = itemRow["操作人"].ToString(),
                        FSManagerID = itemRow["操作人"].ToString(),
                        FBillTypeID = itemRow["chukuleixing"].ToString(),
                        FInterID = number + i
                    };
                    headliList.Add(head);
                    recordlist.Add(string.Format("update cmis_mk_voucher_main2 set kindeestate='1' where 单据号='{0}'", itemRow["单据号"]));
                    var j = 1;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("单据号='{0}'", head.FBillNo)))
                    {
                        Body body = new Body
                        {
                            FItemID = bodyitemRow["药品ID"].ToString(),
                            FQty = bodyitemRow["数量"].ToString() == "" ? "0" : bodyitemRow["数量"].ToString(),
                            Fauxqty = bodyitemRow["数量"].ToString() == "" ? "0" : bodyitemRow["数量"].ToString(),
                            FUnitID = bodyitemRow["药库单位"].ToString(),
                            FDCStockID = itemRow["仓库"].ToString(),
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
