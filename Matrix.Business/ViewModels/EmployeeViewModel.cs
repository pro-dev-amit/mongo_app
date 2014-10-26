using Matrix.Entities.MongoEntities;
using Matrix.Core.MongoCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matrix.Business.CommonHelpers;
using Matrix.Core.FrameworkCore;

namespace Matrix.Business.ViewModels
{
    public class EmployeeViewModel
    {
        public Employee Employee { get; set; }

        public IList<DenormalizedReference> LstGender { get; set; }
        public IList<DenormalizedReference> LstRating { get; set; }

        public IList<MXCheckBoxItem> LstSkill { get; set; }
    }
}
