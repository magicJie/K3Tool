namespace ZYJC.Model
{
    /// <summary>
    /// 物料BOM
    /// </summary>   
    public class MaterielBOM : BaseModel
    {
        public string BOMCode;
        public string MaterielCode;
        public string Version;
        public string UseState;
        public double MaterielQuantity;
        public string MaterielUnit;
        public string DetailCode;
        public string DetailMaterielCode;
        public string DetailQuantity;
        public string DetailUnit;
    }
}
