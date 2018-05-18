using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNOWRESTful4DMPM.Entities
{
    class Endpoint
    {

        public string EndPoint { get; set; }
        public string ConnectionStringBD { get; set; }
        public string RestUser { get; set; }
        public string RestPwd { get; set; }
        public string RestContentType { get; set; }
        public string RestTimeout { get; set; }
        public string RestParameters { get; set; }
        public string WriteType { get; set; }
        public string SchemaFile { get; set; }

        public Endpoint()
        {
            this.EndPoint = ConfigurationManager.AppSettings["RestEndpoint"].ToString();
            this.ConnectionStringBD = ConfigurationManager.AppSettings["ConnectionStringBD"].ToString();
            this.RestUser = ConfigurationManager.AppSettings["RestUser"].ToString();
            this.RestPwd = ConfigurationManager.AppSettings["RestPwd"].ToString();
            this.RestContentType = ConfigurationManager.AppSettings["RestContentType"].ToString();
            this.RestTimeout = ConfigurationManager.AppSettings["RestTimeout"].ToString();
            this.RestParameters = ConfigurationManager.AppSettings["RestParameters"].ToString();
            this.WriteType = ConfigurationManager.AppSettings["WriteType"].ToString();
            this.SchemaFile = ConfigurationManager.AppSettings["Schema"].ToString();
        }
    }
}
