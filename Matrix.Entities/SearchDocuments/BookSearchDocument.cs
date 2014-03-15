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

        [ElasticProperty(OmitNorms = true, Index = FieldIndexOption.not_analyzed)]
        public int AvaliableCopies { get; set; }
                
        public MXSearchDenormalizedRefrence Author { get; set; }
                
        public MXSearchDenormalizedRefrence Category { get; set; }
    }
}
