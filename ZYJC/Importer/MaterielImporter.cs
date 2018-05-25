using System;
using System.Collections.Generic;
using ZYJC.Model;
using System.Data.SqlClient;
using System.Linq;
using Oracle.DataAccess.Client;
using System.Configuration;

namespace ZYJC.Importer
{
    public class MaterielImporter : BaseImporter
    {
        public MaterielImporter(Source source):base(source) {
        }

        public override Type GetModelType()
        {
            return typeof(Materiel);
        }

        public override int InitImport(DateTime startTime, DateTime endTime)
        {
            var result = 0;
            //定义批处理的
            var models = new BaseModel[BatchNum];
            SourceConn.Open();
            RelatedConn.Open();
            var sourceCmd = new SqlCommand
            {
                Connection = SourceConn,
                CommandText =
                    $@"select FNumber,FName,(select FName from t_SubMessage where t_SubMessage.FInterID=t_icitem.FErpClsID) FTypeName,(SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=t_icitem.FUnitID) unit,
                                 FModel,FLastCheckDate,FItemID from t_icitem 
                                    where FNumber like '30%' and FLastCheckDate between CONVERT(datetime, '{startTime}') and CONVERT(datetime, '{endTime}')"
            };
            var reader = sourceCmd.ExecuteReader();
            var relatedCmd = new OracleCommand()
            {
                Connection = RelatedConn,
                CommandText = GetInsertCmdText()
            };
            try
            {
                var i = 0;
                while (reader.Read())
                {
                    var materiel = new Materiel();
                    if (reader["FNumber"] ==DBNull.Value)
                        continue;
                    materiel.Code = reader["FNumber"].ToString();
                    materiel.Name = reader["FName"].ToString();
                    materiel.Type = reader["FTypeName"].ToString();
                    materiel.BaseUnit = reader["unit"].ToString();
                    materiel.Specification = reader["FModel"].ToString();
                    materiel.Flag = 'C';
                    materiel.K3TimeStamp = DateTime.Now;
                    materiel.SourceDb = "XG";
                    materiel.ID = Guid.NewGuid().ToString();
                    models[i] = materiel;
                    i++;
                    if (i == BatchNum)
                    {
                        CommitBatch(relatedCmd, models);
                        result += i;
                        i = 0;
                        models=new BaseModel[BatchNum];//重置批
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
                    CommandText = $"select Code from materiel where SourceDb='{Source.Name}'"
                };
                var updateCmd = new OracleCommand
                {
                    Connection = RelatedConn,
                    CommandText = GetDeleteCmdText()
                };
                updateCmd.Parameters.Add(new OracleParameter("Code", OracleDbType.Char));
                updateCmd.Parameters.Add(new OracleParameter("SourceDb", OracleDbType.Char));
                updateCmd.Prepare();
                var reader = readCmd.ExecuteReader();
                var sourceCmd = new SqlCommand
                {
                    Connection = SourceConn,
                    CommandText = $@"select FNumber,FName,(select FName from t_SubMessage where t_SubMessage.FInterID=t_icitem.FErpClsID) FTypeName,(SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=t_icitem.FUnitID) unit,
                                 FModel, FLastCheckDate, FItemID from t_icitem where FNumber like '30%' and FNumber=@FNumber"
                };
                sourceCmd.Parameters.Add(new SqlParameter("FNumber", System.Data.SqlDbType.Char,8000));
                sourceCmd.Prepare();
                while (reader.Read())
                {
                    sourceCmd.Parameters[0].Value = reader[0];
                    if (sourceCmd.ExecuteScalar() == null)//如果找不到了，则说明源对应的行被删除，需要标记中间表数据为删除状态
                    {
                        updateCmd.Parameters[0].Value = reader[0];
                        updateCmd.Parameters[1].Value = Source.Name;
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
                    $@"select FNumber,FName,(select FName from t_SubMessage where t_SubMessage.FInterID=t_icitem.FErpClsID) FTypeName,(SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=t_icitem.FUnitID) unit,
                                 FModel,FLastCheckDate,FItemID from t_icitem 
                                    where FNumber like '30%' "
                // and FLastCheckDate between CONVERT(datetime, '{startTime}') and CONVERT(datetime, '{endTime}')
            };
            var relatedCmd = new OracleCommand
            {
                Connection = RelatedConn,
                CommandText = "select ID||','||HashCode from Materiel where Code=:Code and SourceDb=:SourceDb"
            };
            relatedCmd.Parameters.Add(new OracleParameter("Code", OracleDbType.Char));
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
                CommandText = GetUpdateCmdText()+ $@" where Code=:Code  and SourceDb=:SourceDb"
            };
            try
            {
                var i = 0;
                var j = 0;
                while (reader.Read())
                {
                    var materiel = new Materiel();
                    if (reader["FNumber"] == DBNull.Value)
                        continue;
                    materiel.Code = reader["FNumber"].ToString();
                    materiel.Name = reader["FName"].ToString();
                    materiel.Type = reader["FTypeName"].ToString();
                    materiel.BaseUnit = reader["unit"].ToString();
                    materiel.Specification = reader["FModel"].ToString();
                    materiel.Flag = 'C';
                    materiel.K3TimeStamp = DateTime.Now;
                    materiel.SourceDb = Source.Name;
                    materiel.CalculateHashCode();
                    relatedCmd.Parameters[0].Value = materiel.Code;
                    relatedCmd.Parameters[1].Value = materiel.SourceDb;
                    var obj = relatedCmd.ExecuteScalar();
                    if (obj==null)
                    {
                        materiel.ID = Guid.NewGuid().ToString();
                        insertModels[i] = materiel;
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
                        //对比哈希值决定是否需要更新
                        if (materiel.HashCode != obj.ToString().Split(',')[1].ToString())
                        {
                            log4net.LogManager.GetLogger("Logger").Info($"检测到物料更新【{materiel.Code}】");
                            materiel.ID = obj.ToString().Split(',')[0].ToString();
                            materiel.Flag = 'U';
                            updateModels[j] = materiel;
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
                log4net.LogManager.GetLogger("Logger").Error(e.ToString() + "\r\n" + sourceCmd.CommandText + "\r\n" + insertCmd.CommandText+ "\r\n" + updateCmd.CommandText);
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
            return $@"update materiel set flag = 'D' where Code =:Code and SourceDb=:SourceDb";
        }

        protected override void AddDeleteParameter(OracleCommand cmd, BaseModel model)
        {
            cmd.Parameters.Add(new OracleParameter("Code", ((Materiel)model).Code));
        }

        public override DateTime GetLastUpdateTime()
        {
            RelatedConn.Open();
            try
            {
                var cmd = new OracleCommand
                {
                    Connection = RelatedConn,
                    CommandText = $@"select t1 from LastUpdateTime where id={Source.Name}"
                };
                var time = cmd.ExecuteScalar();
                if (time == null)
                    return new DateTime(1970,1,1);
                return (DateTime)(time);
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
                    CommandText = $@"update LastUpdateTime set t1=sysdate where id={Source.Name}",
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