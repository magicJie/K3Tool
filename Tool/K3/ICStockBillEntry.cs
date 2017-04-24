namespace Tool.K3
{
    public class ICStockBillEntry
    {
        public static string TableName = "ICStockBillEntry";
        #region 属性
        /// <summary>
        /// FBrNo
        /// </summary>
        public int FBrNo
        {
            get { return 0; }
        }
        /// <summary>
        /// 关联主键
        /// </summary>
        public int FInterID { get; set; }
        /// <summary>
        /// 行号
        /// </summary>
        public int FEntryID { get; set; }
        #endregion
        #region 虚方法

        public virtual void GetICStockBillEntry(ICStockBillEntry icStockBillEntry)
        {
            
        }
        #endregion
    }
}
