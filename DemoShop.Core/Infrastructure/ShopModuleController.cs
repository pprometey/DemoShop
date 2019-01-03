using System;
using System.Collections.Generic;
using System.Text;

namespace DemoShop.Core.Infrastructure
{
    public abstract class ShopModuleController : ShopController
    {
        protected void AddErrors(ShopOperationResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
