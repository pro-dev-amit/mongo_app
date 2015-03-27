using Matrix.Core.FrameworkCore;
using Matrix.Core.MongoCore;
using Matrix.Entities.MongoEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Business.ViewModels
{
    /// <summary>
    /// I'm directly using the "Book" MongoEntity here. 
    /// But you just create separate objects for all functionalities such as Employee, Client, Book etc and others and map them using automapper in the custom 
    /// repositories. That would give you the flexibility to even change the underlying datastore without affecting the business model here.
    /// </summary>
    public class BookViewModel
    {
        public Book Book { get; set; }

        public IList<DenormalizedReference> LstAuthor { get; set; }

        public IList<DenormalizedReference> LstCategory { get; set; }
    }
}
