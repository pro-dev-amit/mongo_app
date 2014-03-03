using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.FrameworkCore
{
    public interface IMXEntity
    {
        string Id { get; set; }

        string Name { get; set; }

        bool IsActive { get; set; }

        string CreatedBy { get; set; }

        DateTime CreatedDate { get; set; }
    }
}
