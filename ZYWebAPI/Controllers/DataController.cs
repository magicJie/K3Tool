using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using ZYJC.Model;
using ZYJC.Importer;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZYWebAPI.Controllers
{
    public class DataController : ApiController
    {
        public string InsertMateriel(JObject jo)
        {
            var materiel = new Materiel();
            materiel.Code = jo["FNumber"].ToString();
            materiel.Name = jo["FName"].ToString();
            materiel.Type = jo["FTypeName"].ToString();
            materiel.BaseUnit = jo["unit"].ToString();
            materiel.Specification = jo["FModel"].ToString();
            materiel.Flag = 'C';
            materiel.K3TimeStamp = DateTime.Now;
            materiel.SourceDb = "XG";
            materiel.ID = Guid.NewGuid().ToString();
            var importer = new MaterielImporter();
            importer.Insert(materiel);
            return "Insert Materiel Success!";
        }

        public string UpdateMateriel(JObject jo)
        {
            var materiel = new Materiel();
            materiel.Code = jo["FNumber"].ToString();
            materiel.Name = jo["FName"].ToString();
            materiel.Type = jo["FTypeName"].ToString();
            materiel.BaseUnit = jo["unit"].ToString();
            materiel.Specification = jo["FModel"].ToString();
            materiel.Flag = 'U';
            materiel.K3TimeStamp = DateTime.Now;
            materiel.SourceDb = "XG";
            //materiel.ID = Guid.NewGuid().ToString();
            var importer = new MaterielImporter();
            importer.Update(materiel);
            return "Update Materiel Success!";
        }

        public string DeleteMateriel(JObject jo)
        {
            var materiel = new Materiel();
            materiel.Code = jo["FNumber"].ToString();
            var importer = new MaterielImporter();
            importer.Delete(materiel);
            return "Delete Materiel Success!";
        }

        public string InsertBOM(JObject jo)
        {
            var bom = new BOM();
            bom.BOMCode = jo["FBOMNumber"].ToString();
            bom.MaterielCode = jo["fshortnumber"].ToString();
            bom.Version = jo["FVersion"].ToString();
            bom.UseState = jo["FStatus"].ToString();
            bom.MaterielQuantity = jo["FQty"].ToString() == "" ? 0 : double.Parse(jo["FQty"].ToString());
            bom.MaterielUnit = jo["FunitID"].ToString();
            bom.DetailCode = jo["FEntryID"].ToString();
            bom.DetailMaterielCode = jo["detailfshortnumber"].ToString();
            bom.DetailQuantity = jo["detailfqty"].ToString() == "" ? 0 : double.Parse(jo["FQty"].ToString());
            bom.DetailUnit = jo["detailFUnitID"].ToString();
            bom.Flag = 'C';
            bom.K3TimeStamp = DateTime.Parse(jo["FEnterTime"].ToString());
            bom.SourceDb = "XG";
            bom.ID = Guid.NewGuid().ToString();
            var importer = new BOMImporter();
            importer.Insert(bom);
            return "Insert BOM Success!";
        }

        public string UpdateBOM(JObject jo)
        {
            var bom = new BOM();
            bom.BOMCode = jo["FBOMNumber"].ToString();
            bom.MaterielCode = jo["fshortnumber"].ToString();
            bom.Version = jo["FVersion"].ToString();
            bom.UseState = jo["FStatus"].ToString();
            bom.MaterielQuantity = jo["FQty"].ToString() == "" ? 0 : double.Parse(jo["FQty"].ToString());
            bom.MaterielUnit = jo["FunitID"].ToString();
            bom.DetailCode = jo["FEntryID"].ToString();
            bom.DetailMaterielCode = jo["detailfshortnumber"].ToString();
            bom.DetailQuantity = jo["detailfqty"].ToString() == "" ? 0 : double.Parse(jo["FQty"].ToString());
            bom.DetailUnit = jo["detailFUnitID"].ToString();
            bom.Flag = 'U';
            bom.K3TimeStamp = DateTime.Parse(jo["FEnterTime"].ToString());
            bom.SourceDb = "XG";
            //bom.ID = Guid.NewGuid().ToString();
            var importer = new BOMImporter();
            importer.Update(bom);
            return "Update BOM Success!";
        }

        public string DeleteBOM(JObject jo)
        {
            var bom = new BOM();
            bom.BOMCode = jo["FBOMNumber"].ToString();
            bom.DetailCode = jo["FEntryID"].ToString();
            var importer = new BOMImporter();
            importer.Delete(bom);
            return "Delete BOM Success!";
        }

        public string InsertProductionPlan(JObject jo)
        {
            var plan = new ProductionPlan();
            if (jo["FBillNo"].ToString().ToString() == "")
                return "";
            //华为只要"JP"开头单据，烽火套账只需要“BB”开头单据
            if (!jo["FBillNo"].ToString().ToString().ToUpper().StartsWith(ConfigurationManager.AppSettings["PlanCodePrefix"].ToString()))
                return "";
            plan.PlanCode = jo["FBillNo"].ToString().ToString();
            plan.WorkOrder = jo["FGMPBatchNo"].ToString().ToString();
            plan.MaterielCode = jo["fshortnumber"].ToString().ToString();
            plan.Planner = jo["FBillerID"].ToString();
            plan.BillDate = DateTime.Parse(jo["FCheckDate"].ToString());
            plan.BOMCode = jo["FBOMNumber"].ToString();
            plan.BOMVersion = jo["FVersion"].ToString();
            plan.OrderState = jo["FStatus"].ToString();
            plan.PlanQuantity = jo["FAuxQty"].ToString() == "" ? 0 : double.Parse(jo["FAuxQty"].ToString());
            plan.BaseUnit = jo["FUnitID"].ToString();
            plan.ProductionType = jo["FType"].ToString();
            if (jo["FPlanCommitDate"].ToString() != "")
                plan.PlanStartTime = DateTime.Parse(jo["FPlanCommitDate"].ToString());
            if (jo["FPlanFinishDate"].ToString() != "")
                plan.PlanEndTime = DateTime.Parse(jo["FPlanFinishDate"].ToString());
            plan.WorkShop = jo["FWorkShop"].ToString();
            plan.Flag = 'C';
            plan.K3TimeStamp = DateTime.Parse(jo["FCheckDate"].ToString());
            plan.SourceDb = "XG";
            plan.Line = "FH";
            plan.ID = Guid.NewGuid().ToString();
            var importer = new ProductionPlanImporter();
            importer.Insert(plan);
            return "Insert ProductionPlan Success!";
        }

        public string UpdateProductionPlan(JObject jo)
        {
            var plan = new ProductionPlan();
            if (jo["FBillNo"].ToString() == "")
                return "";
            //华为只要"JP"开头单据，烽火套账只需要“BB”开头单据
            if (!jo["FBillNo"].ToString().ToUpper().StartsWith(ConfigurationManager.AppSettings["PlanCodePrefix"].ToString()))
                return "";
            plan.PlanCode = jo["FBillNo"].ToString();
            plan.WorkOrder = jo["FGMPBatchNo"].ToString();
            plan.MaterielCode = jo["fshortnumber"].ToString();
            plan.Planner = jo["FBillerID"].ToString();
            plan.BillDate = DateTime.Parse(jo["FCheckDate"].ToString());
            plan.BOMCode = jo["FBOMNumber"].ToString();
            plan.BOMVersion = jo["FVersion"].ToString();
            plan.OrderState = jo["FStatus"].ToString();
            plan.PlanQuantity = jo["FAuxQty"].ToString() == "" ? 0 : double.Parse(jo["FAuxQty"].ToString());
            plan.BaseUnit = jo["FUnitID"].ToString();
            plan.ProductionType = jo["FType"].ToString();
            if (jo["FPlanCommitDate"].ToString() != "")
                plan.PlanStartTime = DateTime.Parse(jo["FPlanCommitDate"].ToString());
            if (jo["FPlanFinishDate"].ToString() != "")
                plan.PlanEndTime = DateTime.Parse(jo["FPlanFinishDate"].ToString());
            plan.WorkShop = jo["FWorkShop"].ToString();
            plan.Flag = 'U';
            plan.K3TimeStamp = DateTime.Parse(jo["FCheckDate"].ToString());
            plan.SourceDb = "XG";
            plan.Line = "FH";
            //plan.ID = Guid.NewGuid().ToString();
            var importer = new ProductionPlanImporter();
            importer.Update(plan);
            return "Update Production Success!";
        }

        public string DeleteProductionPlan(JObject jo)
        {
            var plan = new ProductionPlan();
            if (jo["FBillNo"].ToString() == "")
                return "";
            //华为只要"JP"开头单据，烽火套账只需要“BB”开头单据
            if (jo["DataSource"].ToString() == "HW" && !jo["FBillNo"].ToString().ToUpper().StartsWith("JP") ||
                jo["DataSource"].ToString() == "FH" && !jo["FBillNo"].ToString().ToUpper().StartsWith("BB"))
            {
                return "";
            }
            plan.PlanCode = jo["FBillNo"].ToString();
            var importer = new ProductionPlanImporter();
            importer.Delete(plan);
            return "Delete ProductionPlan Success!";
        }

        public string GetNotify()
        {
            var result = "";
            var type = HttpContext.Current.Request["type"];
            var detail = HttpContext.Current.Request["detail"];
            var dic = new Dictionary<string, string>();
            var jo = (JObject)JsonConvert.DeserializeObject(detail);
            switch (HttpContext.Current.Request["ClaseName"])
            {
                case "Materiel":
                    if (type == "INSERT")
                    {
                        result = InsertMateriel(jo);
                    }
                    else if (type == "UPDATE")
                    {
                        result = UpdateMateriel(jo);
                    }
                    else if (type == "DELETE")
                    {
                        result = DeleteMateriel(jo);
                    }
                    break;
                case "BOM":
                    if (type == "INSERT")
                    {
                        result = InsertBOM(jo);
                    }
                    else if (type == "UPDATE")
                    {
                        result = UpdateBOM(jo);
                    }
                    else if (type == "DELETE")
                    {
                        result = DeleteBOM(jo);
                    }
                    break;
                case "ProductionPlan":
                    if (type == "INSERT")
                    {
                        result = InsertProductionPlan(jo);
                    }
                    else if (type == "UPDATE")
                    {
                        result = UpdateProductionPlan(jo);
                    }
                    else if (type == "DELETE")
                    {
                        result = DeleteProductionPlan(jo);
                    }
                    break;
                default:
                    throw new Exception("不支持的参数！");
            }
            return result;
        }
    }
}