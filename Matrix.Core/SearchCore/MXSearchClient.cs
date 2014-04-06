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
        static string _connectionString, _defaultIndex;

        static MXSearchClient()
        {
            _connectionString = ConfigurationManager.AppSettings["elasticSearchConnectionString"];
            _defaultIndex = ConfigurationManager.AppSettings["elasticSearchDefaultIndex"];
        }

        Lazy<ElasticClient> _client = new Lazy<ElasticClient>(() =>
        {
            var setting = new ConnectionSettings(new Uri(_connectionString));
            setting.SetDefaultIndex(_defaultIndex);

            return new ElasticClient(setting);            
        });
        
        public ElasticClient Client
        {
            get
            {
                return _client.Value;   
            }
        }

    }//End of class MXSearchClient

    //discarding this. I do not want to code more.
    //public class MXElasticConnectionSetting
    //{
    //    static string _connectionString, _defaultIndex;

    //    static MXElasticConnectionSetting()
    //    {
    //        _connectionString = ConfigurationManager.AppSettings["elasticSearchConnectionString"];
    //        _defaultIndex = ConfigurationManager.AppSettings["elasticSearchDefaultIndex"];
    //    }

    //    public MXElasticConnectionSetting() { }

    //    public ElasticClient Client
    //    {
    //        get { return getSearchClient(); } 
    //    }

    //    private ElasticClient getSearchClient()
    //    {            
    //        var setting = new ConnectionSettings(new Uri(_connectionString));
    //        setting.SetDefaultIndex(_defaultIndex);

    //        return new ElasticClient(setting);
    //    }
    //}

}
