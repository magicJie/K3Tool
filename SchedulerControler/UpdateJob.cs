using MagicBox;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZYJC.Importer;


namespace ZYJC
{
    public class UpdateJob:IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var id = context.JobDetail.Key.Name;
            if (Scheduler.Instance.IsExecuting(id))
            {
                return;
            }
            var logger = log4net.LogManager.GetLogger("logger");

            try
            {
                Scheduler.Instance.SetExecuting(id);
                //检查License
                const string key = "<RSAKeyValue><Modulus>oQDY28PCNjIXjUaIZI8Lp7SqPN3imXERophD6VGAdbsPqyjFo+udn4AKkh2sWJiBhtNvcCM/7RooPc0PcpnHg9vI0ewhtoT96b6kyAVaOc4Vc9Ns8ItIZrvEMS7U0wdXDtkXkClHY16Ofe56rsvb/Lftyt58RIZCJ6cPUxu7owc=</Modulus><Exponent>AQAB</Exponent><P>zX0p58iHpezBAVc4ZnJuWtzG18hzMLbfuQfGyNQvoGM3LnCycrSLroSSahmKmXnLG4o5RJGMuDN0+/UKJ2gucQ==</P><Q>yJRUamCS9cTo6xH5Gv3PIYlx3QXxIrTjMpUYxDjNcBxx69u2GGcxCT0YN45Sl8JaHJN/sXZQB9qjyAMocvYU9w==</Q><DP>thUqhNAUItrfSyjWOTE8yBbJu3tLVuc05ugVJO+gJCbGRshl125UN5gRhSKMxeI+L3ETvcabQr2V84z2KmUOgQ==</DP><DQ>L+HhCQgG7S7Vn5BJ1Fy0vr/N88KAXxWpiyC2qdkZieLweyqXHNo9hYQVLOYH53yZAGtht16UGlvib1P5qv/93w==</DQ><InverseQ>cH+xMYtJT2UySKCLDfMaQNrWvT8BzQpfiZbNLGFm83uo+EgTOGYzPk6Qa4TUphgr0oth7PPa4z/Ye5iHUgwT1Q==</InverseQ><D>B1kRjJ4Xi/+nQYRZjzVUE9hT+KkrwVL+ugv29XwJv9XEtkwLERscu33NfbOq/PQg3TVjHZwOO9T4xzFHpjRMfa6O2lrplLwGBdejbpAgqqpTtAAGvmGCSVujn78hbqtwUy1s+ibm2Rk9udsEuBo1d1FW/qo8Pcw27gMBODOPPmE=</D></RSAKeyValue>";
                if (!File.Exists("license.txt"))
                    throw new Exception("未检测到license文件!");
                var licence = new License("license.txt", key);
                var day = licence.Validate();
                if (day <= 0)
                    throw new Exception("license已过期！");
                else if (day > 0 && day < 30)
                    log4net.LogManager.GetLogger("logger").Warn($"license还有{day}天过期！");
                foreach (Source source in Configuration.Current.Sources)
                {
                    BaseImporter importer = new MaterielImporter(source);
                    //importer.SetLastUpdateTime();
                    logger.Info($"计划任务成功从{source.Name}新增(更新)物料 {importer.Update(new DateTime(1970, 1, 1), DateTime.Now)}条");
                    importer = new BOMImporter(source);
                    //importer.SetLastUpdateTime();
                    logger.Info($"计划任务成功从{source.Name}新增(更新)BOM {importer.Update(new DateTime(1970, 1, 1), DateTime.Now)}条");
                    importer = new ProductionPlanImporter(source);
                    //importer.SetLastUpdateTime();
                    logger.Info($"计划任务成功从{source.Name}新增(更新)生产计划 {importer.Update(new DateTime(1970, 1, 1), DateTime.Now)}条");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            finally
            {
                Scheduler.Instance.SetExecuted(id);
            }
        }
    }
}
