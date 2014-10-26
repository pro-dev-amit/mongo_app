using Matrix.Core.FrameworkCore;
using Matrix.Core.MongoCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Matrix.Business.CommonHelpers
{
    public class MXCheckBoxItem
    {
        public DenormalizedReference DenormalizedReference { get; set; }

        public bool IsSelected { get; set; }
    }
}