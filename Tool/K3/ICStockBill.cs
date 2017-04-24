using System;

namespace Tool.K3
{
    public class ICStockBill
    {
        public static string TableName = "ICStockBill";
        #region 字段
        /// <summary>
        /// 单据类型字段
        /// </summary>
        protected int Ftrantype;
        #endregion
        #region 属性
        /// <summary>
        /// FBrNo
        /// </summary>
        public int FBrNo
        {
            get { return 0; }
        } /// <summary>
        /// 编号
        /// </summary>
        public string FBillNo { get; set; }
        /// <summary>
        /// 关联主键
        /// </summary>
        public int FInterID { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Fdate { get; set; }
        /// <summary>
        /// 单据类型
        /// </summary>
        public int FTranType { get { return GetFTranType(); } set { Ftrantype = value; } }
        #endregion      
        #region  虚方法
        /// <summary>
        /// 获取单据类型
        /// </summary>
        /// <returns></returns>
        protected virtual int GetFTranType()
        {
            return Ftrantype;
        }
        #endregion
    }
}
