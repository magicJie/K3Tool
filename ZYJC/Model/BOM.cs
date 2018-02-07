namespace ZYJC.Model
{
    /// <summary>
    /// 物料BOM
    /// </summary>   
    public class BOM : BaseModel
    {
        public string BOMCode { set; get; }
        public string MaterielCode { set; get; }
        public string Version { set; get; }
        public string UseState { set; get; }
        public double MaterielQuantity { set; get; }
        public string MaterielUnit { set; get; }
        public string DetailCode { set; get; }
        public string DetailMaterielCode { set; get; }
        public double DetailQuantity { set; get; }
        public string DetailUnit { set; get; }
    }
}
