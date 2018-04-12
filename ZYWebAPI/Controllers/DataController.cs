using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using ZYJC.Model;
using ZYJC.Importer;
using System.Configuration;

namespace ZYWebAPI.Controllers
{
    public class DataController: ApiController
    {
        public string InsertMateriel()
        {
            var materiel = new Materiel();
            materiel.Code = HttpContext.Current.Request["FNumber"];
            materiel.Name = HttpContext.Current.Request["FName"];
            materiel.Type = HttpContext.Current.Request["FTypeName"];
            materiel.BaseUnit = HttpContext.Current.Request["unit"];
            materiel.Specification = HttpContext.Current.Request["FModel"];
            materiel.Flag = 'C';
            materiel.K3TimeStamp = DateTime.Now;
            materiel.SourceDb = "XG";
            materiel.ID = Guid.NewGuid().ToString();
            var importer = new MaterielImporter();
            importer.Insert(materiel);
            return "Insert Materiel Success!";
        }

        public string UpdateMateriel()
        {
            var materiel = new Materiel();
            materiel.Code = HttpContext.Current.Request["FNumber"];
            materiel.Name = HttpContext.Current.Request["FName"];
            materiel.Type = HttpContext.Current.Request["FTypeName"];
            materiel.BaseUnit = HttpContext.Current.Request["unit"];
            materiel.Specification = HttpContext.Current.Request["FModel"];
            materiel.Flag = 'U';
            materiel.K3TimeStamp = DateTime.Now;
            materiel.SourceDb = "XG";
            //materiel.ID = Guid.NewGuid().ToString();
            var importer = new MaterielImporter();
            importer.Update(materiel);
            return "Update Materiel Success!";
        }

        public string DeleteMateriel()
        {
            var materiel = new Materiel();
            materiel.Code = HttpContext.Current.Request["FNumber"];
            var importer = new MaterielImporter();
            importer.Delete(materiel);
            return "Delete Materiel Success!";
        }

        public string InsertBOM()
        {
            var bom = new BOM();
            bom.BOMCode = HttpContext.Current.Request["FBOMNumber"];
            bom.MaterielCode = HttpContext.Current.Request["fshortnumber"];
            bom.Version = HttpContext.Current.Request["FVersion"];
            bom.UseState = HttpContext.Current.Request["FStatus"];
            bom.MaterielQuantity = HttpContext.Current.Request["FQty"] == "" ? 0 : double.Parse(HttpContext.Current.Request["FQty"]);
            bom.MaterielUnit = HttpContext.Current.Request["FunitID"];
            bom.DetailCode = HttpContext.Current.Request["FEntryID"];
            bom.DetailMaterielCode = HttpContext.Current.Request["detailfshortnumber"];
            bom.DetailQuantity = HttpContext.Current.Request["detailfqty"] == "" ? 0 : double.Parse(HttpContext.Current.Request["FQty"]);
            bom.DetailUnit = HttpContext.Current.Request["detailFUnitID"];
            bom.Flag = 'C';
            bom.K3TimeStamp = DateTime.Parse(HttpContext.Current.Request["FEnterTime"]);
            bom.SourceDb = "XG";
            bom.ID = Guid.NewGuid().ToString();
            var importer = new BOMImporter();
            importer.Insert(bom);
            return "Insert BOM Success!";
        }

        public string UpdateBOM()
        {
            var bom = new BOM();
            bom.BOMCode = HttpContext.Current.Request["FBOMNumber"];
            bom.MaterielCode = HttpContext.Current.Request["fshortnumber"];
            bom.Version = HttpContext.Current.Request["FVersion"];
            bom.UseState = HttpContext.Current.Request["FStatus"];
            bom.MaterielQuantity = HttpContext.Current.Request["FQty"] == "" ? 0 : double.Parse(HttpContext.Current.Request["FQty"]);
            bom.MaterielUnit = HttpContext.Current.Request["FunitID"];
            bom.DetailCode = HttpContext.Current.Request["FEntryID"];
            bom.DetailMaterielCode = HttpContext.Current.Request["detailfshortnumber"];
            bom.DetailQuantity = HttpContext.Current.Request["detailfqty"] == "" ? 0 : double.Parse(HttpContext.Current.Request["FQty"]);
            bom.DetailUnit = HttpContext.Current.Request["detailFUnitID"];
            bom.Flag = 'U';
            bom.K3TimeStamp = DateTime.Parse(HttpContext.Current.Request["FEnterTime"]);
            bom.SourceDb = "XG";
            //bom.ID = Guid.NewGuid().ToString();
            var importer = new BOMImporter();
            importer.Update(bom);
            return "Update BOM Success!";
        }

        public string DeleteBOM()
        {
            var bom = new BOM();
            bom.BOMCode = HttpContext.Current.Request["FBOMNumber"];
            bom.DetailCode = HttpContext.Current.Request["FEntryID"];
            var importer = new BOMImporter();
            importer.Delete(bom);
            return "Delete BOM Success!";
        }

        public string InsertProductionPlan()
        {
            var plan = new ProductionPlan();
            if (HttpContext.Current.Request["FBillNo"] =="")
                return "";
            //华为只要"JP"开头单据，烽火套账只需要“BB”开头单据
            if (!HttpContext.Current.Request["FBillNo"].ToUpper().StartsWith(ConfigurationManager.AppSettings["PlanCodePrefix"]))
                return "";
            plan.PlanCode = HttpContext.Current.Request["FBillNo"];
            plan.WorkOrder = HttpContext.Current.Request["FGMPBatchNo"];
            plan.MaterielCode = HttpContext.Current.Request["fshortnumber"];
            plan.Planner = HttpContext.Current.Request["FBillerID"];
            plan.BillDate = DateTime.Parse(HttpContext.Current.Request["FCheckDate"]);
            plan.BOMCode = HttpContext.Current.Request["FBOMNumber"];
            plan.BOMVersion = HttpContext.Current.Request["FVersion"];
            plan.OrderState = HttpContext.Current.Request["FStatus"];
            plan.PlanQuantity = HttpContext.Current.Request["FAuxQty"] == "" ? 0 : double.Parse(HttpContext.Current.Request["FAuxQty"]);
            plan.BaseUnit = HttpContext.Current.Request["FUnitID"];
            plan.ProductionType = HttpContext.Current.Request["FType"];
            if (HttpContext.Current.Request["FPlanCommitDate"] != "")
                plan.PlanStartTime = DateTime.Parse(HttpContext.Current.Request["FPlanCommitDate"]);
            if (HttpContext.Current.Request["FPlanFinishDate"] != "")
                plan.PlanEndTime = DateTime.Parse(HttpContext.Current.Request["FPlanFinishDate"]);
            plan.WorkShop = HttpContext.Current.Request["FWorkShop"];
            plan.Flag = 'C';
            plan.K3TimeStamp = DateTime.Parse(HttpContext.Current.Request["FCheckDate"]);
            plan.SourceDb = "XG";
            plan.Line = "FH";
            plan.ID = Guid.NewGuid().ToString();
            var importer = new ProductionPlanImporter();
            importer.Insert(plan);
            return "Insert ProductionPlan Success!";
        }

        public string UpdateProductionPlan()
        {
            var plan = new ProductionPlan();
            if (HttpContext.Current.Request["FBillNo"] == "")
                return "";
            //华为只要"JP"开头单据，烽火套账只需要“BB”开头单据
            if (!HttpContext.Current.Request["FBillNo"].ToUpper().StartsWith(ConfigurationManager.AppSettings["PlanCodePrefix"]))
                return "";
            plan.PlanCode = HttpContext.Current.Request["FBillNo"];
            plan.WorkOrder = HttpContext.Current.Request["FGMPBatchNo"];
            plan.MaterielCode = HttpContext.Current.Request["fshortnumber"];
            plan.Planner = HttpContext.Current.Request["FBillerID"];
            plan.BillDate = DateTime.Parse(HttpContext.Current.Request["FCheckDate"]);
            plan.BOMCode = HttpContext.Current.Request["FBOMNumber"];
            plan.BOMVersion = HttpContext.Current.Request["FVersion"];
            plan.OrderState = HttpContext.Current.Request["FStatus"];
            plan.PlanQuantity = HttpContext.Current.Request["FAuxQty"] == "" ? 0 : double.Parse(HttpContext.Current.Request["FAuxQty"]);
            plan.BaseUnit = HttpContext.Current.Request["FUnitID"];
            plan.ProductionType = HttpContext.Current.Request["FType"];
            if (HttpContext.Current.Request["FPlanCommitDate"] != "")
                plan.PlanStartTime = DateTime.Parse(HttpContext.Current.Request["FPlanCommitDate"]);
            if (HttpContext.Current.Request["FPlanFinishDate"] != "")
                plan.PlanEndTime = DateTime.Parse(HttpContext.Current.Request["FPlanFinishDate"]);
            plan.WorkShop = HttpContext.Current.Request["FWorkShop"];
            plan.Flag = 'U';
            plan.K3TimeStamp = DateTime.Parse(HttpContext.Current.Request["FCheckDate"]);
            plan.SourceDb = "XG";
            plan.Line = "FH";
            //plan.ID = Guid.NewGuid().ToString();
            var importer = new ProductionPlanImporter();
            importer.Update(plan);
            return "Update Production Success!";
        }

        public string DeleteProductionPlan()
        {
            var plan = new ProductionPlan();
            if (HttpContext.Current.Request["FBillNo"] == "")
                return "";
            //华为只要"JP"开头单据，烽火套账只需要“BB”开头单据
            if (HttpContext.Current.Request["DataSource"] == "HW" && !HttpContext.Current.Request["FBillNo"].ToUpper().StartsWith("JP") ||
                HttpContext.Current.Request["DataSource"] == "FH" && !HttpContext.Current.Request["FBillNo"].ToUpper().StartsWith("BB"))
            {
                return "";
            }
            plan.PlanCode = HttpContext.Current.Request["FBillNo"];
            var importer = new ProductionPlanImporter();
            importer.Delete(plan);
            return "Delete ProductionPlan Success!";
        }
    }
}