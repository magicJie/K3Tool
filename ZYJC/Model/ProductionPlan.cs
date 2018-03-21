using System;

namespace ZYJC.Model
{
    /// <summary>
    /// 生产计划
    /// </summary>   
    public class ProductionPlan : BaseModel
    {
        public string PlanCode { set; get; }
        public string WorkOrder { set; get; }
        public string MaterielCode { set; get; }
        public string Planner { set; get; }
        [DbType(DbDataType = "TimeStamp")]
        public DateTime BillDate { set; get; }
        public string BOMCode { set; get; }
        public string BOMVersion { set; get; }
        public string OrderState { set; get; }
        public double PlanQuantity { set; get; }
        public string BaseUnit { set; get; }
        public string ProductionType { set; get; }
        [DbType(DbDataType = "TimeStamp")]
        public DateTime PlanStartTime { set; get; }
        [DbType(DbDataType = "TimeStamp")]
        public DateTime PlanEndTime { set; get; }
        public string WorkShop { set; get; }
        public string Line { set; get; }
    }
}
