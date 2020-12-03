using MVC_Store.Models.Data;
using System.ComponentModel.DataAnnotations;

namespace MVC_Store.Models.ViewModels.Pages
{
    public class PageVM
    {
        public PageVM() { 

        }

        public PageVM(PagesDTO row) {
            Id = row.Id;
            Title = row.Title;
            Slag = row.Slag;
            Body = row.Body;
            Sorting = row.Sorting;
            HasSidebar = row.HasSidebar;
        }

        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength:50, MinimumLength = 3)]
        public string Title { get; set; }

        public string Slag { get; set; }
        [Required]
        [StringLength(maximumLength: int.MaxValue, MinimumLength = 3)]
        public string Body { get; set; }

        public int Sorting { get; set; }
        [Display(Name = "Sidebar")]
        public bool HasSidebar { get; set; }
    }
}