using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MatrixCore.Framework
{

    public class RequiredDenormalizedRefernceAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            var obj = value as DenormalizedReference;
            if (obj == null || string.IsNullOrEmpty(obj.DenormalizedId))
                return false;

            return true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessage,
                ValidationType = "RequiredDenormalizedRefernce"
            };
        }
    }
    
}
