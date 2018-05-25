using System.Security.Cryptography;
using System.Linq;
using System.Text;

namespace ZYJC.Model
{
    /// <summary>
    /// 物料
    /// </summary>   
    public class Materiel: BaseModel
    {
        public string Code { set; get; }
        public string Name { set; get; }
        public string Type { set; get; }
        public string BaseUnit { set; get; }
        public string Specification { set; get; }
    }
}