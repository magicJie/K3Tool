using System;
using System.Collections.Generic;
using ZYJC.Model;
using System.Data.SqlClient;
using Oracle.DataAccess.Client;

namespace ZYJC.Importer
{
    public class MaterielImporter : BaseImporter
    {
        public override int InitImport(DateTime startTime, DateTime endTime)
        {
            var result = 0;
            //定义批处理的
            var materielList = new List<Materiel>();
            SourceConn.Open();
            RelatedConn.Open();
            var sourceCmd = new SqlCommand()
            {
                Connection = SourceConn,
                CommandText =
                    string.Format(@"select (SELECT FName FROM T_MeasureUnit where FMeasureUnitID=FUnitID) unit,
                                    FModel,FName,FItemID,FModifyTime,FShortNumber,FErpClsID,FTypeID,FLastCheckDate
                                from t_icitem 
                                    where FLastCheckDate between CONVERT(datetime, '{0}') and CONVERT(datetime, '{1}')",
                                startTime.ToString(), endTime.ToString())
            };
            var reader = sourceCmd.ExecuteReader();

            var relatedCmd = new OracleCommand()
            {
                Connection = RelatedConn,
                CommandText = @"insert into Materiel(Code,Name,Type,BaseUnit,Specification,Flag,K3TimeStamp)
                          values(:Code,:Name,:Type,:BaseUnit,:Specification,:Flag,:K3TimeStamp)"
            };
            var i = 0;

            try
            {
                while (reader.Read())
                {
                    var materiel = new Materiel();
                    if(string.IsNullOrWhiteSpace(reader["FShortNumber"] as string))
                        continue;
                    materiel.Code = reader["FShortNumber"] as string;
                    materiel.Name = reader["FName"] as string;
                    materiel.Type = reader["FTypeID"] as string;
                    materiel.BaseUnit = reader["unit"] as string;
                    materiel.Specification = reader["FModel"] as string;
                    materiel.Flag = 'C';
                    materiel.K3TimeStamp = DateTime.Parse(reader["FLastCheckDate"].ToString());
                    materielList.Add(materiel);
                    i++;
                    if (i == BatchNum)
                    {
                        CommitBatch(materielList, relatedCmd);
                        result += i;
                        i = 0;
                        materielList = new List<Materiel>();//重置批
                    }
                }
                result += i;
                CommitBatch(materielList, relatedCmd);
                reader.Close();
            }
            catch (Exception e)
            {
                log4net.LogManager.GetLogger("Logger").Error(e.Message+"\r\n"+ sourceCmd.CommandText+"\r\n"+relatedCmd.CommandText);
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

        private List<Materiel> GetMaterielList(DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }

        private void CommitBatch(List<Materiel> materielList, OracleCommand relatedCommand)
        {
            var tx = RelatedConn.BeginTransaction();
            try
            {
                relatedCommand.ArrayBindCount = materielList.Count;

                var arry = new object[7][];
                for (int i = 0; i < arry.Length; i++)
                {
                    arry[i] = new object[BatchNum];
                }
                for (int i = 0; i < materielList.Count; i++)
                {
                    arry[0][i] = materielList[i].Code;
                    arry[1][i] = materielList[i].Name;
                    arry[2][i] = materielList[i].Type;
                    arry[3][i] = materielList[i].BaseUnit;
                    arry[4][i] = materielList[i].Specification;
                    arry[5][i] = materielList[i].Flag;
                    arry[6][i] = materielList[i].K3TimeStamp;
                }
                relatedCommand.Parameters.Add(new OracleParameter("Code", OracleDbType.NVarchar2) {Value = arry[0]});
                relatedCommand.Parameters.Add(new OracleParameter("Name", OracleDbType.NVarchar2) { Value = arry[1] });
                relatedCommand.Parameters.Add(new OracleParameter("Type", OracleDbType.NVarchar2) { Value = arry[2] });
                relatedCommand.Parameters.Add(new OracleParameter("BaseUnit", OracleDbType.NVarchar2) { Value = arry[3] });
                relatedCommand.Parameters.Add(new OracleParameter("Specification", OracleDbType.NVarchar2) { Value = arry[4] });
                relatedCommand.Parameters.Add(new OracleParameter("Flag", OracleDbType.Char) { Value = arry[5] });
                relatedCommand.Parameters.Add(new OracleParameter("K3TimeStamp", OracleDbType.TimeStamp) { Value = arry[6] });

                relatedCommand.ExecuteNonQuery();
                tx.Commit();
            }
            catch (Exception)
            {
                tx.Rollback();
                throw;
            }
        }
    }
}