using System;

namespace DemoShop.UI.Models.Identity.UsersViewModels
{
    public class RolesTreeView
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public bool? hasChild { get; set; }
        public bool? isChecked { get; set; }
        public bool? expanded { get; set; }
    }
}