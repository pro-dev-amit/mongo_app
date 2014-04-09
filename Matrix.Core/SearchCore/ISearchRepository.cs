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

        bool Index<T>(IList<T> documents, string index = "") where T : MXSearchDocument;

        bool BulkIndex<T>(IList<T> documents, string index = "") where T : MXSearchDocument;

        T GetOne<T>(string id, string index = "", string documentType = "") where T : MXSearchDocument;

        /// <summary>
        /// Searching for a term on all fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="term"></param>
        /// <returns></returns>
        IList<T> GenericSearch<T>(string term, int skip = 0, int take = 30) where T : MXSearchDocument;

        bool Update<T>(T document, string index = "") where T : MXSearchDocument;
    }
}
