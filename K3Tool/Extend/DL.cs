using System;
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
        public static string RelatedConn = ConfigurationManager.ConnectionStrings["dlrelatedtest"].ToString();
        public static string SourceConn = ConfigurationManager.ConnectionStrings["dlsourcetest"].ToString();
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
                    var filter = string.Format("FName='关章瑛'");
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.登录用户, filter);
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
                if (!SqlHelper.TestConnection(SourceConn))
                {
                    log4net.LogManager.GetLogger("logger").Error(string.Format("提供数据库链接【{SourceConn}】连接失败！"));
                    throw new Exception(string.Format("提供数据库链接【{SourceConn}】连接失败！"));
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
                    var filter = string.Format("FName='关章瑛'");
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.登录用户, filter);
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
                CommonFunction.Initalize(SourceConn, "T_Mat_KFRefund");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring =
                    string.Format(@"select a.FBillNo 单据号,a.FBillDate 操作时间,a.FSupplierID 供应商,b.FNumber 操作人,c.FNumber 仓库 ,a.FKFRefundID 表头ID
                                    from T_Mat_KFRefund a
                                    left join T_Sys_User b on b.FUserID = a.FBillUserID
                                    left join T_Sys_Group c on c.FGroupID = a.FKFGroupID 
                                    where kindeestate is null and a.FBillDate>='{0}' and a.FBillDate<='{1}'", kstime, jstime);
                var bodysqlstring = @"select b.FNumber 药品ID,c.FNumber 仓库,a.FQuantity 数量,a.FPurchaseAmt 进货总价,a.FPurchasePrice 进货单价,a.FKFUnit 药库单位,a.FKFRefundID 表头ID
                                      from T_Mat_KFRefundDetail a 
                                      left join T_Biz_MedSpec b on b.FFeeItemID=a.FFeeItemID 
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
                    var filter = string.Format("FName='关章瑛'");
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.登录用户, filter);
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
                var bodysqlstring = @"select b.FNumber 药品ID,a.FQuantity 数量,a.FKFUnit 药库单位,a.FPharmaceuticalID 单据号
                                    from T_Biz_PharmaceuticalDetail a
                                    left join T_Biz_MedSpec b on b.FFeeItemID=a.FFeeItemID ";
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
                            //FSCStockID = itemRow["仓库"].ToString(),
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
                    var filter = string.Format("FName='关章瑛'");
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.登录用户, filter);
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
                CommonFunction.Initalize(SourceConn, "T_Mat_DepImport");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring = string.Format("select FDepImportID,FBillNo as 单据号,FCreateDate as 操作时间,FBillUserID as 操作人,20 as 交货单位 from T_Mat_DepImport where 业务类型='2' and kindeestate is null and 操作时间>='{0}' and 操作时间<='{1}'", kstime, jstime);
                var bodysqlstring = "select FDepImportID,FFeeItemID as 药品ID,FKFUnit as 药库单位,FQuantity as 数量,FPurchasePrice as 进货单价,FPurchaseAmt as 进货总价 from T_Mat_DepImportDetail";
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
                    recordlist.Add(string.Format("update T_Mat_DepImport set kindeestate='1' where FDepImportID='{0}'", itemRow["FDepImportID"]));
                    var j = 1;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("FDepImportID='{0}'", itemRow["FDepImportID"])))
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

                //protected override string Getfdeptid()
                //{
                //    var filter = string.Format("FNumber='{0}'", Fdeptid);
                //    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.部门, filter);
                //}

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
                    var filter = string.Format("FName='关章瑛'");
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.登录用户, filter);
                }

                protected override string GetFsupplyid()
                {
                    var filter = string.Format("FName='{0}'", "客户");
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.客户, filter);
                }

                protected override string GetFempid()
                {
                    var filter = string.Format("FNumber='{0}'", Fempid);
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.职员, filter);
                }
                /// <summary>
                /// 医师
                /// </summary>
                //public string FHeadSelfB0154
                //{
                //    get
                //    {
                //        var filter = string.Format("FNumber='{0}'", _ys);
                //        return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.医师, filter);
                //    }
                //    set { _ys = value; }
                //}
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
                CommonFunction.Initalize(SourceConn, "T_Med_MZSale");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring = string.Format(@"select a.FBillNo as 处方号,a.FBillDate as 录入时间,b.FNumber 录入人,a.FMZSaleID as 主键 from T_Med_MZSale a
                                         left join T_Sys_User b on b.FUserID = a.FBillUserID 
                                         where a.kindeestate is null and a.FBillDate>='{0}' and a.FBillDate<='{1}'", kstime, jstime);
                var bodysqlstring = @"select b.FNumber as 收费项目id,FQuantity as 实发数量,a.FUnit as 最小单位,a.FCheckPrice as 新单价,a.FCheckAmt as 新总价格,c.FNumber as 发药库房,a.FMZSaleID as 主键 from T_Med_MZSaleDetail a
                                     left join T_Biz_MedSpec b on b.FFeeItemID=a.FFeeItemID
                                     left join T_Sys_Group c on c.FGroupID = a.FSendGroupID";
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
                        //FDeptID = itemRow["科室id"].ToString(),
                        FEmpID = itemRow["录入人"].ToString(),
                        //FSupplyID = itemRow["科室id"].ToString(),
                        //FHeadSelfB0154 = itemRow["医生id"].ToString(),
                        FInterID = number + i
                    };
                    headliList.Add(head);
                    recordlist.Add(string.Format("update T_Med_MZSale set kindeestate='1' where FMZSaleID='{0}'", itemRow["主键"]));
                    var j = 1;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("主键='{0}'", itemRow["主键"])))
                    {
                        Body body = new Body
                        {
                            FItemID = bodyitemRow["收费项目id"].ToString(),
                            FQty = bodyitemRow["实发数量"].ToString() == "" ? "0" : bodyitemRow["实发数量"].ToString(),
                            Fauxqty = bodyitemRow["实发数量"].ToString() == "" ? "0" : bodyitemRow["实发数量"].ToString(),
                            FUnitID = bodyitemRow["最小单位"].ToString(),
                            FConsignPrice = bodyitemRow["新单价"].ToString() == "" ? "0" : bodyitemRow["新单价"].ToString(),
                            FConsignAmount = bodyitemRow["新总价格"].ToString() == "" ? "0" : bodyitemRow["新总价格"].ToString(),
                            FDCStockID = bodyitemRow["发药库房"].ToString(),
                            FInterID = head.FInterID,
                            FEntryID = j
                        };
                        //if (bodyitemRow["最小单位"].ToString() == "g" && bodyitemRow["单位"].ToString() == "kg")
                        //{
                        //    body.FQty = (Convert.ToDouble(body.FQty) / 1000).ToString(CultureInfo.InvariantCulture);
                        //}
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
        /// 其他出库单
        /// </summary>
        public class DlOtherOutboundBill
        {
            public class Head : OtherOutboundBills.Head
            {
                private string _ys;

                //protected override string Getfdeptid()
                //{
                //    var filter = string.Format("FNumber='{0}'", Fdeptid);
                //    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.部门, filter);
                //}

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
                    var filter = string.Format("FName='关章瑛'");
                    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.登录用户, filter);
                }

                //protected override string GetFsupplyid()
                //{
                //    var filter = string.Format("FNumber='{0}'", Fsupplyid);
                //    return CommonFunction.Getfitemid(RelatedConn, Fitemclassid.客户, filter);
                //}
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
                CommonFunction.Initalize(SourceConn, "T_Mat_KFExport");
                var headliList = new List<ICStockBill>();
                var bodyliList = new List<ICStockBillEntry>();
                var recordlist = new List<string>();
                var headsqlstring = string.Format(@"select  a.FBillNo as 单据号,a.FBillDate as 操作时间,a.FAppGroupID as 领用部门,c.FNumber as 接收科室,b.FNumber as zhidanren,0 as chukuleixing,a.FKFExportID as 主键 from T_Mat_KFExport a
                                                    left join T_Sys_User b on b.FUserID = a.FBillUserID
                                                    left join T_Sys_Group c on c.FGroupID = a.FKFGroupID where a.FBillDate>='{0}' and a.FBillDate<='{1}' and a.kindeestate is null", kstime, jstime);
                var bodysqlstring = @"select b.FNumber as 药品ID,a.FExpQuanlity as 数量,a.FUnit as 药库单位,c.FNumber as 仓库,a.FKFExportID as 主键 from T_Mat_KFExportDetail as a
                                   left join T_Biz_MedSpec b on b.FFeeItemID=a.FFeeItemID 
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
                        FBillNo = itemRow["单据号"].ToString(),
                        Fdate = DateTime.Parse(itemRow["操作时间"].ToString()),
                        //FDeptID = itemRow["领用部门"].ToString(),
                        //FSupplyID = itemRow["接收科室"].ToString(),
                        FFManagerID = itemRow["zhidanren"].ToString(),
                        FSManagerID = itemRow["zhidanren"].ToString(),
                        FBillTypeID = itemRow["chukuleixing"].ToString(),
                        FInterID = number + i
                    };
                    headliList.Add(head);
                    recordlist.Add(string.Format("update T_Mat_KFExport set kindeestate='1' where FKFExportID='{0}'", itemRow["主键"]));
                    var j = 1;
                    foreach (DataRow bodyitemRow in bodytable.Select(string.Format("主键='{0}'", itemRow["主键"])))
                    {
                        if (bodyitemRow["药品ID"].ToString() == "")
                        {
                            continue;
                        }
                        Body body = new Body
                        {
                            FItemID = bodyitemRow["药品ID"].ToString(),
                            Fauxqty = bodyitemRow["数量"].ToString() == "" ? "0" : bodyitemRow["数量"].ToString(),
                            FUnitID = bodyitemRow["药库单位"].ToString(),
                            FDCStockID = bodyitemRow["仓库"].ToString(),
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

        //盘盈单

        //盘亏单
    }
}
