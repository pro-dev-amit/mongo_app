using Matrix.Core.FrameworkCore;
using Matrix.Entities.MongoEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Models.ViewModels
{
    public class BookViewModel
    {
        public Book Book { get; set; }

        public IList<DenormalizedReference> LstAuthor { get; set; }

        public IList<DenormalizedReference> LstCategory { get; set; }
    }
}
