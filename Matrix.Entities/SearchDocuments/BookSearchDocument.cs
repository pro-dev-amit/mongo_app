using Matrix.Core.SearchCore;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Entities.SearchDocuments
{
    public class BookSearchDocument : MXSearchDocument
    {
        public string Title { get; set; }

        //store the field but do not index it.
        [ElasticProperty(Name = "aC", OmitNorms = true, Index = FieldIndexOption.no, Store = true)]
        public int AvaliableCopies { get; set; }
        
        [ElasticProperty(Name= "au")]        
        public MXSearchDenormalizedRefrence Author { get; set; }

        [ElasticProperty(Name = "ca")]  
        public MXSearchDenormalizedRefrence Category { get; set; }
    }
}
