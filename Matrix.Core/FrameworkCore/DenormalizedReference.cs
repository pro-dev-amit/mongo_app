using Matrix.Core.FrameworkCore;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.FrameworkCore
{
    /// <summary>
    /// this is a special custom type and used throughout the framework for some very general perposes.
    /// </summary>
    public class DenormalizedReference : IDenormalizedReference
    {     
        public string DenormalizedId { get; set; }
                
        public string DenormalizedName { get; set; }
    }
}
