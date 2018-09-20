using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DataTool.Model
{
    public class ConfigSettings
    {
        public List<RecentDoc> RecentDocs { set; get; } = new List<RecentDoc>();
        public bool OpenRecent { set; get; }= true;
        public bool DefaultNew { set; get; } = true;

        public static ConfigSettings Load(string configFile="")
        {
            var configSettings = new ConfigSettings();
            if (configFile == "")
            {
                if (File.Exists("config.xml"))
                    configFile = "config.xml";
                else if (File.Exists("../config.xml"))
                {
                    configFile = "../config.xml";
                }
                else
                {
                    return configSettings;
                }
            }
            var xDocument = XDocument.Load(configFile);
            configSettings.RecentDocs.AddRange(from recentDoc in xDocument.Descendants("RecentDocs").Descendants("add")
                                               select new RecentDoc() {Name= recentDoc.Attribute("name").Value,FullNanme = recentDoc.Attribute("name").Value } );
            var openRecent = true;
            if (bool.TryParse(xDocument.Element("OpenRecent")?.Value, out openRecent))
            {
                configSettings.OpenRecent = openRecent;
            }
            var defaultNew = true;
            if (bool.TryParse(xDocument.Element("DefaultNew")?.Value, out defaultNew))
            {
                configSettings.DefaultNew = defaultNew;
            }
            return configSettings;
        }
    }

    public class RecentDoc {
        public string Name { set; get; }
        public string FullNanme { set; get; }
    }
}
