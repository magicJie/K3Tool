using System;
using System.Collections.Generic;
using ZYJC.Model;
using System.Data.SqlClient;
using System.Linq;
using Oracle.DataAccess.Client;

namespace ZYJC.Importer
{
    public class MaterielImporter : BaseImporter
    {
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
                    $@"select FShortNumber,FName,FTypeID,(SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=t_icitem.FUnitID) unit,
                                 FModel,FLastCheckDate,FItemID from t_icitem 
                                    where FLastCheckDate between CONVERT(datetime, '{startTime}') and CONVERT(datetime, '{endTime}')"
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
                    if (reader["FShortNumber"]==DBNull.Value)
                        continue;
                    materiel.Code = reader["FShortNumber"].ToString();
                    materiel.Name = reader["FName"].ToString();
                    materiel.Type = reader["FTypeID"].ToString();
                    materiel.BaseUnit = reader["unit"].ToString();
                    materiel.Specification = reader["FModel"].ToString();
                    materiel.Flag = 'C';
                    materiel.K3TimeStamp = DateTime.Parse(reader["FLastCheckDate"].ToString());
                    materiel.SourceDb = "HW";
                    models[i] = materiel;
                    i++;
                    if (i == BatchNum)
                    {
                        CommitBatch(models, relatedCmd);
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
                        oddModels[j] = models[i-1];
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

        public override int UpdateImport(DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }
    }
}