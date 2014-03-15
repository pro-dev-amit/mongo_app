using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.SearchCore
{
    public interface ISearchDocument
    {
        string Id { get; set; }

        bool IsActive { get; set; }
    }
}
