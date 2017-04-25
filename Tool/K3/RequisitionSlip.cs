using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool.K3
{
    /// <summary>
    /// 调拨单
    /// </summary>
    public class RequisitionSlip
    {
        /// <summary>
        /// 表头
        /// </summary>
        public class Head : ICStockBill
        {
            #region 字段                     
            /// <summary>
            /// 保管字段
            /// </summary>
            protected string Fsmanagerid;          
            /// <summary>
            /// 验收人字段
            /// </summary>
            protected string Ffmanagerid;                
            #endregion
            #region 属性                      
            /// <summary>
            /// 保管
            /// </summary>
            public string FSManagerID { get { return GetFsmanagerid(); } set { Fsmanagerid = value; } }            
            /// <summary>
            /// 验收人
            /// </summary>
            public string FFManagerID { get { return GetFfmanagerid(); } set { Ffmanagerid = value; } }                     
            #endregion
            #region 虚方法                     
            /// <summary>
            /// 获取保管
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFsmanagerid()
            {
                return Fsmanagerid;
            }           
            /// <summary>
            /// 获取验收人
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFfmanagerid()
            {
                return Ffmanagerid;
            }              
            #endregion
            #region 方法实现

            protected override int GetFTranType()
            {
                return 41;
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
            /// 物料编码字段
            /// </summary>
            protected string Fitemid;
            /// <summary>
            /// 单位字段
            /// </summary>
            protected string Funitid;
            /// <summary>
            /// 调入仓库字段
            /// </summary>
            protected string Fdcstockid;
            /// <summary>
            /// 调出仓库字段
            /// </summary>
            protected string Fscstockid;
            #endregion
            #region 属性

            /// <summary>
            /// 物料编码
            /// </summary>
            public string FItemID { get { return GetFItemId(); } set { Fitemid = value; } }
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
            /// 调入仓库
            /// </summary>
            public string FDCStockID { get { return GetFdcstockid(); } set { Fdcstockid = value; } }
            /// <summary>
            /// 调出仓库
            /// </summary>
            public string FSCStockID { get { return GetFscstockid(); } set { Fscstockid = value; } }
            /// <summary>
            /// 基本单位实收数量
            /// </summary>
            public string FQty { get; set; }
            /// <summary>
            /// 调拨金额
            /// </summary>
            public string FAmtRef { get; set; }
            /// <summary>
            /// 调拨单价
            /// </summary>
            public string FAuxPriceRef { get; set; }

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
            /// 获取调入仓库
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFdcstockid()
            {
                return Fdcstockid;
            }
            /// <summary>
            /// 获取调入仓库
            /// </summary>
            /// <returns></returns>
            protected virtual string GetFscstockid()
            {
                return Fscstockid;
            }
            #endregion
        }       
    }
}
