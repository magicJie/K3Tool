namespace Tool.K3
{
    /// <summary>
    /// 外购入库单
    /// </summary>
    public class PurchasedWarehouse
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
            /// 收料仓库字段
            /// </summary>
            protected string Fdcstockid;
            /// <summary>
            /// 保管字段
            /// </summary>
            protected string Fsmanagerid;
            /// <summary>
            /// 业务员字段
            /// </summary>
            protected string Fempid;
            /// <summary>
            /// 验收人字段
            /// </summary>
            protected string Ffmanagerid;
            /// <summary>
            /// 采购方式字段
            /// </summary>
            protected string Fpostyle;
            /// <summary>
            /// 制单人字段
            /// </summary>
            protected string Fbillerid;
            #endregion
            #region 属性                      
            /// <summary>
            /// 供应商
            /// </summary>
            public string FSupplyID
            {
                get { return GetFsupplyid(); }
                set { Fsupplyid = value; }
            }
            /// <summary>
            /// 收料仓库
            /// </summary>
            public string FDCStockID { get { return GetFdcstockid(); } set { Fdcstockid = value; } }
            /// <summary>
            /// 保管
            /// </summary>
            public string FSManagerID { get { return GetFsmanagerid(); } set { Fsmanagerid = value; } }
            /// <summary>
            /// 业务员
            /// </summary>
            public string FEmpID { get { return GetFempid(); } set { Fempid = value; } }
            /// <summary>
            /// 验收人
            /// </summary>
            public string FFManagerID { get { return GetFfmanagerid(); } set { Ffmanagerid = value; } }
            /// <summary>
            /// 采购方式
            /// </summary>
            public string FPOStyle { get { return GetFpostyle(); } set { Fpostyle = value; } }
            /// <summary>
            /// 制单人
            /// </summary>
            public string FBillerID { get { return GetFbillerid(); } set { Fbillerid = value; } }
            #endregion
            #region 虚方法
            /// <summary>
            /// 获取供应商
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFsupplyid()
            {
                return Fsupplyid;
            }
            /// <summary>
            /// 获取收料仓库
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFdcstockid()
            {
                return Fdcstockid;
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
            /// 获取业务员
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFempid()
            {
                return Fempid;
            }
            /// <summary>
            /// 获取验收人
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFfmanagerid()
            {
                return Ffmanagerid;
            }
            /// <summary>
            /// 获取采购方式
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFpostyle()
            {
                return Fpostyle;
            }
            /// <summary>
            /// 获取制单人
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFbillerid()
            {
                return Fbillerid;
            }
            #endregion
            #region 方法实现

            protected override int GetFTranType()
            {
                return 1;
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
            /// 物料编码字段
            /// </summary>
            protected string Fitemid;
            /// <summary>
            /// 单位字段
            /// </summary>
            protected string Funitid;
            /// <summary>
            /// 收料仓库字段
            /// </summary>
            protected string Fdcstockid;                        
            #endregion
            #region 属性
           
            /// <summary>
            /// 物料编码
            /// </summary>
            public string FItemID {get { return GetFItemId(); }set { Fitemid = value; }}
            /// <summary>
            /// 实收数量
            /// </summary>
            public string Fauxqty { get; set; }
            /// <summary>
            /// 金额
            /// </summary>
            public string Famount { get; set; }
            /// <summary>
            /// 单价
            /// </summary>
            public string Fauxprice { get; set; }
            /// <summary>
            /// 单位
            /// </summary>
            public string FUnitID { get { return GetFunitid(); } set { Funitid = value; } }
            /// <summary>
            /// 收料仓库
            /// </summary>
            public string FDCStockID { get { return GetFdcstockid(); } set { Fdcstockid = value; } }
           
            /// <summary>
            /// 基本单位实收数量
            /// </summary>
            public string FQty { get; set; }
           
            #endregion
            #region 虚方法
            /// <summary>
            /// 获取物料编码
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
            protected virtual string GetFunitid()
            {
                return Funitid;
            }  
            /// <summary>
            /// 获取收料仓库
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
