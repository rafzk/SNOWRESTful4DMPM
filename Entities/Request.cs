using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNOWRESTful4DMPM.Entities
{
    class Request
    {
        private string _apitbl;
        private string _dbtbl;
        private string _apifield;
        private string _dbfield;
        private string _query;
        private string _limit;

        public Request(string apitbl, string dbtbl, string apifield, string dbfield, string query, string limit)
        {
            _apitbl = apitbl;
            _dbtbl = dbtbl;
            _apifield = apifield;
            _dbfield = dbfield;
            _query = query;
            _limit = limit;
        }

        public string ApiTable { get; set; }
        public string DbTable { get; set; }
        public string ApiFieldsList { get; set; }
        public string DbFieldsList { get; set; }
        public string Query { get; set; }
        public string Limit { get; set; }
    }
}
