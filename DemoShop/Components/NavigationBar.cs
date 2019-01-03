using Microsoft.AspNetCore.Mvc;

namespace DemoShop.UI.Components
{
    public class NavigationBar : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}