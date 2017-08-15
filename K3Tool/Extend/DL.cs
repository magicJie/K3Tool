﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Tool.K3;
using Tool.Sql;

namespace K3Tool.Extend
{
    /// <summary>
    /// 大连医卫
    /// </summary>
    public class Dl
    {
        public static string RelatedConn = ConfigurationManager.ConnectionStrings["dlrelated"].ToString();
        public static string SourceConn = ConfigurationManager.ConnectionStrings["dlsource"].ToString();
        private static log4net.ILog _logger = log4net.LogManager.GetLogger("logger");
        /// <summary>
        /// 外购入库单
        /// </summary>
        public class DlPurchasedWarehouse
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
                    return "";
                    //var filter = string.Format("FNumber='{0}'", Fdcstockid);
                    //return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.仓库, filter);
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
                if (!SqlHelper.TestConnection(SourceConn))
                {
                    log4net.LogManager.GetLogger("logger").Error($"提供数据库链接【{SourceConn}】连接失败！");
                    throw new Exception($"提供数据库链接【{SourceConn}】连接失败！");
                }
                CommonFunction.Initalize(SourceConn, "T_Mat_KFPurchase");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring = 
                    string.Format(@"select a.FBillNo 单据号,a.FBillDate 操作时间,a.FSupplierID 供应商,c.FNumber 仓库,b.FNumber 操作人,a.FKFPurchaseID 表头ID
                                    from T_Mat_KFPurchase a
                                    left join T_Sys_User b on b.FUserID = a.FBillUserID
                                    left join T_Sys_Group c on c.FGroupID = a.FKFGroupID
                                    where kindeestate is null and a.FBillDate>='{0}' and a.FBillDate<='{1}'",
                                    kstime, jstime);
                var bodysqlstring = @"select b.FNumber 药品ID,c.FNumber 仓库,a.FQuantity 数量,a.FPurchaseAmt 进货总价,a.FPurchasePrice 进货单价,a.FKFUnit 药库单位,a.FKFPurchaseID 表头ID
                                      from T_Mat_KFPurchaseDetail a 
                                      left join T_Biz_MedSpec b on b.FFeeItemID=a.FFeeItemID 
                                      left join T_Sys_Group c on c.FGroupID = a.FKFGroupID";
                var headtable = SqlHelper.Query(SourceConn, headsqlstring, true);
                var bodytable = SqlHelper.Query(SourceConn, bodysqlstring);
                var i = 1;
                var number = CommonFunction.GetMaxNum(RelatedConn, ICStockBill.TableName);
                foreach (DataRow itemRow in headtable.Rows)
                {
                    i = i + 1;
                    Head head = new Head
                    {
                        FBillNo = itemRow["仓库"].ToString().Substring(4,3)+itemRow["单据号"],//大连医卫设计是同一个药库的BillNo不重复
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
                    recordlist.Add(string.Format("update T_Mat_KFPurchase set kindeestate='1' where FKFPurchaseID='{0}'", itemRow["表头ID"]));
                    var j = 1;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("表头ID='{0}'", itemRow["表头ID"])))
                    {
                        if (bodyitemRow["药品ID"].ToString() == "")
                        {
                            continue;
                        }
                        Body body = new Body
                        {
                            FItemID = bodyitemRow["药品ID"].ToString(),
                            FInterID = head.FInterID,
                            FQty = bodyitemRow["数量"].ToString(),
                            Fauxqty = bodyitemRow["数量"].ToString(),
                            Famount = bodyitemRow["进货总价"].ToString(),
                            Fauxprice = bodyitemRow["进货单价"].ToString(),
                            FDCStockID = bodyitemRow["仓库"].ToString(),
                            FUnitID = bodyitemRow["药库单位"].ToString(),
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
        /// 退货单
        /// </summary>
        public class DlRefund
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
                CommonFunction.Initalize(SourceConn, "T_Mat_KFPurchase");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring =
                    string.Format(@"select a.FBillNo 单据号,a.FBillDate 操作时间,a.FSupplierID 供应商,b.FNumber 操作人,c.FNumber 仓库 ,a.FKFRefundID 表头ID
                                    from T_Mat_KFRefund a
                                    left join T_Sys_User b on b.FUserID = a.FBillUserID
                                    left join T_Sys_Group c on c.FGroupID = a.FKFGroupID 
                                    where kindeestate is null and ,a.FBillDate>='{0}' and ,a.FBillDate<='{1}'", kstime, jstime);
                var bodysqlstring = @"select b.FNumber 药品ID,c.FNumber 仓库,a.FQuantity 数量,a.FPurchaseAmt 进货总价,a.FPurchasePrice 进货单价,a.FKFUnit 药库单位,a.FKFRefundID 表头ID
                                      from T_Mat_KFRefundDetail a 
                                      left join T_Biz_MedSpec b on b.FItemID=a.FFeeItemID 
                                      left join T_Sys_Group c on c.FGroupID = a.FKFGroupID";
                var headtable = SqlHelper.Query(SourceConn, headsqlstring, true);
                var bodytable = SqlHelper.Query(SourceConn, bodysqlstring);
                var i = 0;
                var number = CommonFunction.GetMaxNum(RelatedConn, ICStockBill.TableName);
                foreach (DataRow itemRow in headtable.Rows)
                {
                    i = i + 1;
                    Head head = new Head
                    {
                        FBillNo = itemRow["仓库"].ToString().Substring(4, 3) + itemRow["单据号"],
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
                    recordlist.Add(string.Format("update T_Mat_KFPurchase set kindeestate='1' where FKFRefundID='{0}'", itemRow["表头ID"]));
                    var j = 1;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("表头ID='{0}'", itemRow["表头ID"])))
                    {
                        if (bodyitemRow["药品ID"].ToString() == "")
                        {
                            continue;
                        }
                        Body body = new Body
                        {
                            FItemID = bodyitemRow["药品ID"].ToString(),
                            FInterID = head.FInterID,
                            FQty = bodyitemRow["数量"].ToString(),
                            Fauxqty = bodyitemRow["数量"].ToString(),
                            Famount = bodyitemRow["进货总价"].ToString(),
                            Fauxprice = bodyitemRow["进货单价"].ToString(),
                            FDCStockID = bodyitemRow["仓库"].ToString(),
                            FUnitID = bodyitemRow["药库单位"].ToString(),
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
        /// 生产领料单
        /// </summary>
        public class DlPicking
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
                CommonFunction.Initalize(SourceConn, "T_Biz_Pharmaceutical");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring = string.Format(
                                    @"select a.FPharmaceuticalID 单据号,a.FCreateDate 操作时间,b.FNumber 操作人
                                    from T_Biz_Pharmaceutical a
                                    left join T_Sys_User b on b.FUserID = a.FCreateUserID
                                    where  kindeestate is null and a.FCreateDate>='{0}' and a.FCreateDate<='{1}'",kstime, jstime);
                var bodysqlstring = @"select b.FNumber 药品ID,a.FQuantity 数量,a.FPurchaseAmt 进货总价,a.FPurchasePrice 进货单价,a.FKFUnit 药库单位,a.FPharmaceuticalID 单据号
                                    from T_Biz_PharmaceuticalDetail a
                                    left join T_Biz_Item b on b.FItemID=a.FFeeItemID";
                var headtable = SqlHelper.Query(SourceConn, headsqlstring, true);
                var bodytable = SqlHelper.Query(SourceConn, bodysqlstring);
                var i = 0;
                var number = CommonFunction.GetMaxNum(RelatedConn, ICStockBill.TableName);
                foreach (DataRow itemRow in headtable.Rows)
                {
                    i = i + 1;
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
                    recordlist.Add(string.Format("update T_Biz_Pharmaceutical set kindeestate='1' where 单据号='{0}'", head.FBillNo));
                    var j = 1;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("单据号='{0}'", head.FBillNo)))
                    {
                        if (bodyitemRow["药品ID"].ToString() == "")
                        {
                            continue;
                        }
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
        public class DlSalesOutLet
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
                    set { _ys = value; }
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
            public static int Work(string kstime, string jstime)
            {
                CommonFunction.Initalize(SourceConn, "cmis_chufang_detail");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring = string.Format("select *,(select FNumber from [T_Sys_User] where FUserID=a.FCreateUserID) as 操作人,FCreateDate as 操作时间,(select Fnumber from T_Bd_Manufacturer where FManufacturerID=a.FManufacturerID) as 交货单位 from T_Biz_Pharmaceutical a where a.kindeestate is null and a.操作时间>='{0}' and a.操作时间<='{1}'", kstime, jstime);
                var bodysqlstring = "select *,FFeeItemID as 药品id from T_Biz_PharmaceuticalDetail";
                var headtable = SqlHelper.Query(SourceConn, headsqlstring, true);
                var bodytable = SqlHelper.Query(SourceConn, bodysqlstring);
                var i = 0;
                var number = CommonFunction.GetMaxNum(RelatedConn, ICStockBill.TableName);
                foreach (DataRow itemRow in headtable.Rows)
                {
                    i = i + 1;
                    Head head = new Head
                    {
                        FBillNo = itemRow["处方号"].ToString(),
                        Fdate = DateTime.Parse(itemRow["录入时间"].ToString()),
                        FDeptID = itemRow["科室id"].ToString(),
                        FBillerID = itemRow["录入人"].ToString(),
                        FEmpID = itemRow["录入人"].ToString(),
                        //FSupplyID = itemRow["科室id"].ToString(),
                        FHeadSelfB0154 = itemRow["医生id"].ToString(),
                        FInterID = number + i
                    };
                    headliList.Add(head);
                    recordlist.Add(string.Format("update T_Biz_Pharmaceutical set kindeestate='1' where 单据号='{0}'", itemRow["单据号"]));
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
                            body.FQty = (Convert.ToDouble(body.FQty) / 1000).ToString(CultureInfo.InvariantCulture);
                        }
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
        /// 产品入库单
        /// </summary>
        public class DlProductInventory
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
                var headsqlstring = string.Format("select 单据号,'1.'+CONVERT(char(5),子库房) as 仓库,20 as 交货单位,操作时间,操作人 from cmis_mk_voucher_main2 where 业务类型='2' and kindeestate is null and 操作时间>='{0}' and 操作时间<='{1}'", kstime, jstime);
                var bodysqlstring = "select 单据号,药品ID,药库单位,数量,进货单价,进货总价 from cmis_mk_voucher_detail";
                var headtable = SqlHelper.Query(SourceConn, headsqlstring, true);
                var bodytable = SqlHelper.Query(SourceConn, bodysqlstring);
                var i = 0;
                var number = CommonFunction.GetMaxNum(RelatedConn, ICStockBill.TableName);
                foreach (DataRow itemRow in headtable.Rows)
                {
                    i = i + 1;
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
        /// 收款单
        /// </summary>
        public class DlNewReceiveBill
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
                        throw new Exception("收款类型" + _FBillType + "找不到");
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
            public static int Work(string kstime, string jstime)
            {
                CommonFunction.Initalize(SourceConn, "cmis_chufang_detail");
                var headliList = new List<NewReceiveBill.Head>();
                var bodyliList = new List<NewReceiveBill.Body>();
                var contactList = new List<NewReceiveBill.RPContact>();
                var headsqlstring = String.Format(@"select id,处方号,科室id,医生id,处方类型,总价格,录入人,录入时间,'耿惠平' as 制单人,'客户' as 客户,处方类型,折扣金额 
                                      from  cmis_chufang_detail where 录入时间>='{0}' and 录入时间<='{1}' and 处方类型 in (3,5,6,8,9,10,15) and kindeestate is null", kstime, jstime);
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
                            FItemClassID = 1,
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
                            FBillType = int.Parse(headReader["处方类型"].ToString()),
                            FClassTypeID = "1000005",
                            FReceiveAmount = totalAmount.ToString(),
                            FReceiveAmountFor = totalAmount.ToString(),
                            FSettleAmount = totalAmount.ToString(),
                            FSettleAmountFor = totalAmount.ToString(),
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
                            FAmount = head.FAmount,
                            FAmountFor = head.FAmount,
                            FBillID = head.FBillID,
                            FCurrencyID = head.FCurrencyID,
                            FCustomer = head.FCustomer,
                            FDate = head.FDate,
                            FDepartment = head.FDepartment,
                            FEmployee = head.FEmployee,
                            FExchangeRate = head.FExchangeRate,
                            FFincDate = head.FFincDate,
                            FNumber = head.FNumber,
                            FPeriod = head.FDate.Month,
                            FRemainAmount = head.FAmount,
                            FRemainAmountFor = head.FAmount,
                            FRP = head.FRP,
                            FRPDate = head.FDate,
                            FYear = head.FDate.Year,
                            FType = 5
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

            private static int DoBatch(int number, List<NewReceiveBill.Head> heads, List<NewReceiveBill.Body> bodys, List<NewReceiveBill.RPContact> contracts, List<string> recordIds)
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
        public class DlOtherOutboundBill
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
                var headsqlstring = string.Format("select * from  T_Mat_KFExport where 操作时间>='{0}' and 操作时间<='{1}' and kindeestate is null", kstime, jstime);
                var bodysqlstring = "select * from  T_Mat_KFExport";
                var headtable = SqlHelper.Query(SourceConn, headsqlstring, true);
                var bodytable = SqlHelper.Query(SourceConn, bodysqlstring);
                var i = 0;
                var number = CommonFunction.GetMaxNum(RelatedConn, ICStockBill.TableName);
                foreach (DataRow itemRow in headtable.Rows)
                {
                    i = i + 1;
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
                    recordlist.Add(string.Format("update T_Mat_KFExport set kindeestate='1' where 单据号='{0}'", itemRow["单据号"]));
                    var j = 1;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("单据号='{0}'", head.FBillNo)))
                    {
                        Body body = new Body
                        {
                            FItemID = bodyitemRow["药品ID"].ToString(),
                            Fauxqty = bodyitemRow["数量"].ToString() == "" ? "0" : bodyitemRow["数量"].ToString(),
                            FUnitID = bodyitemRow["药库单位"].ToString(),
                            FDCStockID = itemRow["仓库"].ToString(),
                            FInterID = head.FInterID,
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
