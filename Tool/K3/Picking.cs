using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool.K3
{
    /// <summary>
    /// 领料单
    /// </summary>
    public class Picking
    {
        /// <summary>
        /// 表头
        /// </summary>
        public class Head : ICStockBill
        {
            #region 字段          
            /// <summary>
            /// 制单人字段
            /// </summary>
            protected string Fbillerid;
            /// <summary>
            /// 部门字段
            /// </summary>
            protected string Fdeptid;
            /// <summary>
            /// 领料人字段
            /// </summary>
            protected string FsmanagerId;
            /// <summary>
            /// 发料人字段
            /// </summary>
            protected string FfmanagerId;
            /// <summary>
            /// 领料类型字段
            /// </summary>
            protected string FPurposeId;
            #endregion
            #region 属性           
            /// <summary>
            /// 制单人
            /// </summary>
            public string FBillerID { get { return GetFbillerid(); } set { Fbillerid = value; } }
            /// <summary>
            /// 部门
            /// </summary>
            public string FDeptID { get { return Getfdeptid(); } set { Fdeptid = value; } }
            /// <summary>
            /// 领料人
            /// </summary>
            public string FSManagerID {get { return GetFsManagerId(); }set { FsmanagerId = value; }}
            /// <summary>
            /// 发料人
            /// </summary>
            public string FFManagerID{get { return GetFfmanagerId(); }set { FfmanagerId = value; }}
            /// <summary>
            /// 领料类型
            /// </summary>
            public string FPurposeID {get { return GetFPurposeId(); }set { FPurposeId = value; }}

            #endregion
            #region 虚方法 
            /// <summary>
            /// 获取制单人
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFbillerid()
            {
                return Fbillerid;
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
            /// 获取领料人
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFsManagerId()
            {
                return FsmanagerId;
            }
            /// <summary>
            /// 获取发料人
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFfmanagerId()
            {
                return FfmanagerId;
            }
            /// <summary>
            /// 获取领料类型
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFPurposeId()
            {
                return FPurposeId;
            }
            #endregion
            #region 实现

            protected override int GetFTranType()
            {
                return 24;
            }

            #endregion
        }
        /// <summary>
        /// 表体
        /// </summary>
        public class Body : ICStockBillEntry
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
            /// 发料仓库字段
            /// </summary>
            protected string FscStockId;
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
            /// 实发数量
            /// </summary>
            public string Fauxqty { get; set; }
            /// <summary>
            /// 单位
            /// </summary>
            public string FUnitID { get { return Getfunitid(); } set { Funitid = value; } }
            /// <summary>
            /// 发料仓库
            /// </summary>
            public string FSCStockID { get { return GetFdcstockid(); } set { FscStockId = value; } }

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
            /// 获取发料仓库
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFdcstockid()
            {
                return FscStockId;
            }

            #endregion
        }       
    }
}
