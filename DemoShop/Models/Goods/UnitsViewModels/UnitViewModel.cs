using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Models.Goods.UnitsViewModels
{
    public class UnitViewModel
    {
        [Required(ErrorMessage = "Необходимо указать {0}")]
        [DataType(DataType.Text)]
        [Display(Name = "Полное наименование")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Необходимо указать {0}")]
        [DataType(DataType.Text)]
        [Display(Name = "Сокращенное наименование")]
        public string ShortName { get; set; }
    }
}