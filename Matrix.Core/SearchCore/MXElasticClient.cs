//using Nest;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Matrix.Core.SearchCore
//{
//    public class MXElasticClient
//    {
//        public ElasticClient Client
//        {
//            get
//            {
//                //return new ElasticClient(new ConnectionSettings(new Uri("http://localhost:9200"))); ;
//                var setting = new ConnectionSettings(new Uri("http://localhost:9200"));                
//                setting.SetDefaultIndex("element");
//                return new ElasticClient(setting);
//            }
//        }


//    }//End of class MXElasticClient
//}
