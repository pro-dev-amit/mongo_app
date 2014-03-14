using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.SearchCore
{
    public interface ISearchRepository
    {
        string Index<T>(T document, string index = "") where T : MXSearchDocument;

        int IndexMany<T>(IList<T> documents, string index = "") where T : MXSearchDocument;

        T GetOne<T>(string id, string index = "") where T : MXSearchDocument;
    }
}
