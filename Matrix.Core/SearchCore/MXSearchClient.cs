using Elasticsearch.Net;
using Elasticsearch.Net.Connection;
using Elasticsearch.Net.ConnectionPool;
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
        protected Lazy<string> connectionString, indexName;

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

        /// <summary>
        /// Providing automatic failover support
        /// </summary>
        /// <returns></returns>
        ElasticClient getSession()
        {
            var nodes = new List<Uri>();

            foreach (var uri in connectionString.Value.Split(',')) //write in web.config as "http://machine1:9200, http://machine2:9200, http://machine3:9200"
            {
                nodes.Add(new Uri(uri));
            }
            
            var setting = new ConnectionSettings(new SniffingConnectionPool(nodes));//new Uri(connectionString.Value));
            
            return new ElasticClient(setting);  
        }

    }//End of class MXSearchClient
}
