namespace Tool.K3
{
    /// <summary>
    /// 销售出库单
    /// </summary>
    public class SalesOutLet
    {        
        /// <summary>
        /// 表头
        /// </summary>
        public class Head:ICStockBill
        {            
            #region 字段
            /// <summary>
            /// 购货单位字段
            /// </summary>
            protected string Fsupplyid;
            /// <summary>
            /// 制单人字段
            /// </summary>
            protected string Fbillerid;
            /// <summary>
            /// 保管字段
            /// </summary>
            protected string Fsmanagerid;
            /// <summary>
            /// 发货字段
            /// </summary>
            protected string Ffmanagerid;
            /// <summary>
            /// 部门字段
            /// </summary>
            protected string Fdeptid;
            /// <summary>
            /// 销售方式字段
            /// </summary>
            protected int Fsalestyle;

            /// <summary>
            /// 业务员字段
            /// </summary>
            protected string Fempid;
            #endregion
            #region 属性                      
            /// <summary>
            /// 购货单位
            /// </summary>
            public string FSupplyID {get { return GetFsupplyid(); }set { Fsupplyid = value; }}
            /// <summary>
            /// 制单人
            /// </summary>
            public string FBillerID { get { return GetFbillerid(); } set { Fbillerid = value; } }

            /// <summary>
            /// 保管
            /// </summary>
            public string FSManagerID { get { return GetFsmanagerid(); } set { Fsmanagerid = value; } }


            /// <summary>
            /// 保管
            /// </summary>
            public string FFManagerID { get { return GetFfmanagerid(); } set { Ffmanagerid = value; } }

            /// <summary>
            /// 部门
            /// </summary>
            public string FDeptID { get { return Getfdeptid(); } set { Fdeptid = value; } }
            /// <summary>
            /// 销售方式
            /// </summary>
            public int FSaleStyle { get { return GetFsalestyle(); } set { Fsalestyle = value; } }

            /// <summary>
            /// 业务员
            /// </summary>
            public string FEmpID { get { return GetFempid(); } set { Fempid = value; } }
            #endregion
            #region 虚方法
            /// <summary>
            /// 获取购货单位
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFsupplyid()
            {
                return Fsupplyid;
            }
            /// <summary>
            /// 获取制单人
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFbillerid()
            {
                return Fbillerid;
            }
            /// <summary>
            /// 获取保管
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFsmanagerid()
            {
                return Fsmanagerid;
            }

            /// <summary>
            /// 获取发货
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFfmanagerid()
            {
                return Ffmanagerid;
            }
            /// <summary>
            /// 获取部门
            /// </summary>
            /// <returns></returns>
            protected virtual string Getfdeptid()
            {
                return Fdeptid;
            }
            /// <summary>
            /// 获取销售方式
            /// </summary>
            /// <returns></returns>
            protected virtual int GetFsalestyle()
            {
                return Fsalestyle;
            }
            /// <summary>
            /// 获取业务员
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFempid()
            {
                return Fempid;
            }
            #endregion
            #region 实现

            protected override int GetFTranType()
            {
                return 21;
            }

            #endregion
        }
        /// <summary>
        /// 表体
        /// </summary>
        public class Body:ICStockBillEntry
        {            
            #region 字段
            /// <summary>
            /// 产品代码字段
            /// </summary>
            protected string Fitemid;
            /// <summary>
            /// 单位字段
            /// </summary>
            protected string Funitid;
             /// <summary>
            /// 发货仓库字段
            /// </summary>
            protected string Fdcstockid;
            
           
           
            #endregion
            #region 属性           
            /// <summary>
            /// 产品代码
            /// </summary>
            public string FItemID
            {
                get { return GetFItemId(); }
                set { Fitemid = value; }
            }
            /// <summary>
            /// 销售单价
            /// </summary>
            public string FConsignPrice { get; set; }
            /// <summary>
            /// 销售金额
            /// </summary>
            public string FConsignAmount { get; set; }
            /// <summary>
            /// 实发数量
            /// </summary>
            public string Fauxqty { get; set; }
            /// <summary>
            /// 单位
            /// </summary>
            public string FUnitID { get { return Getfunitid(); } set { Funitid = value; } }
            /// <summary>
            /// 发货仓库
            /// </summary>
            public string FDCStockID { get { return GetFdcstockid(); } set { Fdcstockid = value; } }
           
            /// <summary>
            /// 基本单位发数量
            /// </summary>
            public string FQty { get; set; }
           
           
            #endregion
            #region 虚方法
            /// <summary>
            /// 获取产品代码
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFItemId()
            {
                return Fitemid;
            }
            /// <summary>
            /// 获取单位
            /// </summary>
            /// <returns></returns>
            protected virtual string Getfunitid()
            {
                return Funitid;
            }
            /// <summary>
            /// 获取发货仓库
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFdcstockid()
            {
                return Fdcstockid;
            }
                           
            #endregion
        }       
    }
}
