using System.Collections.Generic;

namespace DemoShop.UI.Models.Identity.UsersViewModels
{
    public class SmallUserViewModel : FullNameViewModel
    {
        public IEnumerable<string> UserRoles { get; set; }
        public IEnumerable<RolesTreeView> Roles { get; set; }

        public SmallUserViewModel()
        {
            UserRoles = new List<string>();
            Roles = new List<RolesTreeView>();
        }
    }
}