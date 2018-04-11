using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace ZYWebAPI.Controllers
{
    public class DataController: ApiController
    {
        public ActionResult GetNotify()
        {
            string id = HttpContext.Current.Request["id"];
            string message = HttpContext.Current.Request["message"];
            //插入数据  
            string txt = string.Empty;
            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            txt = string.Format("{0} ID:{1} Message:{2}", date, id, message);
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                string path = "D:\\111.txt";//文件的路径，保证文件存在。  
                fs = new FileStream(path, FileMode.Append);
                sw = new StreamWriter(fs);
                sw.WriteLine(txt);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sw.Dispose();
                sw.Close();
                fs.Dispose();
                fs.Close();
            }
            return null;
        }
    }
}