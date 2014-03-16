using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matrix.Entities.MongoEntities;

namespace Matrix.Entities.QueueRequestResponseObjects
{
    public class BooksQueueResponse
    {
        public IList<Book> Books { get; set; }
    }
}
