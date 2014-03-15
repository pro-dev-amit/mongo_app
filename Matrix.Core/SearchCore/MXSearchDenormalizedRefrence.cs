using Matrix.Core.FrameworkCore;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.SearchCore
{
    public class MXSearchDenormalizedRefrence : IDenormalizedReference
    {
        [ElasticProperty(OmitNorms = true, Index = FieldIndexOption.not_analyzed)]
        public string DenormalizedId { get; set; }

        public string DenormalizedName { get; set; }
        
    }
}
