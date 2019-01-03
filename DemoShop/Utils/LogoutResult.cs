using DemoShop.Core.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DemoShop.UI.Utils
{
    public class LogoutResult : IActionResult
    {
        private readonly SignInManager<ShopUser> _signInManager;

        public LogoutResult(SignInManager<ShopUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var userName = context.HttpContext.User.Identity.Name;
            var urlHelper = GetUrlHelper(context);

            await _signInManager.SignOutAsync();
            Log.Information("User logged out {User}.", userName);

            var destinationUrl = urlHelper.Action("Index", "Home");

            context.HttpContext.Response.Redirect(destinationUrl);
        }

        public IUrlHelper UrlHelper { get; set; }

        private IUrlHelper GetUrlHelper(ActionContext context)
        {
            var urlHelper = UrlHelper;
            if (urlHelper == null)
            {
                var services = context.HttpContext.RequestServices;
                urlHelper = services.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(context);
            }

            return urlHelper;
        }
    }
}