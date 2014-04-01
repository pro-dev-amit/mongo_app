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
        [ElasticProperty(Name = "id", OmitNorms = true, Index = FieldIndexOption.no, Store = true)]
        public string DenormalizedId { get; set; }

        [ElasticProperty(Name = "nm")]
        public string DenormalizedName { get; set; }
        
    }
}
