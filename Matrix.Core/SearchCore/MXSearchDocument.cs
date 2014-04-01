using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.SearchCore
{
    public class MXSearchDocument : ISearchDocument
    {
        [ElasticProperty(OmitNorms = true, Index = FieldIndexOption.no, Store = true)]
        public string Id { get; set; }

        [ElasticProperty(OmitNorms = true, Index = FieldIndexOption.not_analyzed)]
        public bool IsActive { get; set; }
    }
}
