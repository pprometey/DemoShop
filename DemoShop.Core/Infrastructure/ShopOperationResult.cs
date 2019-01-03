using System.Collections.Generic;
using System.Linq;

namespace DemoShop.Core.Infrastructure
{
    public class ShopOperationResult
    {
        private static readonly ShopOperationResult _success = new ShopOperationResult { Succeeded = true };
        private List<ShopError> _errors = new List<ShopError>();

        public bool Succeeded { get; protected set; }

        public IEnumerable<ShopError> Errors => _errors;

        public static ShopOperationResult Success => _success;

        public static ShopOperationResult Failed(params ShopError[] errors)
        {
            var result = new ShopOperationResult { Succeeded = false };
            if (errors != null)
            {
                result._errors.AddRange(errors);
            }
            return result;
        }

        public override string ToString()
        {
            return Succeeded ?
                   "Succeeded" :
                   string.Format("{0} : {1}", "Failed", string.Join(",", Errors.Select(x => x.Code).ToList()));
        }
    }
}