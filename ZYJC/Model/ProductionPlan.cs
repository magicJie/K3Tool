using System;

namespace ZYJC.Model
{
    /// <summary>
    /// 生产计划
    /// </summary>   
    public class ProductionPlan : BaseModel
    {
        public string PlanCode;
        public string WorkOrder;
        public string MaterielCode;
        public string Planner;
        public string BillDate;
        public string BOMCode;
        public string BOMVersion;
        public string OrderState;
        public string PlanQuantity;
        public string BaseUnit;
        public string ProductionType;
        public DateTime PlanStartTime;
        public DateTime PlanEndTime;
        public string WorkShop;
    }
}
