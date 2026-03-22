using Sportopoliten.DAL.Entities;

namespace Sportopoliten.ViewModels.HomeViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Category>? Categories { get; set; }
    }
}