using Matrix.Core.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Matrix.Models.CommonHelpers
{
    public class MXCheckBoxItem
    {
        public DenormalizedReference DenormalizedReference { get; set; }

        public bool IsSelected { get; set; }
    }
}