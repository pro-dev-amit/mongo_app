using Nest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.SearchCore
{
    public class MXSearchClient : ISearchClient
    {
        protected string connectionString, indexName;

        readonly Lazy<ElasticClient> _client;

        public MXSearchClient()
        {            
            _client = new Lazy<ElasticClient>(getSession);
        }
        
        public ElasticClient Client
        {
            get
            {
                return _client.Value;   
            }
        }

        ElasticClient getSession()
        {
            var setting = new ConnectionSettings(new Uri(connectionString));
            setting.SetDefaultIndex(indexName);

            return new ElasticClient(setting);  
        }

    }//End of class MXSearchClient
}
