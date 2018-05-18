using log4net;
using SNOWRESTful4DMPM.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SNOWRESTful4DMPM
{
    class Utils
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void WriteToFile(string directory, string tablename, string content)
        {
            string filename = String.Format("{0:yyyy-MM-dd_HH-mm}__{1}", DateTime.Now, tablename);
            string path = Path.Combine(directory, filename);
            File.WriteAllText(path, content);

        }

        public static string RemoveSpecialChars(string str)
        {
            // Create  a string array and add the special characters you want to remove
            string[] chars = new string[] { "'" };
            //Iterate the number of times based on the String array length.
            for (int i = 0; i < chars.Length; i++)
            {
                if (str.Contains(chars[i]))
                {
                    str = str.Replace(chars[i], "");
                }
            }
            return str;
        }

        public static List<Request> ReadSchema (string schemaFile)
        {
            XmlDocument doc = new XmlDocument();
            List<Request> itemsList = new List<Request>();
            doc.Load(schemaFile);

            // Get table name
            XmlNodeList nodes = doc.GetElementsByTagName("item");

            for (int i = 0; i < nodes.Count; i++)
            {
                string _apitbl = nodes[i].Attributes["apitable"].Value;
                string _dbtbl = nodes[i].Attributes["dbtable"].Value;
                string _apifield = nodes[i].SelectSingleNode("apifields").InnerText;
                string _dbfield = nodes[i].SelectSingleNode("dbfields").InnerText;
                string _query = nodes[i].SelectSingleNode("query").InnerText;
                string _limit = nodes[i].SelectSingleNode("limit").InnerText;


                log.Debug($"Schema Source/API Table: { _apitbl}");
                log.Debug($"Schema API Fields: {_apifield}");
                log.Debug($"Schema DB Table: { _dbtbl} ");
                log.Debug($"Schema Query: {_query}");
                log.Debug($"Schema Limit: {_limit}");//Limit to be applied on pagination. The default is 10000.
                itemsList.Add(new Request(_apitbl, _dbtbl, _apifield, _dbfield, _query, _limit));

            }

            return itemsList;

        }

    }
}
