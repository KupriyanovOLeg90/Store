using MVC_Store.Models.Data;
using System.ComponentModel.DataAnnotations;

namespace MVC_Store.Models.ViewModels.Shop
{
    public class CategoryVM
    {
        public CategoryVM()
        {
        }

        public CategoryVM(CategoryDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Slug = row.Slug;
            Sorting = row.Sorting;
        }

        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 3)]
        public string Name { get; set; }

        public string Slug { get; set; }

        public int Sorting { get; set; }
    }
}