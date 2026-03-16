using Sportopoliten.BLL.DTO.Category;
using Sportopoliten.DAL.Entities;
using Sportopoliten.BLL.DTO.Category;

namespace Sportopoliten.ViewModels.HomeViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Category>? Categories { get; set; }
    }
}