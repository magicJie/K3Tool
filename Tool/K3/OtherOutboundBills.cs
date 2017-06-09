namespace Tool.K3
{
    /// <summary>
    /// 其他出库单
    /// </summary>
    public class OtherOutboundBills
    {
        public class Head:ICStockBill
        {
            /// <summary>
            /// 部门字段
            /// </summary>
            protected string Fdeptid;
            /// <summary>
            /// 部门
            /// </summary>
            public string FDeptID { get { return Getfdeptid(); } set { Fdeptid = value; } }
            /// <summary>
            /// 获取部门
            /// </summary>
            /// <returns></returns>                       
            protected virtual string Getfdeptid()
            {
                return Fdeptid;
            }
            /// <summary>
            /// 购货单位字段
            /// </summary>
            protected string Fsupplyid;
            /// <summary>
            /// 购货单位
            /// </summary>
            public string FSupplyID { get { return GetFsupplyid(); } set { Fsupplyid = value; } }
            /// <summary>
            /// 获取购货单位
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFsupplyid()
            {
                return Fsupplyid;
            }
            /// <summary>
            /// 保管字段
            /// </summary>
            protected string Fsmanagerid;
            /// <summary>
            /// 保管
            /// </summary>
            public string FSManagerID { get { return GetFsmanagerid(); } set { Fsmanagerid = value; } }
            /// <summary>
            /// 获取保管
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFsmanagerid()
            {
                return Fsmanagerid;
            }
            /// <summary>
            /// 发货字段
            /// </summary>
            protected string Ffmanagerid;

            /// <summary>
            /// 保管
            /// </summary>
            public string FFManagerID
            {
                get { return GetFfmanagerid(); }
                set { Ffmanagerid = value; }
            }
            /// <summary>
            /// 获取发货
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFfmanagerid()
            {
                return Ffmanagerid;
            }
            public string FBillTypeID { get; set; }
            protected override int GetFTranType()
            {
               return 29;
            }
        }
        public class Body:ICStockBillEntry
        {
            /// <summary>
            /// 产品代码字段
            /// </summary>
            protected string Fitemid;
            /// <summary>
            /// 产品代码
            /// </summary>
            public string FItemID
            {
                get { return GetFItemId(); }
                set { Fitemid = value; }
            }
            /// <summary>
            /// 获取产品代码
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFItemId()
            {
                return Fitemid;
            }
            /// <summary>
            /// 实发数量
            /// </summary>
            public string Fauxqty { get; set; }
            /// <summary>
            /// 单位字段
            /// </summary>
            protected string Funitid;
            /// <summary>
            /// 单位
            /// </summary>
            public string FUnitID { get { return Getfunitid(); } set { Funitid = value; } }
            /// <summary>
            /// 获取单位
            /// </summary>
            /// <returns></returns>
            protected virtual string Getfunitid()
            {
                return Funitid;
            }
            /// <summary>
            /// 发货仓库字段
            /// </summary>
            protected string Fdcstockid;
            /// <summary>
            /// 发货仓库
            /// </summary>
            public string FDCStockID { get { return GetFdcstockid(); } set { Fdcstockid = value; } }
            /// <summary>
            /// 获取发货仓库
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFdcstockid()
            {
                return Fdcstockid;
            }
        }        
    }
}
