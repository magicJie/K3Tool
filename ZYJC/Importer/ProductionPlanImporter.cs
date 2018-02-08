using System;
using System.Collections.Generic;
using ZYJC.Model;
using System.Data.SqlClient;
using Oracle.DataAccess.Client;

namespace ZYJC.Importer
{
    public class ProductionPlanImporter : BaseImporter
    {
        public override Type GetModelType()
        {
            return typeof(ProductionPlan);
        }
        public override int InitImport(DateTime startTime, DateTime endTime)
        {
            var result = 0;
            //定义批处理的
            var models = new BaseModel[BatchNum];
            SourceConn.Open();
            RelatedConn.Open();

            var sourceCmd = new SqlCommand()
            {
                Connection = SourceConn,
                CommandText =
                    $@"SELECT FBrNo,(select FShortNumber from t_icitem where t_icitem.FItemID=ICmo.FItemID) as FShortNumber,
(select FName from t_Item where t_Item.FItemID=ICmo.FBillerID) FBillerID,FCheckDate,(select FBOMNumber from icbom where icbom.FInterID= ICmo.FBomInterID) as FBOMNumber,(select FVersion from icbom where icbom.FInterID= ICmo.FBomInterID) as FVersion,FStatus,FAuxQty,(SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=ICmo.FUnitID)  FUnitID,FType,
FPlanCommitDate,FPlanFinishDate,FWorkShop,FWorkTypeID,FConfirmDate,FGMPBatchNo FROM ICmo   
                                    where FCheckDate between CONVERT(datetime, '{startTime}') and CONVERT(datetime, '{endTime}')"
            };
            var reader = sourceCmd.ExecuteReader();
            var relatedCmd = new OracleCommand
            {
                Connection = RelatedConn,
                CommandText = GetInsertCmdText()
            };
            var i = 0;
            try
            {
                while (reader.Read())
                {
                    var plan = new ProductionPlan();
                    if (string.IsNullOrWhiteSpace(reader["FBrNo"] as string))
                        continue;
                    plan.PlanCode = (string)reader["FBrNo"];
                    plan.WorkOrder = (string)reader["FGMPBatchNo"];
                    plan.MaterielCode = reader["fshortnumber"] as string;
                    plan.Planner = reader["FBillerID"] as string;
                    plan.BillDate = DateTime.Parse(reader["FCheckDate"].ToString());
                    plan.BOMCode = reader["FBOMNumber"] as string;
                    plan.BOMVersion = reader["FVersion"] as string;
                    plan.OrderState = reader["FStatus"] as string;
                    plan.PlanQuantity = reader["FAuxQty"] == null ? 0 : double.Parse(reader["FAuxQty"].ToString());
                    plan.BaseUnit = reader["FUnitID"] as string;
                    plan.ProductionType = reader["FType"]?.ToString() ?? "";
                    if (reader["FPlanCommitDate"] != null)
                        plan.PlanStartTime = DateTime.Parse(reader["FPlanCommitDate"].ToString());
                    if (reader["FPlanFinishDate"] != null)
                        plan.PlanEndTime = DateTime.Parse(reader["FPlanFinishDate"].ToString());
                    plan.WorkShop = reader["FWorkShop"] as string;
                    plan.Flag = 'C';
                    plan.K3TimeStamp = DateTime.Parse(reader["FCheckDate"].ToString());
                    models[i] = plan;
                    i++;
                    if (i == BatchNum)
                    {
                        CommitBatch(models, relatedCmd);
                        result += i;
                        i = 0;
                        models = new BaseModel[BatchNum];//重置批
                    }
                }
                if (i > 0)
                {
                    var oddModels = new BaseModel[i];
                    for (int j = 0; j < i; j++)
                    {
                        oddModels[j] = models[i - 1];
                    }
                    CommitBatch(oddModels, relatedCmd);
                    result += i;
                }
                reader.Close();
            }
            catch (Exception e)
            {
                log4net.LogManager.GetLogger("Logger").Error(e.Message + "\r\n" + sourceCmd.CommandText + "\r\n" + relatedCmd.CommandText);
                throw;
            }
            finally
            {
                SourceConn.Close();
                RelatedConn.Close();
            }
            return result;
        }
    }
}