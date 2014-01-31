//using MatrixCore.Framework;
//using Nest;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MatrixCore.SearchCore
//{
//    public class MXElasticRepository
//    {
//        string index;

//        protected ElasticClient client;

//        public MXElasticRepository(MXElasticIndex indexName)
//        {
//            index = indexName.ToString().ToLower();
//            client = new MXElasticClient().Client;
//        }

//        public bool Index<T>(T document) where T : MXEntity 
//        {
//            var res = client.Index<T>(document, index, typeof(T).Name, document.Id);

//            if (!res.OK)
//            {
 
//            }

//            return res.OK;
//        }

//        public void IndexMany<T>(IList<T> documents) where T : MXEntity
//        {
//            var res = client.IndexMany<T>(documents, index);
//        }

//        public T Search<T>(string q) where T : MXEntity 
//        {
            

//            //var result = client.Search<T>(body =>
//            //    body.Query(query =>
//            //    query.QueryString(qs => qs.Query(q))));

//            //var result = client.Search<T>(s => s.QueryString(q));

//        //    var results = client.Search<T>(s => s
//        //            .Size(10)
//        //            .Fields(f => f.Name)
//        //            .MatchAll()
//        //);

//            var results = client.Search<T>(s => s
//                .Index(MXElasticIndex.Element.ToString().ToLower())
//                .Type("element")
//                .From(0)
//                .Size(10)
//                .Fields(f => f.Name)                
//                .SortDescending(f => f.Name)
//                .Query(qu => qu
//                    .Term(f => f.Name, q)
//                )
//            );

//            return results.Documents.FirstOrDefault();
//        }

//        public void DropIndex(MXElasticIndex indexName)
//        {
//            client.DeleteIndex(indexName.ToString().ToLower());
//        }
//    }
//}
