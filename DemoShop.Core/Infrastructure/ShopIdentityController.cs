using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoShop.Core.Infrastructure
{
    public abstract class ShopIdentityController : ShopController
    {
        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        protected string UnableLoadUser(string userID) { return $"Could not load user id '{userID}'."; }
    }
}
