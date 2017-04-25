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
        /// <summary>
        /// 制单人字段
        /// </summary>
        protected string Fbillerid;
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
        /// 制单人
        /// </summary>
        public string FBillerID { get { return GetFbillerid(); } set { Fbillerid = value; } }
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
        /// <summary>
        /// 获取制单人
        /// </summary>
        /// <returns></returns>
        protected virtual string GetFbillerid()
        {
            return Fbillerid;
        }
        #endregion
    }
}
