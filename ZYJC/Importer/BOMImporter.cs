using System;
using System.Collections.Generic;
using ZYJC.Model;
using System.Data.SqlClient;
using System.Linq;
using Oracle.DataAccess.Client;
using System.Configuration;
using System.Threading;

namespace ZYJC.Importer
{
    public class BOMImporter : BaseImporter
    {
        public BOMImporter() { }

        public BOMImporter(Source source):base(source) {

        }
        public override Type GetModelType()
        {
            return typeof(BOM);
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
                    $@"select * from 
(select FBOMNumber,(select FNumber from t_icitem where t_icitem.FItemID=icbom.FItemID) fshortnumber,FVersion,FStatus,FQty, (SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=icbom.FUnitID) FUnitID
,FInterID,FEnterTime from icbom) a right join (
select FEntryID,(select FNumber from t_icitem where t_icitem.FItemID=ICBOMCHILD.FItemID) detailfshortnumber,(select (SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=t_icitem.FUnitID) from t_icitem where t_icitem.FItemID=ICBOMCHILD.FItemID) detailFUnitID,FQty as detailfqty,FInterID from  ICBOMCHILD ) b on a.FInterID=b.FInterID where fshortnumber like '30%' and detailfshortnumber like '30%'
                       and FEnterTime between CONVERT(datetime, '{startTime}') and CONVERT(datetime, '{endTime}')"
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
                    var bom = new BOM();
                    if (reader["FBOMNumber"] == DBNull.Value)
                        continue;
                    bom.BOMCode = reader["FBOMNumber"].ToString();
                    bom.MaterielCode = reader["fshortnumber"].ToString();
                    bom.Version = reader["FVersion"].ToString();
                    bom.UseState = reader["FStatus"].ToString();
                    bom.MaterielQuantity = reader["FQty"] == DBNull.Value ? 0 : double.Parse(reader["FQty"].ToString());
                    bom.MaterielUnit = reader["FunitID"].ToString();
                    bom.DetailCode = reader["FEntryID"].ToString();
                    bom.DetailMaterielCode = reader["detailfshortnumber"].ToString();
                    bom.DetailQuantity = reader["detailfqty"] == DBNull.Value ? 0 : double.Parse(reader["FQty"].ToString());
                    bom.DetailUnit = reader["detailFUnitID"].ToString();
                    bom.Flag = 'C';
                    bom.K3TimeStamp = DateTime.Parse(reader["FEnterTime"].ToString());
                    bom.SourceDb = "XG";
                    bom.ID = Guid.NewGuid().ToString();
                    models[i] = bom;
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
            SourceConn.Open();
            RelatedConn.Open();
            try
            {
                var readCmd = new OracleCommand()
                {
                    Connection = RelatedConn,
                    CommandText = $"select BOMCode,DetailCode,Version from BOM where SourceDb='{ Source.Name}'"
                };
                var updateCmd = new OracleCommand
                {
                    Connection = RelatedConn,
                    CommandText = $@"update BOM set {Source.Name}del=1 where BOMCode=:BOMCode and DetailCode=:DetailCode and Version=:Version"
                };
                updateCmd.Parameters.Add(new OracleParameter("BOMCode", OracleDbType.Char));
                updateCmd.Parameters.Add(new OracleParameter("DetailCode", OracleDbType.Char));
                updateCmd.Prepare();
                var reader = readCmd.ExecuteReader();
                var sourceCmd = new SqlCommand
                {
                    Connection = SourceConn,
                    CommandText = $@"select * from 
(select FBOMNumber,(select FNumber from t_icitem where t_icitem.FItemID=icbom.FItemID) fshortnumber,FVersion,FStatus,FQty, (SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=icbom.FUnitID) FUnitID
,FInterID,FEnterTime from icbom) a right join (
select FEntryID,(select FNumber from t_icitem where t_icitem.FItemID=ICBOMCHILD.FItemID) detailfshortnumber,(select (SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=t_icitem.FUnitID) from t_icitem where t_icitem.FItemID=ICBOMCHILD.FItemID) detailFUnitID,FQty as detailfqty,FInterID from  ICBOMCHILD ) b on a.FInterID=b.FInterID where fshortnumber like '30%' and detailfshortnumber like '30%'
                       and (FBOMNumber=@FBOMNumber and FEntryID=@FEntryID and FVersion=@FVersion)",
                    CommandTimeout=0
                };
                sourceCmd.Parameters.Add(new SqlParameter("FBOMNumber", System.Data.SqlDbType.Char, 8000));
                sourceCmd.Parameters.Add(new SqlParameter("FEntryID", System.Data.SqlDbType.Char, 8000));
                sourceCmd.Parameters.Add(new SqlParameter("FVersion", System.Data.SqlDbType.Char, 8000));
                sourceCmd.Prepare();
                var a = 0;
                while (reader.Read())
                {
                    sourceCmd.Parameters[0].Value = reader[0];
                    sourceCmd.Parameters[1].Value = reader[1];
                    sourceCmd.Parameters[2].Value = reader[2];
                    //log4net.LogManager.GetLogger("logger").Info($"a:{++a}BOMNumber:{reader[0]}");
                    if (sourceCmd.ExecuteScalar() == null)//如果找不到了，则说明源对应的行被删除，需要标记中间表数据为删除状态
                    {
                        updateCmd.Parameters[0].Value = reader[0];
                        updateCmd.Parameters[1].Value = reader[1];
                        updateCmd.Parameters[2].Value = reader[2]; ;
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
                    $@"select * from 
(select FBOMNumber,(select FNumber from t_icitem where t_icitem.FItemID=icbom.FItemID) fshortnumber,FVersion,FStatus,FQty, (SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=icbom.FUnitID) FUnitID
,FInterID,FEnterTime from icbom) a right join (
select FEntryID,(select FNumber from t_icitem where t_icitem.FItemID=ICBOMCHILD.FItemID) detailfshortnumber,(select (SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=t_icitem.FUnitID) from t_icitem where t_icitem.FItemID=ICBOMCHILD.FItemID) detailFUnitID,FQty as detailfqty,FInterID from  ICBOMCHILD ) b on a.FInterID=b.FInterID where fshortnumber like '30%' and detailfshortnumber like '30%'
                       ",
                CommandTimeout=0
                //and FEnterTime between CONVERT(datetime, '{startTime}') and CONVERT(datetime, '{endTime}'),
            };
            var relatedCmd = new OracleCommand
            {
                Connection = RelatedConn,
                CommandText = "select ID||','||HashCode||','||SourceDb from BOM where BOMCode=:BOMCode and DetailCode=:DetailCode and Version=:Version"
            };
            relatedCmd.Parameters.Add(new OracleParameter("BOMCode", OracleDbType.Char));
            relatedCmd.Parameters.Add(new OracleParameter("DetailCode", OracleDbType.Char));
            relatedCmd.Parameters.Add(new OracleParameter("Version", OracleDbType.Char));
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
                CommandText = GetUpdateCmdText() + $@" where BOMCode=:BOMCode and DetailCode=:DetailCode and Version=:Version"
            };
            try
            {
                var i = 0;
                var j = 0;
                while (reader.Read())
                {
                    var bom = new BOM();
                    if (reader["FBOMNumber"] == DBNull.Value||reader["FVersion"].ToString()=="")
                        continue;
                    bom.BOMCode = reader["FBOMNumber"].ToString();
                    bom.MaterielCode = reader["fshortnumber"].ToString();
                    bom.Version = reader["FVersion"].ToString();
                    bom.UseState = reader["FStatus"].ToString();
                    bom.MaterielQuantity = reader["FQty"] == DBNull.Value ? 0 : double.Parse(reader["FQty"].ToString());
                    bom.MaterielUnit = reader["FunitID"].ToString();
                    bom.DetailCode = reader["FEntryID"].ToString();
                    bom.DetailMaterielCode = reader["detailfshortnumber"].ToString();
                    bom.DetailQuantity = reader["detailfqty"] == DBNull.Value ? 0 : double.Parse(reader["FQty"].ToString());
                    bom.DetailUnit = reader["detailFUnitID"].ToString();
                    bom.Flag = 'C';
                    bom.K3TimeStamp = DateTime.Parse(reader["FEnterTime"].ToString());
                    bom.SourceDb = Source.Name;
                    bom.CalculateHashCode();
                    relatedCmd.Parameters[0].Value = bom.BOMCode;
                    relatedCmd.Parameters[1].Value = bom.DetailCode;
                    relatedCmd.Parameters[2].Value = bom.Version;
                    var obj = relatedCmd.ExecuteScalar();
                    if (obj==null)
                    {
                        bom.ID = Guid.NewGuid().ToString();
                        insertModels[i] = bom;
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
                        var arr = obj.ToString().Split(',');
                        if (bom.HashCode != arr[1])
                        {
                            if (arr[2] != Source.Name && Source.Name != Configuration.Current.MainData)
                                continue;
                            log4net.LogManager.GetLogger("Logger").Info($"检测到BOM更新【{bom.BOMCode}】");
                            bom.ID = obj.ToString().Split(',')[0].ToString();
                            bom.Flag = 'U';
                            updateModels[j] = bom;
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

        protected override void AddDeleteParameter(OracleCommand cmd, BaseModel model)
        {
            cmd.Parameters.Add(new OracleParameter("BOMCode", ((BOM)model).BOMCode));
            cmd.Parameters.Add(new OracleParameter("BOMCode", ((BOM)model).DetailCode));
        }

        public override DateTime GetLastUpdateTime()
        {
            RelatedConn.Open();
            try
            {
                var cmd = new OracleCommand
                {
                    Connection = RelatedConn,
                    CommandText = $@"select t2 from LastUpdateTime where id={Source.Name}"
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
                    CommandText = $@"update LastUpdateTime set t2=sysdate where id={Source.Name}",
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