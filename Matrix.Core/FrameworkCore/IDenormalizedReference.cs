using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.FrameworkCore
{
    public interface IDenormalizedReference
    {        
        string DenormalizedId { get; set; }

        string DenormalizedName { get; set; }
    }
}
