using System;
using System.Collections.Generic;
using System.Configuration;
using ZYJC.Model;
using System.Data.SqlClient;
using Oracle.DataAccess.Client;

namespace ZYJC.Importer
{
    public class ProductionPlanImporter : BaseImporter
    {
        public ProductionPlanImporter(Source source) : base(source) { }
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
                    $@"SELECT FBillNo,(select FNumber from t_icitem where t_icitem.FItemID=ICmo.FItemID) as FShortNumber,
(select FName from t_User where t_User.FUserID=ICmo.FBillerID) FBillerID,FCheckDate,(select FBOMNumber from icbom where icbom.FInterID= ICmo.FBomInterID) as FBOMNumber,(select FVersion from icbom where icbom.FInterID= ICmo.FBomInterID) as FVersion,FStatus,FAuxQty,(SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=ICmo.FUnitID)  FUnitID,FType,
FPlanCommitDate,FPlanFinishDate,(select FName from t_Department where t_Department.FItemID=ICmo.FWorkShop) FWorkShop,FWorkTypeID,FConfirmDate,FGMPBatchNo FROM ICmo   
                                    where FStatus=1 and FCheckDate between CONVERT(datetime, '{startTime}') and CONVERT(datetime, '{endTime}')"
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
                    if (reader["FBillNo"] == DBNull.Value)
                        continue;
                    //华为只要"JP"开头单据，烽火套账只需要“BB”开头单据
                    if (!reader["FBillNo"].ToString().ToUpper().StartsWith(Source.PlanCodePrefix))
                        continue;
                    plan.PlanCode = reader["FBillNo"].ToString();
                    plan.WorkOrder = reader["FGMPBatchNo"].ToString();
                    plan.MaterielCode = reader["fshortnumber"].ToString();
                    plan.Planner = reader["FBillerID"].ToString();
                    plan.BillDate = DateTime.Parse(reader["FCheckDate"].ToString());
                    plan.BOMCode = reader["FBOMNumber"].ToString();
                    plan.BOMVersion = reader["FVersion"].ToString();
                    plan.OrderState = reader["FStatus"].ToString();
                    plan.PlanQuantity = reader["FAuxQty"] == DBNull.Value ? 0 : double.Parse(reader["FAuxQty"].ToString());
                    plan.BaseUnit = reader["FUnitID"].ToString();
                    plan.ProductionType = reader["FType"].ToString();
                    if (reader["FPlanCommitDate"] != DBNull.Value)
                        plan.PlanStartTime = DateTime.Parse(reader["FPlanCommitDate"].ToString());
                    if (reader["FPlanFinishDate"] != DBNull.Value)
                        plan.PlanEndTime = DateTime.Parse(reader["FPlanFinishDate"].ToString());
                    plan.WorkShop = reader["FWorkShop"].ToString();
                    plan.Flag = 'C';
                    plan.K3TimeStamp = DateTime.Parse(reader["FCheckDate"].ToString());
                    plan.SourceDb = "XG";
                    plan.Line = "FH";
                    plan.ID = Guid.NewGuid().ToString();
                    models[i] = plan;
                    i++;
                    if (i == BatchNum)
                    {
                        CommitBatch(relatedCmd, models);
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
                        oddModels[j] = models[j];
                    }
                    CommitBatch(relatedCmd, oddModels);
                    result += i;
                }
                reader.Close();
            }
            catch (Exception e)
            {
                log4net.LogManager.GetLogger("Logger").Error(e.ToString() + "\r\n" + sourceCmd.CommandText + "\r\n" + relatedCmd.CommandText);
                throw;
            }
            finally
            {
                SourceConn.Close();
                RelatedConn.Close();
            }
            return result;
        }

        public override int UpdateImport(DateTime startTime, DateTime endTime)
        {
            var a = BackUpdate(startTime, endTime);
            var b = Update(startTime, endTime);
            return a + b;
        }

        public override int BackUpdate(DateTime startTime, DateTime endTime)
        {
            var result = 0;
            try
            {
                SourceConn.Open();
                RelatedConn.Open();
                var readCmd = new OracleCommand()
                {
                    Connection = RelatedConn,
                    CommandText = $"select PlanCode from ProductionPlan where SourceDb='{ Source.Name}'"
                };
                var updateCmd = new OracleCommand
                {
                    Connection = RelatedConn,
                    CommandText = $@"update ProductionPlan set flag='D' where PlanCode=:PlanCode and SourceDb=:SourceDb"
                };
                updateCmd.Parameters.Add(new OracleParameter("PlanCode", OracleDbType.Char));
                updateCmd.Parameters.Add(new OracleParameter("SourceDb", OracleDbType.Char));
                updateCmd.Prepare();
                var reader = readCmd.ExecuteReader();
                var sourceCmd = new SqlCommand
                {
                    Connection = SourceConn,
                    CommandText = $@"SELECT FBillNo,(select FNumber from t_icitem where t_icitem.FItemID=ICmo.FItemID) as FShortNumber,
(select FName from t_User where t_User.FUserID=ICmo.FBillerID) FBillerID,FCheckDate,(select FBOMNumber from icbom where icbom.FInterID= ICmo.FBomInterID) as FBOMNumber,(select FVersion from icbom where icbom.FInterID= ICmo.FBomInterID) as FVersion,FStatus,FAuxQty,(SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=ICmo.FUnitID)  FUnitID,FType,
FPlanCommitDate,FPlanFinishDate,(select FName from t_Department where t_Department.FItemID=ICmo.FWorkShop) FWorkShop,FWorkTypeID,FConfirmDate,FGMPBatchNo FROM ICmo   
                                    where FStatus=1 and FBillNo=@FBillNo"
                };
                sourceCmd.Parameters.Add(new SqlParameter("FBillNo", System.Data.SqlDbType.Char, 8000));
                sourceCmd.Prepare();
                while (reader.Read())
                {
                    sourceCmd.Parameters[0].Value = reader[0];
                    if (sourceCmd.ExecuteScalar() == null)//如果找不到了，则说明源对应的行被删除，需要标记中间表数据为删除状态
                    {
                        updateCmd.Parameters[0].Value = reader[0];
                        updateCmd.ExecuteNonQuery();
                        result++;
                    }
                }
            }
            catch (Exception ex)
            {
                log4net.LogManager.GetLogger("Logger").Error(ex.ToString());
                throw;
            }
            finally
            {
                SourceConn.Close();
                RelatedConn.Close();
            }
            return result;
        }

        public override int Update(DateTime startTime, DateTime endTime)
        {
            var result = 0;
            SourceConn.Open();
            RelatedConn.Open();
            //定义批处理的
            var insertModels = new BaseModel[BatchNum];
            var updateModels = new BaseModel[BatchNum];
            var sourceCmd = new SqlCommand
            {
                Connection = SourceConn,
                CommandText =
                    $@"SELECT FBillNo,(select FNumber from t_icitem where t_icitem.FItemID=ICmo.FItemID) as FShortNumber,
(select FName from t_User where t_User.FUserID=ICmo.FBillerID) FBillerID,FCheckDate,(select FBOMNumber from icbom where icbom.FInterID= ICmo.FBomInterID) as FBOMNumber,(select FVersion from icbom where icbom.FInterID= ICmo.FBomInterID) as FVersion,FStatus,FAuxQty,(SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=ICmo.FUnitID)  FUnitID,FType,
FPlanCommitDate,FPlanFinishDate,(select FName from t_Department where t_Department.FItemID=ICmo.FWorkShop) FWorkShop,FWorkTypeID,FConfirmDate,FGMPBatchNo FROM ICmo   
                                    where FStatus=1 "
                //and FConfirmDate between CONVERT(datetime, '{startTime}') and CONVERT(datetime, '{endTime}')
            };
            var relatedCmd = new OracleCommand
            {
                Connection = RelatedConn,
                CommandText = "select ID||','||HashCode from ProductionPlan where PlanCode=:PlanCode and SourceDb=:SourceDb"
            };
            relatedCmd.Parameters.Add(new OracleParameter("PlanCode", OracleDbType.Char));
            relatedCmd.Parameters.Add(new OracleParameter("SourceDb", OracleDbType.Char));
            relatedCmd.Prepare();
            var reader = sourceCmd.ExecuteReader();
            var insertCmd = new OracleCommand()
            {
                Connection = RelatedConn,
                CommandText = GetInsertCmdText()
            };
            var updateCmd = new OracleCommand()
            {
                Connection = RelatedConn,
                CommandText = GetUpdateCmdText() + $@" where PlanCode=:PlanCode and SourceDb=:SourceDb"
            };
            try
            {
                var i = 0;
                var j = 0;
                while (reader.Read())
                {
                    var plan = new ProductionPlan();
                    if (reader["FBillNo"] == DBNull.Value)
                        continue;
                    //武汉华为只要"JP"开头单据，孝感烽火只需要“BB”开头单据
                    if (!reader["FBillNo"].ToString().ToUpper().StartsWith(Source.PlanCodePrefix))
                        continue;
                    plan.PlanCode = reader["FBillNo"].ToString();
                    plan.WorkOrder = reader["FGMPBatchNo"].ToString();
                    plan.MaterielCode = reader["fshortnumber"].ToString();
                    plan.Planner = reader["FBillerID"].ToString();
                    plan.BillDate = DateTime.Parse(reader["FCheckDate"].ToString());
                    plan.BOMCode = reader["FBOMNumber"].ToString();
                    plan.BOMVersion = reader["FVersion"].ToString();
                    plan.OrderState = reader["FStatus"].ToString();
                    plan.PlanQuantity = reader["FAuxQty"] == DBNull.Value ? 0 : double.Parse(reader["FAuxQty"].ToString());
                    plan.BaseUnit = reader["FUnitID"].ToString();
                    plan.ProductionType = reader["FType"].ToString();
                    if (reader["FPlanCommitDate"] != DBNull.Value)
                        plan.PlanStartTime = DateTime.Parse(reader["FPlanCommitDate"].ToString());
                    if (reader["FPlanFinishDate"] != DBNull.Value)
                        plan.PlanEndTime = DateTime.Parse(reader["FPlanFinishDate"].ToString());
                    plan.WorkShop = reader["FWorkShop"].ToString();
                    plan.Flag = 'C';
                    plan.K3TimeStamp = DateTime.Parse(reader["FCheckDate"].ToString());
                    plan.SourceDb = Source.Name;
                    plan.Line = ConfigurationManager.AppSettings["Line"];
                    plan.CalculateHashCode();
                    relatedCmd.Parameters[0].Value = plan.PlanCode;
                    relatedCmd.Parameters[1].Value = plan.SourceDb;
                    var obj = relatedCmd.ExecuteScalar();
                    if (obj==null)
                    {
                        plan.ID = Guid.NewGuid().ToString();
                        insertModels[i] = plan;
                        i++;
                        if (i == BatchNum)
                        {
                            CommitBatch(insertCmd, insertModels);
                            result += i;
                            i = 0;
                            insertModels = new BaseModel[BatchNum];//重置批
                        }
                    }
                    else
                    {
                        if (plan.HashCode != obj.ToString().Split(',')[1].ToString())
                        {
                            log4net.LogManager.GetLogger("Logger").Info($"检测到生产计划更新【{plan.PlanCode}】");
                            plan.Flag = 'U';
                            plan.ID = obj.ToString().Split(',')[0].ToString();
                            updateModels[j] = plan;
                            j++;
                            if (j == BatchNum)
                            {
                                CommitBatch(updateCmd, updateModels);
                                result += j;
                                j = 0;
                                updateModels = new BaseModel[BatchNum];//重置批
                            }
                        }
                    }
                }
                if (i > 0)
                {
                    var oddModels = new BaseModel[i];
                    for (int k = 0; k < i; k++)
                    {
                        oddModels[k] = insertModels[k];
                    }
                    CommitBatch(insertCmd, oddModels);
                    result += i;
                }
                if (j > 0)
                {
                    var oddModels = new BaseModel[j];
                    for (int k = 0; k < j; k++)
                    {
                        oddModels[k] = updateModels[k];
                    }
                    CommitBatch(updateCmd, oddModels);
                    result += j;
                }
                reader.Close();
            }
            catch (Exception e)
            {
                log4net.LogManager.GetLogger("Logger").Error(e.ToString() + "\r\n" + sourceCmd.CommandText + "\r\n" + insertCmd.CommandText);
                throw;
            }
            finally
            {
                SourceConn.Close();
                RelatedConn.Close();
            }
            return result;
        }

        protected override string GetDeleteCmdText()
        {
            return $@"update ProductionPlan set flag='D' where PlanCode=:PlanCode and SourceDb=:SourceDb";
        }
        protected override void AddDeleteParameter(OracleCommand cmd, BaseModel model)
        {
            cmd.Parameters.Add(new OracleParameter("PlanCode", ((ProductionPlan)model).PlanCode));
        }

        public override DateTime GetLastUpdateTime()
        {
            RelatedConn.Open();
            try
            {
                var cmd = new OracleCommand
                {
                    Connection = RelatedConn,
                    CommandText = $@"select t3 from LastUpdateTime where id={Source.Name}"
                };
                return (DateTime)(cmd.ExecuteScalar());
            }
            finally
            {
                RelatedConn.Close();
            }
        }

        public override void SetLastUpdateTime()
        {
            RelatedConn.Open();
            var tx = RelatedConn.BeginTransaction();
            try
            {
                var cmd = new OracleCommand
                {
                    Connection = RelatedConn,
                    CommandText = $@"update LastUpdateTime set t3=sysdate where id={Source.Name}",
                    Transaction = tx
                };
                cmd.ExecuteNonQuery();
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
            finally
            {
                RelatedConn.Close();
            }
        }
    }
}