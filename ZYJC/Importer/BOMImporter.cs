using System;
using System.Collections.Generic;
using ZYJC.Model;
using System.Data.SqlClient;
using System.Linq;
using Oracle.DataAccess.Client;

namespace ZYJC.Importer
{
    public class BOMImporter : BaseImporter
    {
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
,FInterID,FEnterTime from icbom where fshortnumber like '30%') a right join (
select FEntryID,(select FNumber from t_icitem where t_icitem.FItemID=ICBOMCHILD.FItemID) detailfshortnumber,(select (SELECT FName FROM T_MeasureUnit where T_MeasureUnit.FMeasureUnitID=t_icitem.FUnitID) from t_icitem where t_icitem.FItemID=ICBOMCHILD.FItemID) detailFUnitID,FQty as detailfqty,FInterID from  ICBOMCHILD where detailfshortnumber like '30%' ) b on a.FInterID=b.FInterID
                       where FEnterTime between CONVERT(datetime, '{startTime}') and CONVERT(datetime, '{endTime}')"
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
                    if (reader["FBOMNumber"]==DBNull.Value)
                        continue;
                    bom.BOMCode = reader["FBOMNumber"].ToString();
                    bom.MaterielCode = reader["fshortnumber"].ToString();
                    bom.Version = reader["FVersion"].ToString();
                    bom.UseState = reader["FStatus"].ToString();
                    bom.MaterielQuantity = reader["FQty"]==DBNull.Value?0: double.Parse(reader["FQty"].ToString());
                    bom.MaterielUnit = reader["FunitID"].ToString();
                    bom.DetailCode = reader["FEntryID"].ToString();
                    bom.DetailMaterielCode = reader["detailfshortnumber"].ToString();
                    bom.DetailQuantity = reader["detailfqty"] ==DBNull.Value?0: double.Parse(reader["FQty"].ToString());
                    bom.DetailUnit = reader["detailFUnitID"].ToString();
                    bom.Flag = 'C';
                    bom.K3TimeStamp = DateTime.Parse(reader["FEnterTime"].ToString());
                    bom.SourceDb = "HW";
                    models[i] = bom;
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
                        oddModels[j] = models[j];
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