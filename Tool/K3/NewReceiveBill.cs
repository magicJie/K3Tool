using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool.K3
{
    /// <summary>
    /// 收款单
    /// </summary>
    public class NewReceiveBill
    {
        /// <summary>
        /// 表头
        /// </summary>
        public class Head 
        {
            public static string TableName = "t_RP_NewReceiveBill";

            #region 字段
            /// <summary>
            /// 打印次数
            /// </summary>
            protected string Fbillerid;
            /// <summary>
            /// 单据号
            /// </summary>
            protected string Fdeptid;
            /// <summary>
            /// 单据日期
            /// </summary>
            protected string FsmanagerId;
            /// <summary>
            /// 财务日期
            /// </summary>
            protected string FfmanagerId;
            /// <summary>
            /// 汇率类型
            /// </summary>
            protected string FPurposeId;
            /// <summary>
            /// 币别
            /// </summary>
            protected string FPurposeId;
            /// <summary>
            /// 汇率
            /// </summary>
            protected string FPurposeId;
            /// <summary>
            /// 核算项目
            /// </summary>
            protected string FPurposeId;
            /// <summary>
            /// 单据金额
            /// </summary>
            protected string FPurposeId;
            /// <summary>
            /// 制单人
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
            public string FSManagerID { get { return GetFsManagerId(); } set { FsmanagerId = value; } }
            /// <summary>
            /// 发料人
            /// </summary>
            public string FFManagerID { get { return GetFfmanagerId(); } set { FfmanagerId = value; } }
            /// <summary>
            /// 领料类型
            /// </summary>
            public string FPurposeID { get { return GetFPurposeId(); } set { FPurposeId = value; } }

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
            #endregion
        }
        /// <summary>
        /// 表体
        /// </summary>
        public class Body
        {
            public static string TableName = "t_RP_Contact";

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
            public string FSCStockID { get { return GetFscstockid(); } set { FscStockId = value; } }

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
            protected virtual string GetFscstockid()
            {
                return FscStockId;
            }

            #endregion
        }
    }
}
