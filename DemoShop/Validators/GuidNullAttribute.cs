using System;
using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Validators
{
    public class GuidNullAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "Некорректный GUID";

        public override bool IsValid(object value)
        {
            if (value == null) return false;

            bool s = (Guid)value != Guid.Empty ? true : false;
            return s;
        }
    }
}